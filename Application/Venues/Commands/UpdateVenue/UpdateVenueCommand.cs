using Application.Venues.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Commands.UpdateVenue
{
    /// <summary>
    /// Command to update an existing venue
    /// </summary>
    public class UpdateVenueCommand : IRequest<OperationResult<VenueDto>>
    {
        public Guid VenueId { get; set; }
        public Guid CoordinatorId { get; set; }
        public UpdateVenueRequest Request { get; set; }

        public UpdateVenueCommand(Guid venueId, Guid coordinatorId, UpdateVenueRequest request)
        {
            VenueId = venueId;
            CoordinatorId = coordinatorId;
            Request = request;
        }
    }
}
