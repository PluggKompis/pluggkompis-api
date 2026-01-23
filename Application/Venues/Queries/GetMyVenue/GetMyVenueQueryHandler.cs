using Application.Common.Interfaces;
using Application.Venues.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetMyVenue
{
    /// <summary>
    /// Handles fetching the coordinator's venue
    /// </summary>
    public class GetMyVenueQueryHandler : IRequestHandler<GetMyVenueQuery, OperationResult<VenueDto>>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;

        public GetMyVenueQueryHandler(IVenueRepository venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<VenueDto>> Handle(GetMyVenueQuery request, CancellationToken cancellationToken)
        {
            var venue = await _venueRepository.GetByCoordinatorIdAsync(request.CoordinatorId);

            if (venue is null)
            {
                // Treat "no venue yet" as a valid state => 200 OK with data = null
                return OperationResult<VenueDto>.Success(null!);
            }

            var dto = _mapper.Map<VenueDto>(venue);
            return OperationResult<VenueDto>.Success(dto);
        }
    }
}
