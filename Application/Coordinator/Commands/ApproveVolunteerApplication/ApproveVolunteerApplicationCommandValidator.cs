using FluentValidation;

namespace Application.Coordinator.Commands.ApproveVolunteerApplication
{
    public class ApproveVolunteerApplicationCommandValidator : AbstractValidator<ApproveVolunteerApplicationCommand>
    {
        public ApproveVolunteerApplicationCommandValidator()
        {
            RuleFor(x => x.CoordinatorId).NotEmpty();
            RuleFor(x => x.ApplicationId).NotEmpty();
        }
    }
}
