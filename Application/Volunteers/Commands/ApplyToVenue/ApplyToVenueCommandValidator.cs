using FluentValidation;

namespace Application.Volunteers.Commands.ApplyToVenue
{
    public class ApplyToVenueCommandValidator : AbstractValidator<ApplyToVenueCommand>
    {
        public ApplyToVenueCommandValidator()
        {
            RuleFor(x => x.VolunteerId).NotEmpty();

            RuleFor(x => x.Dto.VenueId).NotEmpty();

            RuleFor(x => x.Dto.Bio)
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(x => x.Dto.Experience)
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(x => x.Dto.Subjects)
                .NotNull()
                .Must(s => s.Count > 0)
                .WithMessage("At least one subject must be provided.");

            RuleForEach(x => x.Dto.Subjects).ChildRules(subject =>
            {
                subject.RuleFor(s => s.SubjectId).NotEmpty();
                subject.RuleFor(s => s.ConfidenceLevel).IsInEnum();
            });
        }
    }
}
