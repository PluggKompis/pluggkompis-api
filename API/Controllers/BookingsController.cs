using API.Extensions;
using Application.Bookings.Commands.CancelBooking;
using Application.Bookings.Commands.CreateBooking;
using Application.Bookings.Dtos;
using Application.Bookings.Queries.GetMyBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Controller for managing bookings
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Parent,Student")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.GetUserId();
            var query = new GetMyBookingsQuery(userId);
            var result = await _mediator.Send(query);
            return this.FromOperationResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Parent,Student")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var userId = User.GetUserId();
            var command = new CreateBookingCommand(userId, request);
            var result = await _mediator.Send(command);
            return this.FromOperationResult(result, created: true);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Parent,Student")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var userId = User.GetUserId();
            var command = new CancelBookingCommand(userId, id);
            var result = await _mediator.Send(command);
            return this.FromOperationResultNoContent(result);
        }
    }
}
