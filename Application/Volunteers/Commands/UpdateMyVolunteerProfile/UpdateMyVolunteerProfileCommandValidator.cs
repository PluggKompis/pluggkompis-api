using FluentValidation;

namespace Application.Volunteers.Commands.UpdateMyVolunteerProfile
{
    public class UpdateMyVolunteerProfileCommandValidator : AbstractValidator<UpdateMyVolunteerProfileCommand>
    {
        public UpdateMyVolunteerProfileCommandValidator()
        {
            RuleFor(x => x.VolunteerId).NotEmpty();

            When(x => x.Dto.Bio is not null, () =>
            {
                RuleFor(x => x.Dto.Bio!)
                    .NotEmpty()
                    .MaximumLength(2000);
            });

            When(x => x.Dto.Experience is not null, () =>
            {
                RuleFor(x => x.Dto.Experience!)
                    .NotEmpty()
                    .MaximumLength(2000);
            });

            When(x => x.Dto.Subjects is not null, () =>
            {
                RuleForEach(x => x.Dto.Subjects!).ChildRules(s =>
                {
                    s.RuleFor(x => x.SubjectId).NotEmpty();
                    s.RuleFor(x => x.ConfidenceLevel).IsInEnum();
                });
            });
        }
    }
}
