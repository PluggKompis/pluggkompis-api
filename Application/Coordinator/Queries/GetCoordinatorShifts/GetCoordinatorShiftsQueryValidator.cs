using FluentValidation;

namespace Application.Coordinator.Queries.GetCoordinatorShifts
{
    public class GetCoordinatorShiftsQueryValidator : AbstractValidator<GetCoordinatorShiftsQuery>
    {
        public GetCoordinatorShiftsQueryValidator()
        {
            RuleFor(x => x.CoordinatorId)
                .NotEmpty();

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 200);

            When(x => x.StartUtc.HasValue && x.EndUtcExclusive.HasValue, () =>
            {
                RuleFor(x => x.EndUtcExclusive!.Value)
                    .GreaterThan(x => x.StartUtc!.Value)
                    .WithMessage("EndUtcExclusive must be greater than StartUtc.");
            });
        }
    }
}
