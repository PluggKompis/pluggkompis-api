using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Commands.DeleteVenue
{
    /// <summary>
    /// Command to delete a venue
    /// </summary>
    public class DeleteVenueCommand : IRequest<OperationResult>
    {
        public Guid VenueId { get; set; }
        public Guid CoordinatorId { get; set; }
        public DeleteVenueCommand(Guid venueId, Guid coordinatorId)
        {
            VenueId = venueId;
            CoordinatorId = coordinatorId;
        }
    }
}
