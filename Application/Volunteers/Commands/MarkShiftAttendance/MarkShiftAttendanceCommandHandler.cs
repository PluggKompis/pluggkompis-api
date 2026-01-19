using Application.Common.Interfaces;
using Application.VolunteerShifts.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Volunteers.Commands.MarkShiftAttendance
{
    public class MarkShiftAttendanceCommandHandler
        : IRequestHandler<MarkShiftAttendanceCommand, OperationResult<VolunteerShiftDto>>
    {
        private readonly IVolunteerShiftRepository _shiftRepo;
        private readonly IGenericRepository<Domain.Models.Entities.Volunteers.VolunteerShift> _genericShifts;
        private readonly IMapper _mapper;

        public MarkShiftAttendanceCommandHandler(
            IVolunteerShiftRepository shiftRepo,
            IGenericRepository<Domain.Models.Entities.Volunteers.VolunteerShift> genericShifts,
            IMapper mapper)
        {
            _shiftRepo = shiftRepo;
            _genericShifts = genericShifts;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerShiftDto>> Handle(
            MarkShiftAttendanceCommand request,
            CancellationToken cancellationToken)
        {
            var shift = await _shiftRepo.GetByIdWithTimeSlotVenueAsync(request.ShiftId);
            if (shift is null)
                return OperationResult<VolunteerShiftDto>.Failure("Volunteer shift not found");

            // Only the venue coordinator can mark attendance
            var venueCoordinatorId = shift.TimeSlot.Venue.CoordinatorId;
            if (venueCoordinatorId != request.CoordinatorId)
                return OperationResult<VolunteerShiftDto>.Failure("Forbidden: you are not the coordinator for this venue");

            // Don't allow attendance on cancelled shifts
            if (shift.Status == VolunteerShiftStatus.Cancelled)
                return OperationResult<VolunteerShiftDto>.Failure("Cannot mark attendance for a cancelled shift");

            shift.IsAttended = request.Dto.IsAttended;

            // Notes optional (replace if provided; leave as-is if null)
            if (request.Dto.Notes is not null)
                shift.Notes = request.Dto.Notes;

            // Ticket requirement
            shift.Status = VolunteerShiftStatus.Completed;

            await _genericShifts.UpdateAsync(shift);

            var dto = _mapper.Map<VolunteerShiftDto>(shift);

            // Calculate duration from TimeSlot (ticket requirement)
            var duration = shift.TimeSlot.EndTime - shift.TimeSlot.StartTime;
            dto.DurationHours = duration.TotalHours;

            return OperationResult<VolunteerShiftDto>.Success(dto);
        }
    }
}
