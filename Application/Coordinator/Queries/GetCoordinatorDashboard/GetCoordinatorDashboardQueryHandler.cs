using Application.Common.Interfaces;
using Application.Coordinator.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Queries.GetCoordinatorDashboard
{
    public class GetCoordinatorDashboardQueryHandler
        : IRequestHandler<GetCoordinatorDashboardQuery, OperationResult<CoordinatorDashboardDto>>
    {
        private readonly ICoordinatorDashboardRepository _repo;
        private readonly IMapper _mapper;

        public GetCoordinatorDashboardQueryHandler(
            ICoordinatorDashboardRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OperationResult<CoordinatorDashboardDto>> Handle(
            GetCoordinatorDashboardQuery request,
            CancellationToken cancellationToken)
        {
            // Repo also enforces: "only their venue"
            var dashboard = await _repo.BuildCoordinatorDashboardAsync(request.CoordinatorId, cancellationToken);

            if (!dashboard.IsSuccess)
                return OperationResult<CoordinatorDashboardDto>.Failure(dashboard.Errors.ToArray());

            var dto = _mapper.Map<CoordinatorDashboardDto>(dashboard.Data!);
            return OperationResult<CoordinatorDashboardDto>.Success(dto);
        }
    }
}
