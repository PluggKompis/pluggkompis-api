using Application.Common.Interfaces;
using Application.Venues.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Venues;
using MediatR;

namespace Application.Venues.Commands.CreateVenue
{
    /// <summary>
    /// Handles venue creation for coordinators.
    /// Enforces one-venue-per-coordinator rule.
    /// </summary>
    public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, OperationResult<VenueDto>>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;
        public CreateVenueCommandHandler(IVenueRepository venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
        }
        public async Task<OperationResult<VenueDto>> Handle(CreateVenueCommand command, CancellationToken cancellationToken)
        {
            var existingVenue = await _venueRepository.GetByCoordinatorIdAsync(command.CoordinatorId);
            if (existingVenue != null)
                return OperationResult<VenueDto>.Failure("You already have a venue.");

            var venue = _mapper.Map<Venue>(command.Request);

            venue.Id = Guid.NewGuid();
            venue.CoordinatorId = command.CoordinatorId;
            venue.IsActive = true;

            var created = await _venueRepository.CreateAsync(venue);

            var dto = _mapper.Map<VenueDto>(created);
            return OperationResult<VenueDto>.Success(dto);
        }
    }
}
