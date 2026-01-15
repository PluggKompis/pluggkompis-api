using Application.Common.Mappings;
using Application.TimeSlots.Dtos;
using AutoMapper;
using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Test.UnitTests
{
    [TestFixture]
    public class TimeSlotTests
    {
        private IMapper _mapper = null!;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TimeSlotMappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void TimeSlotDto_CalculatesAvailableSpots_WithNoBookings()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                VenueId = Guid.NewGuid(),
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18),
                MaxStudents = 20,
                Status = TimeSlotStatus.Open,
                Bookings = new List<Booking>()  // No bookings
            };

            // Act
            var dto = _mapper.Map<TimeSlotDto>(timeSlot);

            // Assert
            Assert.That(dto.AvailableSpots, Is.EqualTo(20));
            Assert.That(dto.CurrentBookings, Is.EqualTo(0));
        }

        [Test]
        public void TimeSlotDto_CalculatesAvailableSpots_WithConfirmedBookings()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                VenueId = Guid.NewGuid(),
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18),
                MaxStudents = 20,
                Status = TimeSlotStatus.Open,
                Bookings = new List<Booking>
                {
                    new Booking { Status = BookingStatus.Confirmed },
                    new Booking { Status = BookingStatus.Confirmed },
                    new Booking { Status = BookingStatus.Confirmed },
                    new Booking { Status = BookingStatus.Cancelled },  // Don't count cancelled
                }
            };

            // Act
            var dto = _mapper.Map<TimeSlotDto>(timeSlot);

            // Assert
            Assert.That(dto.CurrentBookings, Is.EqualTo(3));
            Assert.That(dto.AvailableSpots, Is.EqualTo(17));  // 20 - 3
        }

        [Test]
        public void TimeSlotDto_CalculatesAvailableSpots_WhenFull()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                VenueId = Guid.NewGuid(),
                DayOfWeek = WeekDay.Monday,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(18),
                MaxStudents = 5,
                Status = TimeSlotStatus.Full,
                Bookings = new List<Booking>
                {
                    new Booking { Status = BookingStatus.Confirmed },
                    new Booking { Status = BookingStatus.Confirmed },
                    new Booking { Status = BookingStatus.Confirmed },
                    new Booking { Status = BookingStatus.Confirmed },
                    new Booking { Status = BookingStatus.Confirmed },
                }
            };

            // Act
            var dto = _mapper.Map<TimeSlotDto>(timeSlot);

            // Assert
            Assert.That(dto.CurrentBookings, Is.EqualTo(5));
            Assert.That(dto.AvailableSpots, Is.EqualTo(0));
        }
    }
}
