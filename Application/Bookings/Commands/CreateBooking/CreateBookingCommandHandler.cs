using Application.Bookings.Dtos;
using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;
using MediatR;

namespace Application.Bookings.Commands.CreateBooking
{
    /// <summary>
    /// Handler for creating a new booking with validation and authorization
    /// </summary>
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, OperationResult<BookingDto>>
    {
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly ITimeSlotRepository _timeSlotRepo;

        public CreateBookingCommandHandler(IGenericRepository<Booking> bookingRepo, IGenericRepository<User> userRepo, ITimeSlotRepository timeSlotRepo)
        {
            _bookingRepo = bookingRepo;
            _userRepo = userRepo;
            _timeSlotRepo = timeSlotRepo;
        }

        public async Task<OperationResult<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // Get user with children in one query
            var users = await _userRepo.FindWithIncludesAsync(
                u => u.Id == request.UserId,
                u => u.Children  // Include children
            );
            var user = users.FirstOrDefault();

            if (user == null)
                return OperationResult<BookingDto>.Failure("User not found");

            // Get timeslot with venue
            var timeSlot = await _timeSlotRepo.GetByIdAsync(request.Request.TimeSlotId);

            if (timeSlot == null)
                return OperationResult<BookingDto>.Failure("TimeSlot not found");

            // Validation 1: Check if timeslot is full
            var existingBookings = await _bookingRepo.FindAsync(b =>
                b.TimeSlotId == request.Request.TimeSlotId &&
                b.BookingDate.Date == request.Request.BookingDate.Date &&
                b.Status == BookingStatus.Confirmed);

            if (existingBookings.Count() >= timeSlot.MaxStudents)
                return OperationResult<BookingDto>.Failure("This timeslot is full");

            // Determine if booking for child or self
            Guid? childId = null;
            Guid? studentId = null;
            string? childName = null;

            if (user.Role == UserRole.Parent)
            {
                // Validation 2: Parent MUST provide childId
                if (!request.Request.ChildId.HasValue)
                    return OperationResult<BookingDto>.Failure("Parent must specify which child to book for");

                // Validation 3: Verify parent owns this child
                var child = user.Children?.FirstOrDefault(c => c.Id == request.Request.ChildId.Value);
                if (child == null)
                    return OperationResult<BookingDto>.Failure("You can only book for your own children");

                childId = child.Id;
                childName = child.FirstName;
            }
            else if (user.Role == UserRole.Student)
            {
                studentId = user.Id;
            }
            else
            {
                return OperationResult<BookingDto>.Failure("Only parents and students can create bookings");
            }

            // Validation 4: Check for duplicate booking
            var duplicateBookings = await _bookingRepo.FindAsync(b =>
                b.TimeSlotId == request.Request.TimeSlotId &&
                b.BookingDate.Date == request.Request.BookingDate.Date &&
                ((b.ChildId.HasValue && b.ChildId == childId) ||
                 (b.StudentId.HasValue && b.StudentId == studentId)) &&
                b.Status == BookingStatus.Confirmed);

            if (duplicateBookings.Any())
                return OperationResult<BookingDto>.Failure("This person is already booked for this timeslot");

            // Create booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                TimeSlotId = request.Request.TimeSlotId,
                StudentId = studentId,
                ChildId = childId,
                BookedByUserId = user.Id,
                BookingDate = request.Request.BookingDate.Date,
                BookedAt = DateTime.UtcNow,
                Status = BookingStatus.Confirmed,
                Notes = request.Request.Notes
            };

            await _bookingRepo.AddAsync(booking);

            // Build response DTO
            var dto = new BookingDto
            {
                Id = booking.Id,
                TimeSlotId = booking.TimeSlotId,
                StudentId = booking.StudentId,
                ChildId = booking.ChildId,
                BookedByUserId = booking.BookedByUserId,
                BookingDate = booking.BookingDate,
                BookedAt = booking.BookedAt,
                Status = booking.Status,
                Notes = booking.Notes,
                VenueName = timeSlot.Venue?.Name ?? "Unknown Venue",
                VenueAddress = timeSlot.Venue?.Address,
                VenueCity = timeSlot.Venue?.City,
                TimeSlotTime = $"{timeSlot.StartTime:hh\\:mm} - {timeSlot.EndTime:hh\\:mm}",
                ChildName = childName
            };

            return OperationResult<BookingDto>.Success(dto);
        }
    }
}
