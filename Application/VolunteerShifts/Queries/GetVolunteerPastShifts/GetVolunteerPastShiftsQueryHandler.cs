using Application.Common.Interfaces;
using Application.VolunteerShifts.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.VolunteerShifts.Queries.GetVolunteerPastShifts
{
    public class GetVolunteerPastShiftsQueryHandler
            : IRequestHandler<GetVolunteerPastShiftsQuery, OperationResult<List<VolunteerShiftDto>>>
    {
        private readonly IVolunteerShiftRepository _shiftRepo;
        private readonly IMapper _mapper;

        public GetVolunteerPastShiftsQueryHandler(
            IVolunteerShiftRepository shiftRepo,
            IMapper mapper)
        {
            _shiftRepo = shiftRepo;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<VolunteerShiftDto>>> Handle(
            GetVolunteerPastShiftsQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var shifts = await _shiftRepo.GetPastByVolunteerIdAsync(request.VolunteerId, now);

            var dtos = shifts
                .Select(s =>
                {
                    var dto = _mapper.Map<VolunteerShiftDto>(s);

                    // Keep same pattern as upcoming handler (even if naming is "NextOccurrence")
                    dto.NextOccurrenceStartUtc = s.OccurrenceStartUtc;
                    dto.NextOccurrenceEndUtc = s.OccurrenceEndUtc;

                    return dto;
                })
                .OrderByDescending(x => x.NextOccurrenceStartUtc)
                .ToList();

            return OperationResult<List<VolunteerShiftDto>>.Success(dtos);
        }
    }
}
