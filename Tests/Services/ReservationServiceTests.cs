using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Application.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Moq;
using Tests.Mocks;
using Xunit;

namespace Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly Mock<IStationInventoryRepository> _inventoryRepoMock;
        private readonly Mock<IReservationAllocationRepository> _allocationRepoMock;
        private readonly IMapper _mapper;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _reservationRepoMock = new Mock<IReservationRepository>();
            _inventoryRepoMock = new Mock<IStationInventoryRepository>();
            _allocationRepoMock = new Mock<IReservationAllocationRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Reservation, ReservationDto>().ReverseMap();
                cfg.CreateMap<ReservationAllocation, ReservationAllocationDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _service = new ReservationService(
                _reservationRepoMock.Object,
                _inventoryRepoMock.Object,
                _allocationRepoMock.Object,
                _mapper
            );
        }

        // 🧪 1️⃣ Tạo reservation thành công
        [Fact]
        public async Task CreateReservation_ShouldCreateSuccessfully_WhenBatteryAvailable()
        {
            // Arrange
            var request = new CreateReservationRequest
            {
                UserId = "user1",
                StationId = 1,
                VehicleId = 10,
                ReservedBatteryModelId = 100,
                ReservedFrom = DateTime.UtcNow.AddMinutes(10),
                ReservedTo = DateTime.UtcNow.AddMinutes(40)
            };

            var availableBattery = MockDataHelper.CreateBattery(5, 100);

            _reservationRepoMock.Setup(r => r.GetByUserId("user1"))
                .ReturnsAsync(new List<Reservation>());

            _inventoryRepoMock.Setup(r => r.CountAvailableBatteries(1, 100))
                .ReturnsAsync(1);

            _inventoryRepoMock.Setup(r => r.GetFullBatteriesByModel(1, 100))
                .ReturnsAsync(new List<Battery> { availableBattery });

            _allocationRepoMock.Setup(a => a.GetActiveByBattery(5, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((ReservationAllocation?)null);

            _reservationRepoMock.Setup(r => r.Add(It.IsAny<Reservation>()))
                .Callback<Reservation>(r => r.ReservationId = 99)
                .Returns(Task.CompletedTask);

            _allocationRepoMock.Setup(a => a.Add(It.IsAny<ReservationAllocation>()))
                .Returns(Task.CompletedTask);

            _inventoryRepoMock.Setup(i => i.MarkHeld(5, 1, 99))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateReservation(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(99, result.ReservationId);
            Assert.Equal(ReservationStatus.Pending, result.Status);
            Assert.Equal(ReservationAllocationStatus.Active, result.Allocation.Status);
            _inventoryRepoMock.Verify(i => i.MarkHeld(5, 1, 99), Times.Once);
        }

        // 🧪 2️⃣ Tạo reservation trùng giờ → throw
        [Fact]
        public async Task CreateReservation_ShouldThrow_WhenOverlappingReservationExists()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var request = new CreateReservationRequest
            {
                UserId = "user1",
                StationId = 1,
                VehicleId = 10,
                ReservedFrom = now.AddMinutes(5),
                ReservedTo = now.AddMinutes(60)
            };

            var existing = MockDataHelper.CreateOverlappingReservations("user1", now).ToList();

            _reservationRepoMock.Setup(r => r.GetByUserId("user1"))
                .ReturnsAsync(existing);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateReservation(request));
        }

        // 🧪 3️⃣ Không còn pin → throw
        [Fact]
        public async Task CreateReservation_ShouldThrow_WhenNoBatteriesAvailable()
        {
            // Arrange
            var request = new CreateReservationRequest
            {
                UserId = "user1",
                StationId = 1,
                VehicleId = 10,
                ReservedBatteryModelId = 100,
                ReservedFrom = DateTime.UtcNow.AddMinutes(10),
                ReservedTo = DateTime.UtcNow.AddMinutes(40)
            };

            _reservationRepoMock.Setup(r => r.GetByUserId("user1"))
                .ReturnsAsync(new List<Reservation>());

            _inventoryRepoMock.Setup(r => r.CountAvailableBatteries(1, 100))
                .ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateReservation(request));
        }

        // 🧪 4️⃣ Hủy reservation → release pin
        [Fact]
        public async Task CancelReservation_ShouldReleaseBatteryAndMarkCancelled()
        {
            // Arrange
            var reservation = MockDataHelper.CreateReservation(10, "user1", 2, 1, ReservationStatus.Pending);
            var allocations = new List<ReservationAllocation>
            {
                MockDataHelper.CreateAllocation(1, 10, 7, ReservationAllocationStatus.Active)
            };

            _reservationRepoMock.Setup(r => r.GetById(10))
                .ReturnsAsync(reservation);

            _reservationRepoMock.Setup(r => r.Cancel(10))
                .Returns(Task.CompletedTask);

            _allocationRepoMock.Setup(a => a.ReleaseByReservation(10, ReservationAllocationStatus.Released))
                .Returns(Task.CompletedTask);

            _allocationRepoMock.Setup(a => a.GetByReservationId(10))
                .ReturnsAsync(allocations);

            _inventoryRepoMock.Setup(i => i.MarkFull(7, 2))
                .Returns(Task.CompletedTask);

            // Act
            await _service.CancelReservation(new CancelReservationRequest
            {
                ReservationId = 10,
                UserId = "user1"
            });

            // Assert
            _reservationRepoMock.Verify(r => r.Cancel(10), Times.Once);
            _inventoryRepoMock.Verify(i => i.MarkFull(7, 2), Times.Once);
        }
    }
}
