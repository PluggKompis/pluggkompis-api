using Application.Common.Interfaces;
using Application.Venues.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Venues.Commands.UpdateVenue
{
    /// <summary>
    /// Handles venue updates. Only the owning coordinator can update their venue.
    /// </summary>
    public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueCommand, OperationResult<VenueDto>>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;

        public UpdateVenueCommandHandler(IVenueRepository venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<VenueDto>> Handle(UpdateVenueCommand command, CancellationToken cancellationToken)
        {
            var venue = await _venueRepository.GetByIdAsync(command.VenueId);

            if (venue == null)
                return OperationResult<VenueDto>.Failure("Venue not found.");

            if (venue.CoordinatorId != command.CoordinatorId)
                return OperationResult<VenueDto>.Failure("You can only update your own venue.");

            // Map request onto existing entity
            _mapper.Map(command.Request, venue);

            await _venueRepository.UpdateAsync(venue);

            var dto = _mapper.Map<VenueDto>(venue);
            return OperationResult<VenueDto>.Success(dto);
        }
    }
}
