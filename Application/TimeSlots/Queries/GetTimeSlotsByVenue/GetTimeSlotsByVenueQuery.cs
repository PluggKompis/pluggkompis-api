using Application.TimeSlots.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.TimeSlots.Queries.GetTimeSlotsByVenue
{
    /// <summary>
    /// Query to get all TimeSlots for a specific venue
    /// </summary>
    public class GetTimeSlotsByVenueQuery : IRequest<OperationResult<List<TimeSlotDto>>>
    {
        public Guid VenueId { get; set; }
        public bool IncludeCancelled { get; set; } = false;  // Option to include cancelled slots

        public GetTimeSlotsByVenueQuery(Guid venueId, bool includeCancelled = false)
        {
            VenueId = venueId;
            IncludeCancelled = includeCancelled;
        }
    }
}
