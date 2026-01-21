using Application.Common.Interfaces;
using Application.Venues.Dtos;
using Application.Venues.Queries.GetVenueVolunteers;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetVenueById
{
    /// <summary>
    /// Handles fetching detailed venue information including timeslots and volunteers
    /// </summary>
    public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, OperationResult<VenueDetailDto>>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetVenueByIdQueryHandler(IVenueRepository venueRepository, IMapper mapper, IMediator mediator)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<OperationResult<VenueDetailDto>> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
        {
            var venue = await _venueRepository.GetByIdWithDetailsAsync(request.VenueId);

            if (venue == null)
                return OperationResult<VenueDetailDto>.Failure("Venue not found.");

            var dto = _mapper.Map<VenueDetailDto>(venue);

            var volunteersResult = await _mediator.Send(
                new GetVenueVolunteersQuery(request.VenueId),
                cancellationToken);

            if (!volunteersResult.IsSuccess)
                return OperationResult<VenueDetailDto>.Failure(
                    volunteersResult.Errors.ToArray());

            dto.Volunteers = volunteersResult.Data ?? new();

            return OperationResult<VenueDetailDto>.Success(dto);
        }
    }
}
