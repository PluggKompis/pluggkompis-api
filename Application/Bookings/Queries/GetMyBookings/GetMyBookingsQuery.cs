using Application.Bookings.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Bookings.Queries.GetMyBookings
{
    /// <summary>
    /// Query to get all bookings for the current user
    /// </summary>
    public class GetMyBookingsQuery : IRequest<OperationResult<List<MyBookingDto>>>
    {
        public Guid UserId { get; set; }
        public GetMyBookingsQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
