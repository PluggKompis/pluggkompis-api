using Domain.Models.Common;
using MediatR;

namespace Application.TimeSlots.Commands.DeleteTimeSlot
{
    // <summary>
    /// Command to delete a TimeSlot
    /// </summary>
    public class DeleteTimeSlotCommand : IRequest<OperationResult>
    {
        public Guid TimeSlotId { get; set; }
        public Guid CoordinatorId { get; set; }

        public DeleteTimeSlotCommand(Guid timeSlotId, Guid coordinatorId)
        {
            TimeSlotId = timeSlotId;
            CoordinatorId = coordinatorId;
        }
    }
}
