using Application.Bookings.Dtos;
using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Bookings;
using MediatR;

namespace Application.Bookings.Queries.GetMyBookings
{
    /// <summary>
    /// Handler for retrieving all bookings for a user
    /// </summary>
    public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, OperationResult<List<MyBookingDto>>>
    {
        private readonly IGenericRepository<Booking> _bookingRepo;
        public GetMyBookingsQueryHandler(IGenericRepository<Booking> bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        public async Task<OperationResult<List<MyBookingDto>>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
        {
            // Get bookings with all related data in ONE query
            var bookings = await _bookingRepo.FindWithIncludesAsync(
                b => b.BookedByUserId == request.UserId,
                b => b.TimeSlot,           // Include TimeSlot
                b => b.TimeSlot.Venue,     // Include Venue through TimeSlot
                b => b.Child!               // Include Child if exists
            );

            // Build DTOs
            var dtos = bookings
                .OrderByDescending(b => b.BookingDate)
                .ThenBy(b => b.TimeSlot.StartTime)
                .Select(b => new MyBookingDto
                {
                    Id = b.Id,
                    TimeSlotId = b.TimeSlotId,
                    StudentId = b.StudentId,
                    ChildId = b.ChildId,
                    BookedByUserId = b.BookedByUserId,
                    BookingDate = b.BookingDate,
                    BookedAt = b.BookedAt,
                    Status = b.Status,
                    Notes = b.Notes,
                    VenueName = b.TimeSlot.Venue.Name,
                    VenueAddress = b.TimeSlot.Venue.Address,
                    VenueCity = b.TimeSlot.Venue.City,
                    TimeSlotTime = $"{b.TimeSlot.StartTime:hh\\:mm} - {b.TimeSlot.EndTime:hh\\:mm}",
                    ChildName = b.Child?.FirstName
                })
                .ToList();

            return OperationResult<List<MyBookingDto>>.Success(dtos);
        }
    }
}
