using Application.Venues.Dtos;
using FluentValidation;

namespace Application.Venues.Validators
{
    /// <summary>
    /// Validates pagination and filter parameters for venue search
    /// </summary>
    public class VenueFilterParamsValidator : AbstractValidator<VenueFilterParams>
    {
        public VenueFilterParamsValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

            // City is optional, but if provided, should not be empty
            When(x => x.City != null, () =>
            {
                RuleFor(x => x.City)
                    .NotEmpty().WithMessage("City filter cannot be empty string");
            });
        }
    }
}
