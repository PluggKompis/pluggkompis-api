using Application.Venues.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Commands.CreateVenue
{
    /// <summary>
    /// Command to create a new venue
    /// </summary>
    public class CreateVenueCommand : IRequest<OperationResult<VenueDto>>
    {
        public Guid CoordinatorId { get; }
        public CreateVenueRequest Request { get; }
        public CreateVenueCommand(Guid coordinatorId, CreateVenueRequest request)
        {
            CoordinatorId = coordinatorId;
            Request = request;
        }
    }
}
