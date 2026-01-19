using Application.Common.Interfaces;
using Application.VolunteerShifts.Helpers;
using Domain.Models.Common;
using Domain.Models.Enums;
using MediatR;

namespace Application.VolunteerShifts.Commands.CancelVolunteerShift
{
    public class CancelVolunteerShiftCommandHandler
        : IRequestHandler<CancelVolunteerShiftCommand, OperationResult>
    {
        private readonly IVolunteerShiftRepository _shiftRepo;
        private readonly IGenericRepository<Domain.Models.Entities.Volunteers.VolunteerShift> _genericShifts;

        // Cancellation must be made at least this many hours before shift start
        private const int CancelNoticeHours = 2;

        public CancelVolunteerShiftCommandHandler(
            IVolunteerShiftRepository shiftRepo,
            IGenericRepository<Domain.Models.Entities.Volunteers.VolunteerShift> genericShifts)
        {
            _shiftRepo = shiftRepo;
            _genericShifts = genericShifts;
        }

        public async Task<OperationResult> Handle(CancelVolunteerShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepo.GetByIdWithTimeSlotAsync(request.VolunteerShiftId);
            if (shift is null)
                return OperationResult.Failure("Volunteer shift not found");

            if (shift.VolunteerId != request.VolunteerId)
                return OperationResult.Failure("Forbidden: you can only cancel your own shifts");

            if (shift.Status == VolunteerShiftStatus.Cancelled)
                return OperationResult.Success();

            var now = DateTime.UtcNow;
            var (startUtc, _) = TimeSlotOccurrenceHelper.GetNextOccurrenceUtc(shift.TimeSlot, now);

            if (startUtc is null)
                return OperationResult.Failure("Shift start time could not be determined");

            if (now > startUtc.Value.AddHours(-CancelNoticeHours))
                return OperationResult.Failure($"Forbidden: you must cancel at least {CancelNoticeHours} hours before the shift starts");

            shift.Status = VolunteerShiftStatus.Cancelled;
            await _genericShifts.UpdateAsync(shift);

            return OperationResult.Success();
        }
    }
}
