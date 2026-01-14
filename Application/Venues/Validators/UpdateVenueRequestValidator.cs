using Application.Venues.Dtos;
using FluentValidation;

namespace Application.Venues.Validators
{
    /// <summary>
    /// Validates UpdateVenueRequest with same rules as CreateVenueRequest plus IsActive flag
    /// </summary>
    public class UpdateVenueRequestValidator : AbstractValidator<UpdateVenueRequest>
    {
        public UpdateVenueRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Venue name is required")
                .MaximumLength(200).WithMessage("Venue name cannot exceed 200 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required")
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters")
                .Matches(@"^\d{3}\s?\d{2}$").WithMessage("Invalid Swedish postal code format (e.g., 123 45)");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required")
                .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters")
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Phone can only contain numbers, spaces, and +-() characters");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive status is required");
        }
    }
}
