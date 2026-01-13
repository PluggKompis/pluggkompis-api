using FluentValidation;

namespace Application.Auth.Commands.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Dto.FirstName)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.Dto.LastName)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.Dto.Email)
                .NotEmpty().EmailAddress().MaximumLength(256);

            RuleFor(x => x.Dto.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(200);

            RuleFor(x => x.Dto.Role)
                .IsInEnum();
        }
    }
}
