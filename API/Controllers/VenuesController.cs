using Application.Venues.Commands.CreateVenue;
using Application.Venues.Commands.DeleteVenue;
using Application.Venues.Commands.UpdateVenue;
using Application.Venues.Dtos;
using Application.Venues.Queries.GetMyVenue;
using Application.Venues.Queries.GetVenueById;
using Application.Venues.Queries.GetVenues;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetVenues([FromQuery] VenueFilterParams filters)
        {
            var query = new GetVenuesQuery(filters);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Get venue details by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenueById(Guid id)
        {
            var query = new GetVenueByIdQuery(id);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Get my venue (coordinator only)
        /// </summary>
        // TODO: Add [Authorize(Roles = "Coordinator")] after auth is ready
        [HttpGet("my-venue")]
        public async Task<IActionResult> GetMyVenue()
        {
            // TODO: Get from JWT claims after auth is ready
            var coordinatorId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var query = new GetMyVenueQuery(coordinatorId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Create a new venue (coordinator only)
        /// </summary>
        // TODO: Add [Authorize(Roles = "Coordinator")] after auth is ready
        [HttpPost]
        public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequest request)
        {
            // TODO: Get from JWT claims after auth is ready
            var coordinatorId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var command = new CreateVenueCommand(coordinatorId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Errors });

            return CreatedAtAction(
                nameof(GetVenueById),
                new { id = result.Data!.Id },
                result.Data);
        }

        /// <summary>
        /// Update venue (coordinator only, own venue)
        /// </summary>
        // TODO: Add [Authorize(Roles = "Coordinator")] after auth is ready
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] UpdateVenueRequest request)
        {
            // TODO: Get from JWT claims after auth is ready
            var coordinatorId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var command = new UpdateVenueCommand(id, coordinatorId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Delete venue (coordinator only, own venue)
        /// </summary>
        // TODO: Add [Authorize(Roles = "Coordinator")] after auth is ready
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(Guid id)
        {
            // TODO: Get from JWT claims after auth is ready
            var coordinatorId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var command = new DeleteVenueCommand(id, coordinatorId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Errors });

            return NoContent();
        }
    }
}
