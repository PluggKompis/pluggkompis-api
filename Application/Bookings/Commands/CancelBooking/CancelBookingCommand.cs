using Domain.Models.Common;
using MediatR;

namespace Application.Bookings.Commands.CancelBooking
{
    /// <summary>
    /// Command to cancel an existing booking
    /// </summary>
    public class CancelBookingCommand : IRequest<OperationResult>
    {
        public Guid UserId { get; set; }
        public Guid BookingId { get; set; }

        public CancelBookingCommand(Guid userId, Guid bookingId)
        {
            UserId = userId;
            BookingId = bookingId;
        }
    }
}
