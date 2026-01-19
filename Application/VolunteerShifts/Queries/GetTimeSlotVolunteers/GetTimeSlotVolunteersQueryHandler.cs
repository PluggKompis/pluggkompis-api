using Application.Common.Interfaces;
using Application.VolunteerShifts.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;

namespace Application.VolunteerShifts.Queries.GetTimeSlotVolunteers
{
    public class GetTimeSlotVolunteersQueryHandler
        : IRequestHandler<GetTimeSlotVolunteersQuery, OperationResult<List<VolunteerShiftDto>>>
    {
        private readonly IVolunteerShiftRepository _shiftRepo;
        private readonly IMapper _mapper;

        public GetTimeSlotVolunteersQueryHandler(IVolunteerShiftRepository shiftRepo, IMapper mapper)
        {
            _shiftRepo = shiftRepo;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<VolunteerShiftDto>>> Handle(
            GetTimeSlotVolunteersQuery request,
            CancellationToken cancellationToken)
        {
            var shifts = await _shiftRepo.GetByTimeSlotIdAsync(request.TimeSlotId);

            // “Assigned” usually means not cancelled (and often Confirmed only).
            var dtos = shifts
                .Where(s => s.Status != VolunteerShiftStatus.Cancelled)
                .Select(s => _mapper.Map<VolunteerShiftDto>(s))
                .ToList();

            return OperationResult<List<VolunteerShiftDto>>.Success(dtos);
        }
    }
}
