using Application.Common.Interfaces;
using Domain.Models.Common;
using MediatR;

namespace Application.TimeSlots.Commands.DeleteTimeSlot
{
    /// <summary>
    /// Handles TimeSlot deletion with authorization check
    /// </summary>
    public class DeleteTimeSlotCommandHandler : IRequestHandler<DeleteTimeSlotCommand, OperationResult>
    {
        private readonly ITimeSlotRepository _timeSlotRepository;

        public DeleteTimeSlotCommandHandler(ITimeSlotRepository timeSlotRepository)
        {
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<OperationResult> Handle(DeleteTimeSlotCommand command, CancellationToken cancellationToken)
        {
            // Fetch TimeSlot with Venue
            var timeSlot = await _timeSlotRepository.GetByIdAsync(command.TimeSlotId);

            if (timeSlot == null)
                return OperationResult.Failure("TimeSlot not found");

            // Authorization: Only venue coordinator can delete their timeslots
            if (timeSlot.Venue.CoordinatorId != command.CoordinatorId)
                return OperationResult.Failure("You can only delete timeslots for your own venue");

            // Business rule: Cannot delete timeslot with confirmed bookings
            var timeSlotWithBookings = await _timeSlotRepository.GetByIdWithDetailsAsync(command.TimeSlotId);

            if (timeSlotWithBookings!.Bookings.Any(b => b.Status == Domain.Models.Enums.BookingStatus.Confirmed))
                return OperationResult.Failure("Cannot delete timeslot with confirmed bookings. Cancel the timeslot instead.");

            // Delete
            await _timeSlotRepository.DeleteAsync(command.TimeSlotId);

            return OperationResult.Success();
        }
    }
}
