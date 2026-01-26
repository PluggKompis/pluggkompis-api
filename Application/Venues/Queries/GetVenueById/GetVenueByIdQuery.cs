using Application.Venues.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetVenueById
{
    /// <summary>
    /// Query to get detailed venue information by ID
    /// </summary>
    public class GetVenueByIdQuery : IRequest<OperationResult<VenueDetailDto>>
    {
        public Guid VenueId { get; set; }
        public GetVenueByIdQuery(Guid venueId)
        {
            VenueId = venueId;
        }
    }
}
