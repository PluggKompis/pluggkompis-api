using Application.TimeSlots.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.TimeSlots.Commands.UpdateTimeSlot
{
    /// <summary>
    /// Command to update an existing TimeSlot
    /// </summary>
    public class UpdateTimeSlotCommand : IRequest<OperationResult<TimeSlotDto>>
    {
        public Guid TimeSlotId { get; set; }
        public Guid CoordinatorId { get; set; } // From JWT token
        public UpdateTimeSlotRequest Request { get; set; }

        public UpdateTimeSlotCommand(Guid timeSlotId, Guid coordinatorId, UpdateTimeSlotRequest request)
        {
            TimeSlotId = timeSlotId;
            CoordinatorId = coordinatorId;
            Request = request;
        }
    }
}
