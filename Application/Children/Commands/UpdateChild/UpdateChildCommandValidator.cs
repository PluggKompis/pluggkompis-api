using FluentValidation;

namespace Application.Children.Commands.UpdateChild
{
    public class UpdateChildCommandValidator : AbstractValidator<UpdateChildCommand>
    {
        public UpdateChildCommandValidator()
        {
            RuleFor(x => x.ChildId)
                .NotEmpty()
                .WithMessage("ChildId is required.");

            RuleFor(x => x.ParentId)
                .NotEmpty()
                .WithMessage("ParentId is required.");

            RuleFor(x => x.Request.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(100).WithMessage("FirstName must be at most 100 characters.");

            RuleFor(x => x.Request.BirthYear)
                .InclusiveBetween(1900, DateTime.UtcNow.Year)
                .WithMessage($"BirthYear must be between 1900 and {DateTime.UtcNow.Year}.");

            RuleFor(x => x.Request.SchoolGrade)
                .NotEmpty().WithMessage("SchoolGrade is required.")
                .MaximumLength(50).WithMessage("SchoolGrade must be at most 50 characters.");
        }
    }
}
