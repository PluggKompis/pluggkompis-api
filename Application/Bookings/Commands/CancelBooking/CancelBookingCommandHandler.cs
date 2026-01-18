using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Bookings;
using Domain.Models.Enums;
using MediatR;

namespace Application.Bookings.Commands.CancelBooking
{
    /// <summary>
    /// Handler for cancelling a booking with 2-hour window validation
    /// </summary>
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, OperationResult>
    {
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly ITimeSlotRepository _timeSlotRepo;

        public CancelBookingCommandHandler(
            IGenericRepository<Booking> bookingRepo,
            ITimeSlotRepository timeSlotRepo)
        {
            _bookingRepo = bookingRepo;
            _timeSlotRepo = timeSlotRepo;
        }

        public async Task<OperationResult> Handle(
            CancelBookingCommand request,
            CancellationToken cancellationToken)
        {
            // Get booking
            var booking = await _bookingRepo.GetByIdAsync(request.BookingId);

            if (booking == null)
            {
                return OperationResult.Failure("Booking not found.");
            }

            // Authorization: Check user owns this booking
            if (booking.BookedByUserId != request.UserId)
            {
                return OperationResult.Failure("You can only cancel your own bookings.");
            }

            // Get timeslot to check cancellation window
            var timeSlot = await _timeSlotRepo.GetByIdAsync(booking.TimeSlotId);

            if (timeSlot == null)
                return OperationResult.Failure("TimeSlot not found.");

            // Validation: Check 2-hour cancellation window
            var sessionStart = booking.BookingDate.Date + timeSlot.StartTime;
            var hoursUntilSession = (sessionStart - DateTime.Now).TotalHours;

            if (hoursUntilSession < 2)
            {
                return OperationResult.Failure("Bookings can only be cancelled at least 2 hours before the session."); // ✅ ADDED SEMICOLON
            }

            // Cancel booking
            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;

            await _bookingRepo.UpdateAsync(booking);

            return OperationResult.Success();
        }
    }
}
