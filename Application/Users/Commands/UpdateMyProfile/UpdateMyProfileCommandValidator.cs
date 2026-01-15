using FluentValidation;

namespace Application.Users.Commands.UpdateMyProfile
{
    public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
    {
        public UpdateMyProfileCommandValidator()
        {
            RuleFor(x => x.Dto)
                .Must(dto =>
                    dto.FirstName is not null ||
                    dto.LastName is not null ||
                    dto.Email is not null)
                .WithMessage("At least one field must be provided for update.");

            When(x => x.Dto.FirstName is not null, () =>
            {
                RuleFor(x => x.Dto.FirstName!)
                    .NotEmpty()
                    .MaximumLength(100);
            });

            When(x => x.Dto.LastName is not null, () =>
            {
                RuleFor(x => x.Dto.LastName!)
                    .NotEmpty()
                    .MaximumLength(100);
            });

            When(x => x.Dto.Email is not null, () =>
            {
                RuleFor(x => x.Dto.Email!)
                    .NotEmpty()
                    .EmailAddress()
                    .MaximumLength(256);
            });
        }
    }
}
