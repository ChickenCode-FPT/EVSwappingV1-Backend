using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Xunit;

namespace Tests.Jobs
{
    public class ReservationStatusUpdaterJobTests
    {
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly Mock<ISwapTransactionRepository> _transactionRepoMock;
        private readonly Mock<ILogger<ReservationStatusUpdaterJob>> _loggerMock;
        private readonly ReservationStatusUpdaterJob _job;

        public ReservationStatusUpdaterJobTests()
        {
            _reservationRepoMock = new Mock<IReservationRepository>();
            _transactionRepoMock = new Mock<ISwapTransactionRepository>();
            _loggerMock = new Mock<ILogger<ReservationStatusUpdaterJob>>();

            _job = new ReservationStatusUpdaterJob(
                _reservationRepoMock.Object,
                _transactionRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Execute_ShouldMarkCompleted_WhenTransactionExists()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var pendingRes = new Reservation
            {
                ReservationId = 10,
                Status = ReservationStatus.Pending,
                ReservedTo = now.AddMinutes(30)
            };

            _reservationRepoMock.Setup(r => r.GetPendingReservations())
                .ReturnsAsync(new List<Reservation> { pendingRes });

            _transactionRepoMock.Setup(t => t.ExistsByReservationId(10))
                .ReturnsAsync(true);

            _reservationRepoMock.Setup(r => r.SaveChanges())
                .Returns(Task.CompletedTask);

            var context = Mock.Of<IJobExecutionContext>();

            // Act
            await _job.Execute(context);

            // Assert
            Assert.Equal(ReservationStatus.Completed, pendingRes.Status);
            _reservationRepoMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldMarkExpired_WhenReservationPastDue()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var pendingRes = new Reservation
            {
                ReservationId = 11,
                Status = ReservationStatus.Pending,
                ReservedTo = now.AddMinutes(-5)
            };

            _reservationRepoMock.Setup(r => r.GetPendingReservations())
                .ReturnsAsync(new List<Reservation> { pendingRes });

            _transactionRepoMock.Setup(t => t.ExistsByReservationId(It.IsAny<int>()))
                .ReturnsAsync(false);

            _reservationRepoMock.Setup(r => r.SaveChanges())
                .Returns(Task.CompletedTask);

            var context = Mock.Of<IJobExecutionContext>();

            // Act
            await _job.Execute(context);

            // Assert
            Assert.Equal(ReservationStatus.Expired, pendingRes.Status);
            _reservationRepoMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldDoNothing_WhenNoPendingReservations()
        {
            // Arrange
            _reservationRepoMock.Setup(r => r.GetPendingReservations())
                .ReturnsAsync(new List<Reservation>());

            var context = Mock.Of<IJobExecutionContext>();

            // Act
            await _job.Execute(context);

            // Assert
            _reservationRepoMock.Verify(r => r.SaveChanges(), Times.Never);
        }
    }
}
