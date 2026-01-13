using Application.Venues.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetVenues
{
    /// <summary>
    /// Query to get all venues with optional filtering
    /// </summary>
    public class GetVenuesQuery : IRequest<OperationResult<PagedResult<VenueDto>>>
    {
        public VenueFilterParams Filters { get; set; }
        public GetVenuesQuery(VenueFilterParams filters)
        {
            Filters = filters;
        }
    }
}
