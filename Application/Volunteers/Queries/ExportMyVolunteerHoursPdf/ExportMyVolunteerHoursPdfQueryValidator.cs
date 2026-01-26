using FluentValidation;

namespace Application.Volunteers.Queries.ExportMyVolunteerHoursPdf
{
    public class ExportMyVolunteerHoursPdfQueryValidator : AbstractValidator<ExportMyVolunteerHoursPdfQuery>
    {
        public ExportMyVolunteerHoursPdfQueryValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("StartDate must be before or equal to EndDate.");
        }
    }
}
