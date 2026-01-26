using Application.TimeSlots.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.TimeSlots.Commands.CreateTimeSlot
{
    /// <summary>
    /// Command to create a new TimeSlot for a venue
    /// </summary>
    public class CreateTimeSlotCommand : IRequest<OperationResult<TimeSlotDto>>
    {
        public Guid CoordinatorId { get; set; } // From JWT token
        public CreateTimeSlotRequest Request { get; set; }

        public CreateTimeSlotCommand(Guid coordinatorId, CreateTimeSlotRequest request)
        {
            CoordinatorId = coordinatorId;
            Request = request;
        }
    }
}
