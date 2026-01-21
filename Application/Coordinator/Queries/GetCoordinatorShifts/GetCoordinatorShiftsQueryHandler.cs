using Application.Common.Interfaces;
using Application.Coordinator.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Queries.GetCoordinatorShifts
{
    public class GetCoordinatorShiftsQueryHandler
        : IRequestHandler<GetCoordinatorShiftsQuery, OperationResult<PagedResult<CoordinatorShiftDto>>>
    {
        private readonly IVolunteerShiftRepository _volunteerShiftRepository;
        private readonly IMapper _mapper;

        public GetCoordinatorShiftsQueryHandler(
            IVolunteerShiftRepository volunteerShiftRepository,
            IMapper mapper)
        {
            _volunteerShiftRepository = volunteerShiftRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<PagedResult<CoordinatorShiftDto>>> Handle(
            GetCoordinatorShiftsQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _volunteerShiftRepository.GetForCoordinatorAsync(
                coordinatorId: request.CoordinatorId,
                startUtc: request.StartUtc,
                endUtcExclusive: request.EndUtcExclusive,
                isAttended: request.IsAttended,
                venueId: request.VenueId,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

            var dtoItems = _mapper.Map<List<CoordinatorShiftDto>>(items);

            var paged = new PagedResult<CoordinatorShiftDto>
            {
                Items = dtoItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return OperationResult<PagedResult<CoordinatorShiftDto>>.Success(paged);
        }
    }
}
