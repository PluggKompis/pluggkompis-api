using FluentValidation;

namespace Application.Children.Commands.DeleteChild
{
    public class DeleteChildCommandValidator : AbstractValidator<DeleteChildCommand>
    {
        public DeleteChildCommandValidator()
        {
            RuleFor(x => x.ChildId)
                .NotEmpty()
                .WithMessage("ChildId is required.");

            RuleFor(x => x.ParentId)
                .NotEmpty()
                .WithMessage("ParentId is required.");
        }
    }
}
