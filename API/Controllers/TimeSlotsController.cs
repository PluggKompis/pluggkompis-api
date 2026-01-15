using API.Extensions;
using Application.TimeSlots.Commands.CreateTimeSlot;
using Application.TimeSlots.Commands.DeleteTimeSlot;
using Application.TimeSlots.Commands.UpdateTimeSlot;
using Application.TimeSlots.Dtos;
using Application.TimeSlots.Queries.GetTimeslotById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Manages TimeSlot CRUD operations (Coordinator only)
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TimeSlotsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get a specific TimeSlot by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeSlotById(Guid id)
        {
            var query = new GetTimeSlotByIdQuery(id);
            var result = await _mediator.Send(query);
            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Create a new TimeSlot (Coordinator only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> CreateTimeSlot([FromBody] CreateTimeSlotRequest request)
        {
            var coordinatorId = User.GetUserId();

            var command = new CreateTimeSlotCommand(coordinatorId, request);
            var result = await _mediator.Send(command);
            return this.FromOperationResult(result, created: true);
        }

        /// <summary>
        /// Update an existing TimeSlot (Coordinator only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> UpdateTimeSlot(Guid id, [FromBody] UpdateTimeSlotRequest request)
        {
            var coordinatorId = User.GetUserId();

            var command = new UpdateTimeSlotCommand(id, coordinatorId, request);
            var result = await _mediator.Send(command);
            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Cancel a TimeSlot (sets status to Cancelled)
        /// </summary>
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> CancelTimeSlot(Guid id)
        {
            var coordinatorId = User.GetUserId();

            // Create an update request that just sets status to Cancelled
            // We need to fetch the timeslot first to preserve other fields
            var getQuery = new GetTimeSlotByIdQuery(id);
            var getResult = await _mediator.Send(getQuery);

            if (!getResult.IsSuccess)
                return this.FromOperationResult(getResult);

            var timeSlot = getResult.Data!;

            var updateRequest = new UpdateTimeSlotRequest
            {
                DayOfWeek = timeSlot.DayOfWeek,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                MaxStudents = timeSlot.MaxStudents,
                IsRecurring = timeSlot.IsRecurring,
                SpecificDate = timeSlot.SpecificDate,
                SubjectIds = new List<Guid>(),  // Will need to fetch subject IDs from repository
                Status = Domain.Models.Enums.TimeSlotStatus.Cancelled  // This is what we're changing
            };

            var command = new UpdateTimeSlotCommand(id, coordinatorId, updateRequest);
            var result = await _mediator.Send(command);
            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Delete a TimeSlot (Coordinator only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> DeleteTimeSlot(Guid id)
        {
            var coordinatorId = User.GetUserId();

            var command = new DeleteTimeSlotCommand(id, coordinatorId);
            var result = await _mediator.Send(command);

            return this.FromOperationResultNoContent(result);
        }
    }
}
