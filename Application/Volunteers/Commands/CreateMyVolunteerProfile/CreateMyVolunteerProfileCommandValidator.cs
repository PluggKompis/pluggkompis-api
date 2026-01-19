using FluentValidation;

namespace Application.Volunteers.Commands.CreateMyVolunteerProfile
{
    public class CreateMyVolunteerProfileCommandValidator : AbstractValidator<CreateMyVolunteerProfileCommand>
    {
        public CreateMyVolunteerProfileCommandValidator()
        {
            RuleFor(x => x.VolunteerId).NotEmpty();

            RuleFor(x => x.Dto.Bio)
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(x => x.Dto.Experience)
                .NotEmpty()
                .MaximumLength(2000);

            RuleForEach(x => x.Dto.Subjects).ChildRules(s =>
            {
                s.RuleFor(x => x.SubjectId).NotEmpty();
                s.RuleFor(x => x.ConfidenceLevel).IsInEnum();
            });
        }
    }
}
