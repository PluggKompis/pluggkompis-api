using Application.TimeSlots.Dtos;
using FluentValidation;

namespace Application.TimeSlots.Validators
{
    public class CreateTimeSlotRequestValidator : AbstractValidator<CreateTimeSlotRequest>
    {
        public CreateTimeSlotRequestValidator()
        {
            RuleFor(x => x.VenueId)
                .NotEmpty().WithMessage("VenueId is required.");

            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Invalid day of week.");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("StartTime is required.")
                .Must(BeReasonableTime)
                .WithMessage("StartTime must be between 08:00 and 20:00");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("EndTime is required.")
                .Must(BeReasonableTime)
                .WithMessage("EndTime must be between 08:00 and 20:00");

            RuleFor(x => x)
                .Must(x => x.EndTime > x.StartTime)
                .WithMessage("EndTime must be after StartTime.")
                .Must(x => (x.EndTime - x.StartTime).TotalHours <= 4)
                .WithMessage("Time slot duration cannot exceed 4 hours.");

            RuleFor(x => x.MaxStudents)
                .GreaterThan(0)
                .WithMessage("MaxStudents must be greater than 0.")
                .LessThanOrEqualTo(30)
                .WithMessage("MaxStudents cannot exceed 30.");

            RuleFor(x => x.SubjectIds)
                .NotEmpty()
                .WithMessage("At least one SubjectId is required.")
                .Must(x => x.Count <= 3)
                .WithMessage("Cannot have more than 3 subjects to a time slot.");

            // Validate SpecificDate logic
            RuleFor(x => x.SpecificDate)
                .Must((request, specificDate) => request.IsRecurring || specificDate.HasValue)
                .WithMessage("SpecificDate is required for non-recurring timeslots.")
                .Must(BeInFuture)
                .When(x => x.SpecificDate.HasValue)
                .WithMessage("SpecificDate should not be set for recurring timeslots");

            RuleFor(x => x.SpecificDate)
                .Must((request, specificDate) => !request.IsRecurring || !specificDate.HasValue)
                .WithMessage("SpecificDate should not be set for recurring timeslots");
        }

        private bool BeReasonableTime(TimeSpan time)
        {
            var startBoundary = new TimeSpan(8, 0, 0);  // 08:00
            var endBoundary = new TimeSpan(20, 0, 0);   // 20:00
            return time >= startBoundary && time <= endBoundary;
        }

        private bool BeInFuture(DateOnly? date)
        {
            if (!date.HasValue)
                return true;

            return date.Value > DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
