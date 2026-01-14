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

            if (venue == null)
            {
                return OperationResult<VenueDto>.Failure("You do nothave a venue yet.");
            }

            var dto = _mapper.Map<VenueDto>(venue);
            return OperationResult<VenueDto>.Success(dto);
        }
    }
}
