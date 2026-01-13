using FluentValidation;

namespace Application.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.Dto.RefreshToken)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
