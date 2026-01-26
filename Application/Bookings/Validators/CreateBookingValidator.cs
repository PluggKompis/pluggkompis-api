using Application.Bookings.Dtos;
using FluentValidation;

namespace Application.Bookings.Commands.CreateBooking
{
    /// <summary>
    /// Validator for CreateBookingRequest to ensure valid booking data
    /// </summary>
    public class CreateBookingValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.TimeSlotId)
                .NotEmpty()
                .WithMessage("TimeSlot ID is required");

            RuleFor(x => x.BookingDate)
                .NotEmpty()
                .WithMessage("Booking date is required")
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Booking date cannot be in the past");

            RuleFor(x => x.Notes)
                .MaximumLength(2000)
                .WithMessage("Notes cannot exceed 2000 characters");
        }
    }
}
