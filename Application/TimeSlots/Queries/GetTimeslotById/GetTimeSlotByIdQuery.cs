using Application.TimeSlots.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.TimeSlots.Queries.GetTimeslotById
{
    /// <summary>
    /// Query to get a specific TimeSlot by ID
    /// </summary>
    public class GetTimeSlotByIdQuery : IRequest<OperationResult<TimeSlotDto>>
    {
        public Guid TimeSlotId { get; set; }

        public GetTimeSlotByIdQuery(Guid timeSlotId)
        {
            TimeSlotId = timeSlotId;
        }
    }
}
