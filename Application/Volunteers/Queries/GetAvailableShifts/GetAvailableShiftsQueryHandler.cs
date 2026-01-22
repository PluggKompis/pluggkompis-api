using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.GetAvailableShifts
{
    public class GetAvailableShiftsQueryHandler
            : IRequestHandler<GetAvailableShiftsQuery, OperationResult<List<AvailableShiftDto>>>
    {
        private readonly IAvailableShiftsRepository _repo;
        private readonly IMapper _mapper;

        public GetAvailableShiftsQueryHandler(IAvailableShiftsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<AvailableShiftDto>>> Handle(
            GetAvailableShiftsQuery request,
            CancellationToken cancellationToken)
        {
            var modelResult = await _repo.GetAvailableShiftsAsync(request.VolunteerId, cancellationToken);

            if (!modelResult.IsSuccess)
                return OperationResult<List<AvailableShiftDto>>.Failure(modelResult.Errors.ToArray());

            var dto = _mapper.Map<List<AvailableShiftDto>>(modelResult.Data!);
            return OperationResult<List<AvailableShiftDto>>.Success(dto);
        }
    }
}
