using FluentValidation;

namespace Application.Bookings.Commands.CancelBooking
{
    /// <summary>
    /// Validator for CancelBookingCommand to ensure valid cancellation request
    /// </summary>
    public class CancelBookingValidator : AbstractValidator<CancelBookingCommand>
    {
        public CancelBookingValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("Booking ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
    }
}
