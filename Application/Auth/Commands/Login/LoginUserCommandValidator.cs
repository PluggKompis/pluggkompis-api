using FluentValidation;

namespace Application.Auth.Commands.Login
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().EmailAddress().MaximumLength(256);

            RuleFor(x => x.Dto.Password)
                .NotEmpty().MaximumLength(200);
        }
    }
}
