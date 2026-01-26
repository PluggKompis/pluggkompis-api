using FluentValidation;

namespace Application.Volunteers.Commands.ApplyToVenue
{
    public class ApplyToVenueCommandValidator : AbstractValidator<ApplyToVenueCommand>
    {
        public ApplyToVenueCommandValidator()
        {
            RuleFor(x => x.VolunteerId).NotEmpty();
            RuleFor(x => x.Dto.VenueId).NotEmpty();
        }
    }
}
