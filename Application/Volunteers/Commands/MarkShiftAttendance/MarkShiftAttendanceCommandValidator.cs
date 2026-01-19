using FluentValidation;

namespace Application.Volunteers.Commands.MarkShiftAttendance
{
    public class MarkShiftAttendanceCommandValidator : AbstractValidator<MarkShiftAttendanceCommand>
    {
        public MarkShiftAttendanceCommandValidator()
        {
            RuleFor(x => x.CoordinatorId).NotEmpty();
            RuleFor(x => x.ShiftId).NotEmpty();
            RuleFor(x => x.Dto.Notes).MaximumLength(2000);
        }
    }
}
