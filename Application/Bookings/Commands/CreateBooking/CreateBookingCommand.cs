using Application.Bookings.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Bookings.Commands.CreateBooking
{
    /// <summary>
    /// Command to create a new booking for a timeslot
    /// </summary>
    public class CreateBookingCommand : IRequest<OperationResult<BookingDto>>
    {
        public Guid UserId { get; set; }  // The user making the booking
        public CreateBookingRequest Request { get; set; }

        public CreateBookingCommand(Guid userId, CreateBookingRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
