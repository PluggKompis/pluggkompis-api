using Application.Bookings.Commands.CreateBooking;
using Application.Bookings.Dtos;
using Application.Common.Interfaces;
using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Children;
using Domain.Models.Entities.Users;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;
using FakeItEasy;
using System.Linq.Expressions;

namespace Test.UnitTests.Bookings
{
    [TestFixture]
    public class CreateBookingCommandHandlerTests
    {
        private IGenericRepository<Booking> _bookingRepo = null!;
        private IGenericRepository<User> _userRepo = null!;
        private ITimeSlotRepository _timeSlotRepo = null!;
        private CreateBookingCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _bookingRepo = A.Fake<IGenericRepository<Booking>>();
            _userRepo = A.Fake<IGenericRepository<User>>();
            _timeSlotRepo = A.Fake<ITimeSlotRepository>();

            _handler = new CreateBookingCommandHandler(
                _bookingRepo,
                _userRepo,
                _timeSlotRepo
            );
        }

        [Test]
        public async Task Handle_WhenTimeSlotFull_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var timeSlotId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Role = UserRole.Student,
                Email = "student@test.com",
                FirstName = "Test",
                LastName = "Student"
            };

            var timeSlot = new TimeSlot
            {
                Id = timeSlotId,
                MaxStudents = 5,
                Venue = new Venue { Name = "Test Venue" },
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18)
            };

            // Mock 5 existing bookings (full)
            var existingBookings = new List<Booking>
            {
                new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed },
                new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed },
                new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed },
                new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed },
                new Booking { Id = Guid.NewGuid(), Status = BookingStatus.Confirmed },
            };

            A.CallTo(() => _userRepo.FindWithIncludesAsync(
                A<Expression<Func<User, bool>>>._,
                A<Expression<Func<User, object>>[]>._))
                .Returns(new[] { user });

            A.CallTo(() => _timeSlotRepo.GetByIdAsync(timeSlotId))
                .Returns(timeSlot);

            A.CallTo(() => _bookingRepo.FindAsync(A<Expression<Func<Booking, bool>>>._))
                .Returns(existingBookings);

            var request = new CreateBookingRequest
            {
                TimeSlotId = timeSlotId,
                BookingDate = DateTime.UtcNow.AddDays(1)
            };

            var command = new CreateBookingCommand(userId, request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors.First().ToLower(), Does.Contain("full"));
        }

        [Test]
        public async Task Handle_WhenParentDoesNotProvideChildId_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var timeSlotId = Guid.NewGuid();

            var parent = new User
            {
                Id = userId,
                Role = UserRole.Parent,
                Email = "parent@test.com",
                FirstName = "Test",
                LastName = "Parent",
                Children = new List<Child>()
            };

            var timeSlot = new TimeSlot 
            {
                Id = timeSlotId,
                MaxStudents = 10,
                Venue = new Venue { Name = "Test Venue" },
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18)
            };

            A.CallTo(() => _userRepo.FindWithIncludesAsync(
                A<Expression<Func<User, bool>>>._,
                A<Expression<Func<User, object>>[]>._))
                .Returns(new[] { parent });

            A.CallTo(() => _timeSlotRepo.GetByIdAsync(timeSlotId)) 
                .Returns(timeSlot);

            A.CallTo(() => _bookingRepo.FindAsync(A<Expression<Func<Booking, bool>>>._)) 
                .Returns(new List<Booking>()); // No existing bookings

            var request = new CreateBookingRequest
            {
                TimeSlotId = timeSlotId,
                BookingDate = DateTime.UtcNow.AddDays(1),
                ChildId = null // Parent didn't provide childId
            };


            var command = new CreateBookingCommand(userId, request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors.First().ToLower(), Does.Contain("child"));
        }

        [Test]
        public async Task Handle_WhenParentTriesToBookForOtherChild_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var timeSlotId = Guid.NewGuid();
            var ownChildId = Guid.NewGuid();
            var otherChildId = Guid.NewGuid();

            var parent = new User
            {
                Id = userId,
                Role = UserRole.Parent,
                Email = "parent@test.com",
                FirstName = "Test",
                LastName = "Parent",
                Children = new List<Child>
                {
                    new Child { Id = ownChildId, FirstName = "Own Child", ParentId = userId }
                }
            };

            var timeSlot = new TimeSlot 
            {
                Id = timeSlotId,
                MaxStudents = 10,
                Venue = new Venue { Name = "Test Venue" },
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18)
            };

            A.CallTo(() => _userRepo.FindWithIncludesAsync(
                A<Expression<Func<User, bool>>>._,
                A<Expression<Func<User, object>>[]>._))
                .Returns(new[] { parent });

            A.CallTo(() => _timeSlotRepo.GetByIdAsync(timeSlotId)) 
                .Returns(timeSlot);

            A.CallTo(() => _bookingRepo.FindAsync(A<Expression<Func<Booking, bool>>>._))
                .Returns(new List<Booking>()); // No existing bookings

            var request = new CreateBookingRequest
            {
                TimeSlotId = timeSlotId,
                BookingDate = DateTime.UtcNow.AddDays(1),
                ChildId = otherChildId // Trying to book for someone else's child
            };

            var command = new CreateBookingCommand(userId, request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors.First().ToLower(), Does.Contain("your own children"));
        }

        [Test]
        public async Task Handle_WhenValidStudentBooking_ShouldSucceed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var timeSlotId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Role = UserRole.Student,
                Email = "student@test.com",
                FirstName = "Test",
                LastName = "Student"
            };

            var timeSlot = new TimeSlot
            {
                Id = timeSlotId,
                MaxStudents = 10,
                Venue = new Venue { Id = Guid.NewGuid(), Name = "Test Venue" },
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18)
            };

            A.CallTo(() => _userRepo.FindWithIncludesAsync(
                A<Expression<Func<User, bool>>>._,
                A<Expression<Func<User, object>>[]>._))
                .Returns(new[] { user });

            A.CallTo(() => _timeSlotRepo.GetByIdAsync(timeSlotId))
                .Returns(timeSlot);

            A.CallTo(() => _bookingRepo.FindAsync(A<Expression<Func<Booking, bool>>>._))
                .Returns(new List<Booking>()); // No existing bookings

            A.CallTo(() => _bookingRepo.AddAsync(A<Booking>._))
                .Returns(Task.CompletedTask);

            var request = new CreateBookingRequest
            {
                TimeSlotId = timeSlotId,
                BookingDate = DateTime.UtcNow.AddDays(1)
            };

            var command = new CreateBookingCommand(userId, request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.StudentId, Is.EqualTo(userId));
            Assert.That(result.Data.ChildId, Is.Null);
        }
    }
}
