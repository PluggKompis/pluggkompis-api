using Application.Common.Interfaces;
using Application.VolunteerShifts.Dtos;
using Application.VolunteerShifts.Helpers;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.VolunteerShifts.Queries.GetVolunteerUpcomingShifts
{
    public class GetVolunteerUpcomingShiftsQueryHandler
        : IRequestHandler<GetVolunteerUpcomingShiftsQuery, OperationResult<List<VolunteerShiftDto>>>
    {
        private readonly IVolunteerShiftRepository _shiftRepo;
        private readonly IMapper _mapper;

        public GetVolunteerUpcomingShiftsQueryHandler(IVolunteerShiftRepository shiftRepo, IMapper mapper)
        {
            _shiftRepo = shiftRepo;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<VolunteerShiftDto>>> Handle(
            GetVolunteerUpcomingShiftsQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var shifts = await _shiftRepo.GetUpcomingByVolunteerIdAsync(request.VolunteerId, now);

            var dtos = shifts
                .Select(s =>
                {
                    var dto = _mapper.Map<VolunteerShiftDto>(s);

                    var (startUtc, endUtc) = TimeSlotOccurrenceHelper.GetNextOccurrenceUtc(s.TimeSlot, now);
                    dto.NextOccurrenceStartUtc = startUtc;
                    dto.NextOccurrenceEndUtc = endUtc;

                    return dto;
                })
                .OrderBy(x => x.NextOccurrenceStartUtc)
                .ToList();

            return OperationResult<List<VolunteerShiftDto>>.Success(dtos);
        }
    }
}
