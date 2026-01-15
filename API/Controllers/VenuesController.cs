using API.Extensions;
using Application.TimeSlots.Queries.GetTimeSlotsByVenue;
using Application.Venues.Commands.CreateVenue;
using Application.Venues.Commands.DeleteVenue;
using Application.Venues.Commands.UpdateVenue;
using Application.Venues.Dtos;
using Application.Venues.Queries.GetMyVenue;
using Application.Venues.Queries.GetVenueById;
using Application.Venues.Queries.GetVenues;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Handles venue management operations
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VenuesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all venues with optional filtering
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetVenues([FromQuery] VenueFilterParams filters)
        {
            var query = new GetVenuesQuery(filters);
            var result = await _mediator.Send(query);

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Get venue details by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVenueById(Guid id)
        {
            var query = new GetVenueByIdQuery(id);
            var result = await _mediator.Send(query);

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Get my venue (coordinator only)
        /// </summary>
        [HttpGet("my-venue")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetMyVenue()
        {
            var coordinatorId = User.GetUserId();

            var query = new GetMyVenueQuery(coordinatorId);
            var result = await _mediator.Send(query);

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Create a new venue (coordinator only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequest request)
        {
            var coordinatorId = User.GetUserId();

            var command = new CreateVenueCommand(coordinatorId, request);
            var result = await _mediator.Send(command);

            return this.FromOperationResult(result, created: true);
        }

        /// <summary>
        /// Update venue (coordinator only, own venue)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] UpdateVenueRequest request)
        {
            var coordinatorId = User.GetUserId();

            var command = new UpdateVenueCommand(id, coordinatorId, request);
            var result = await _mediator.Send(command);

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Delete venue (coordinator only, own venue)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> DeleteVenue(Guid id)
        {
            var coordinatorId = User.GetUserId();

            var command = new DeleteVenueCommand(id, coordinatorId);
            var result = await _mediator.Send(command);

            return this.FromOperationResultNoContent(result);
        }

        /// <summary>
        /// Get all TimeSlots for a venue (public access)
        /// </summary>
        [HttpGet("{venueId}/timeslots")]
        public async Task<IActionResult> GetVenueTimeslots(Guid venueId, [FromQuery] bool includeCancelled = false)
        {
            var query = new GetTimeSlotsByVenueQuery(venueId, includeCancelled);
            var result = await _mediator.Send(query);
            return this.FromOperationResult(result);
        }
    }
}
