using FluentValidation;

namespace Application.Users.Commands.ChangeMyPassword
{
    public class ChangeMyPasswordCommandValidator : AbstractValidator<ChangeMyPasswordCommand>
    {
        public ChangeMyPasswordCommandValidator()
        {
            RuleFor(x => x.Dto.CurrentPassword)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(x => x.Dto.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(200);
        }
    }
}
