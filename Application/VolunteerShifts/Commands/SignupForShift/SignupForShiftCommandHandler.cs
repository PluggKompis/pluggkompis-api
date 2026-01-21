using Application.Common.Interfaces;
using Application.VolunteerShifts.Dtos;
using Application.VolunteerShifts.Helpers;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Venues;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using MediatR;

namespace Application.VolunteerShifts.Commands.SignupForShift
{

    public class SignupForShiftCommandHandler
        : IRequestHandler<SignupForShiftCommand, OperationResult<VolunteerShiftDto>>
    {
        private readonly IGenericRepository<TimeSlot> _timeSlots;
        private readonly IGenericRepository<VolunteerApplication> _applications;
        private readonly IGenericRepository<VolunteerShift> _genericShifts;
        private readonly IVolunteerShiftRepository _shiftRepo;
        private readonly IMapper _mapper;

        public SignupForShiftCommandHandler(
            IGenericRepository<TimeSlot> timeSlots,
            IGenericRepository<VolunteerApplication> applications,
            IGenericRepository<VolunteerShift> genericShifts,
            IVolunteerShiftRepository shiftRepo,
            IMapper mapper)
        {
            _timeSlots = timeSlots;
            _applications = applications;
            _genericShifts = genericShifts;
            _shiftRepo = shiftRepo;
            _mapper = mapper;
        }

        public async Task<OperationResult<VolunteerShiftDto>> Handle(
            SignupForShiftCommand request,
            CancellationToken cancellationToken)
        {
            var timeSlot = await _timeSlots.GetByIdAsync(request.Dto.TimeSlotId);
            if (timeSlot is null)
                return OperationResult<VolunteerShiftDto>.Failure("TimeSlot not found");

            // Approval check: VolunteerApplication must be Approved for the Venue
            var approvals = await _applications.FindAsync(a =>
                a.VolunteerId == request.VolunteerId &&
                a.VenueId == timeSlot.VenueId &&
                a.Status == VolunteerApplicationStatus.Approved);

            if (!approvals.Any())
                return OperationResult<VolunteerShiftDto>.Failure("Forbidden: volunteer is not approved for this venue");

            // Prevent duplicates (friendly check)
            var existing = await _shiftRepo.GetByVolunteerAndTimeSlotAsync(request.VolunteerId, timeSlot.Id);

            if (existing is not null)
            {
                if (existing.Status != VolunteerShiftStatus.Cancelled)
                    return OperationResult<VolunteerShiftDto>.Failure("You are already signed up for this shift");

                // Re-activate cancelled shift
                existing.Status = VolunteerShiftStatus.Confirmed;
                existing.Notes = request.Dto.Notes ?? existing.Notes;
                await _genericShifts.UpdateAsync(existing);

                var existingDto = _mapper.Map<VolunteerShiftDto>(existing);
                return OperationResult<VolunteerShiftDto>.Success(existingDto);
            }

            var (startUtc, endUtc) = TimeSlotOccurrenceHelper.GetNextOccurrenceUtc(timeSlot, DateTime.UtcNow);

            if (startUtc is null || endUtc is null)
                return OperationResult<VolunteerShiftDto>.Failure("This timeslot has no upcoming occurrence.");

            var shift = new VolunteerShift
            {
                Id = Guid.NewGuid(),
                VolunteerId = request.VolunteerId,
                TimeSlotId = timeSlot.Id,
                Status = VolunteerShiftStatus.Confirmed,
                Notes = request.Dto.Notes,
                IsAttended = false,
                OccurrenceStartUtc = startUtc.Value,
                OccurrenceEndUtc = endUtc.Value
            };

            await _genericShifts.AddAsync(shift);

            // Re-load with includes for DTO mapping
            var created = await _shiftRepo.GetByIdWithTimeSlotAsync(shift.Id);
            if (created is null)
                return OperationResult<VolunteerShiftDto>.Failure("An unexpected error occurred while creating the shift");

            var dto = _mapper.Map<VolunteerShiftDto>(created);
            return OperationResult<VolunteerShiftDto>.Success(dto);
        }
    }
}
