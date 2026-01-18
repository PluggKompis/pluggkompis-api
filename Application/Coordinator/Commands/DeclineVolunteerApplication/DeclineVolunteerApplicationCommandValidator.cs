using FluentValidation;

namespace Application.Coordinator.Commands.DeclineVolunteerApplication
{
    public class DeclineVolunteerApplicationCommandValidator : AbstractValidator<DeclineVolunteerApplicationCommand>
    {
        public DeclineVolunteerApplicationCommandValidator()
        {
            RuleFor(x => x.CoordinatorId).NotEmpty();
            RuleFor(x => x.ApplicationId).NotEmpty();
        }
    }
}
