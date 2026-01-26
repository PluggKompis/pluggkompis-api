using FluentValidation;

namespace Application.VolunteerShifts.Commands.CancelVolunteerShift
{
    public class CancelVolunteerShiftCommandValidator : AbstractValidator<CancelVolunteerShiftCommand>
    {
        public CancelVolunteerShiftCommandValidator()
        {
            RuleFor(x => x.VolunteerId).NotEmpty();
            RuleFor(x => x.VolunteerShiftId).NotEmpty();
        }
    }
}
