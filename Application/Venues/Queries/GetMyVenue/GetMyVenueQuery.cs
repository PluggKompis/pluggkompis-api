using Application.Venues.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetMyVenue
{
    /// <summary>
    /// Query to get the coordinator's own venue
    /// </summary>
    public class GetMyVenueQuery : IRequest<OperationResult<VenueDto>>
    {
        public Guid CoordinatorId { get; set; }
        public GetMyVenueQuery(Guid coordinatorId)
        {
            CoordinatorId = coordinatorId;
        }
    }
}
