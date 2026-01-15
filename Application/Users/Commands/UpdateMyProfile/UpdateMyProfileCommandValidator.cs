using FluentValidation;

namespace Application.Users.Commands.UpdateMyProfile
{
    public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
    {
        public UpdateMyProfileCommandValidator()
        {
            RuleFor(x => x.Dto.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Dto.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Dto.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);
        }
    }
}
