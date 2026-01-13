using Application.Common.Interfaces;
using Domain.Models.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Venues.Commands.DeleteVenue
{
    /// <summary>
    /// Handles venue deletion. Only the owning coordinator can delete their venue.
    /// </summary>
    public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, OperationResult>
    {
        private readonly IVenueRepository _venueRepository;

        public DeleteVenueCommandHandler(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<OperationResult> Handle(DeleteVenueCommand command, CancellationToken cancellationToken)
        {
            var venue = await _venueRepository.GetByIdAsync(command.VenueId);

            if (venue == null)
            {
                return OperationResult.Failure("Venue not found.");
            }

            // Authorization: Verify coordinator owns this venue
            if (venue.CoordinatorId != command.CoordinatorId)
            {
                return OperationResult.Failure("You can only delete your own venue.");
            }

            await _venueRepository.DeleteAsync(command.VenueId);

            return OperationResult.Success();
        }
    }
}
