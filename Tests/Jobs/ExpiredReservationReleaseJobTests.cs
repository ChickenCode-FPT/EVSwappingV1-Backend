using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Tests.Mocks;
using Xunit;

namespace Tests.Jobs
{
    public class ExpiredReservationReleaseJobTests
    {
        private readonly Mock<IReservationAllocationRepository> _allocationRepoMock;
        private readonly Mock<IStationInventoryRepository> _inventoryRepoMock;
        private readonly Mock<ILogger<ExpiredReservationReleaseJob>> _loggerMock;
        private readonly ExpiredReservationReleaseJob _job;

        public ExpiredReservationReleaseJobTests()
        {
            _allocationRepoMock = new Mock<IReservationAllocationRepository>();
            _inventoryRepoMock = new Mock<IStationInventoryRepository>();
            _loggerMock = new Mock<ILogger<ExpiredReservationReleaseJob>>();

            _job = new ExpiredReservationReleaseJob(
                _allocationRepoMock.Object,
                _inventoryRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Execute_ShouldExpireAllocations_AndMarkBatteryFull()
        {
            // Arrange
            var expiredAlloc = MockDataHelper.CreateExpiredAllocation(1, 5, 10);
            expiredAlloc.Reservation = new Reservation { StationId = 2 };

            _allocationRepoMock.Setup(r => r.GetExpiredAllocations(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<ReservationAllocation> { expiredAlloc });

            _inventoryRepoMock.Setup(i => i.MarkFull(5, 2))
                .Returns(Task.CompletedTask);

            _allocationRepoMock.Setup(r => r.SaveChanges())
                .Returns(Task.CompletedTask);

            var context = Mock.Of<IJobExecutionContext>();

            // Act
            await _job.Execute(context);

            // Assert
            Assert.Equal(ReservationAllocationStatus.Expired, expiredAlloc.Status);
            _inventoryRepoMock.Verify(i => i.MarkFull(5, 2), Times.Once);
            _allocationRepoMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldDoNothing_WhenNoExpiredAllocations()
        {
            // Arrange
            _allocationRepoMock.Setup(r => r.GetExpiredAllocations(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<ReservationAllocation>());

            var context = Mock.Of<IJobExecutionContext>();

            // Act
            await _job.Execute(context);

            // Assert
            _inventoryRepoMock.Verify(i => i.MarkFull(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _allocationRepoMock.Verify(r => r.SaveChanges(), Times.Never);
        }
    }
}
