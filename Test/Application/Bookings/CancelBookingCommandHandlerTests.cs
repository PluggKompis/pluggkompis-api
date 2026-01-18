using Application.Bookings.Commands.CancelBooking;
using Application.Common.Interfaces;
using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;
using FakeItEasy;

namespace Test.UnitTests.Bookings
{
    [TestFixture]
    public class CancelBookingCommandHandlerTests
    {
        private IGenericRepository<Booking> _bookingRepo = null!;
        private ITimeSlotRepository _timeSlotRepo = null!;
        private CancelBookingCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _bookingRepo = A.Fake<IGenericRepository<Booking>>();
            _timeSlotRepo = A.Fake<ITimeSlotRepository>();

            _handler = new CancelBookingCommandHandler(
                _bookingRepo,
                _timeSlotRepo
            );
        }

        [Test]
        public async Task Handle_WhenLessThan2HoursUntilSession_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var timeSlotId = Guid.NewGuid();

            // Session starts in 1 hour (less than 2-hour window)
            var sessionDate = DateTime.UtcNow.Date;
            var sessionStartTime = TimeSpan.FromHours(DateTime.UtcNow.Hour + 1);

            var booking = new Booking
            {
                Id = bookingId,
                BookedByUserId = userId,
                TimeSlotId = timeSlotId,
                BookingDate = sessionDate,
                Status = BookingStatus.Confirmed
            };

            var timeSlot = new TimeSlot
            {
                Id = timeSlotId,
                StartTime = sessionStartTime,
                EndTime = sessionStartTime.Add(TimeSpan.FromHours(2))
            };

            A.CallTo(() => _bookingRepo.GetByIdAsync(bookingId))
                .Returns(booking);

            A.CallTo(() => _timeSlotRepo.GetByIdAsync(timeSlotId))
                .Returns(timeSlot);

            var command = new CancelBookingCommand(userId, bookingId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors.First().ToLower(), Does.Contain("2 hours"));
        }

        [Test]
        public async Task Handle_WhenUserDoesNotOwnBooking_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();

            var booking = new Booking
            {
                Id = bookingId,
                BookedByUserId = otherUserId, // Different user
                Status = BookingStatus.Confirmed
            };

            A.CallTo(() => _bookingRepo.GetByIdAsync(bookingId))
                .Returns(booking);

            var command = new CancelBookingCommand(userId, bookingId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors.First().ToLower(), Does.Contain("your own bookings"));
        }

        [Test]
        public async Task Handle_WhenValidCancellation_ShouldSucceed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var timeSlotId = Guid.NewGuid();

            // Session starts in 3 hours (within cancellation window)
            var sessionDate = DateTime.UtcNow.Date.AddDays(1);
            var sessionStartTime = TimeSpan.FromHours(14);

            var booking = new Booking
            {
                Id = bookingId,
                BookedByUserId = userId,
                TimeSlotId = timeSlotId,
                BookingDate = sessionDate,
                Status = BookingStatus.Confirmed
            };

            var timeSlot = new TimeSlot
            {
                Id = timeSlotId,
                StartTime = sessionStartTime,
                EndTime = sessionStartTime.Add(TimeSpan.FromHours(2))
            };

            A.CallTo(() => _bookingRepo.GetByIdAsync(bookingId))
                .Returns(booking);

            A.CallTo(() => _timeSlotRepo.GetByIdAsync(timeSlotId))
                .Returns(timeSlot);

            A.CallTo(() => _bookingRepo.UpdateAsync(A<Booking>._))
                .Returns(Task.CompletedTask);

            var command = new CancelBookingCommand(userId, bookingId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);

            // Verify booking was updated
            A.CallTo(() => _bookingRepo.UpdateAsync(A<Booking>.That.Matches(b =>
                b.Status == BookingStatus.Cancelled &&
                b.CancelledAt != null)))
                .MustHaveHappenedOnceExactly();
        }
    }
}
