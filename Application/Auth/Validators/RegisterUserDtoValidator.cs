using Application.Auth.Dtos;
using Domain.Models.Enums;
using FluentValidation;

namespace Application.Auth.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

            RuleFor(x => x.Role)
                .IsInEnum()
                .Must(r => r == UserRole.Parent || r == UserRole.Student || r == UserRole.Volunteer)
                .WithMessage("Selected role is not allowed.");
        }
    }
}
