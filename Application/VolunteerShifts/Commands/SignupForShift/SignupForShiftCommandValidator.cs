using FluentValidation;

namespace Application.VolunteerShifts.Commands.SignupForShift
{
    public class SignupForShiftCommandValidator : AbstractValidator<SignupForShiftCommand>
    {
        public SignupForShiftCommandValidator()
        {
            RuleFor(x => x.VolunteerId).NotEmpty();
            RuleFor(x => x.Dto.TimeSlotId).NotEmpty();
            RuleFor(x => x.Dto.Notes).MaximumLength(1000);
        }
    }
}
