using API.Extensions;
using Application.Coordinator.Commands.ApproveVolunteerApplication;
using Application.Coordinator.Commands.DeclineVolunteerApplication;
using Application.Coordinator.Queries.GetCoordinatorShifts;
using Application.Coordinator.Queries.GetPendingApplications;
using Application.Volunteers.Commands.MarkShiftAttendance;
using Application.Volunteers.Dtos;
using Application.VolunteerShifts.Dtos;
using Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/coordinator")]
    public class CoordinatorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoordinatorController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get all pending volunteer applications for the coordinator
        /// </summary>
        [Authorize(Roles = "Coordinator")]
        [HttpGet("applications")]
        public async Task<IActionResult> GetPendingApplications()
        {
            var coordinatorId = User.GetUserId();
            var result = await _mediator.Send(new GetPendingApplicationsQuery(coordinatorId));
            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Approve a volunteer application
        /// </summary>
        [Authorize(Roles = "Coordinator")]
        [HttpPut("applications/{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveVolunteerRequest request)
        {
            var coordinatorId = User.GetUserId();
            var result = await _mediator.Send(
                new ApproveVolunteerApplicationCommand(coordinatorId, id, request));

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Decline a volunteer application
        /// </summary>
        [Authorize(Roles = "Coordinator")]
        [HttpPut("applications/{id:guid}/decline")]
        public async Task<IActionResult> Decline(Guid id, [FromBody] ApproveVolunteerRequest request)
        {
            var coordinatorId = User.GetUserId();
            var result = await _mediator.Send(
                new DeclineVolunteerApplicationCommand(coordinatorId, id, request));

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Mark attendance for a volunteer in a shift
        /// </summary>
        [HttpPut("shifts/{id:guid}/attendance")]
        [Authorize(Roles = nameof(UserRole.Coordinator))]
        public async Task<IActionResult> MarkAttendance([FromRoute] Guid id, [FromBody] MarkAttendanceRequest request)
        {
            var coordinatorId = User.GetUserId();

            var result = await _mediator.Send(new MarkShiftAttendanceCommand(coordinatorId, id, request));
            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Get coordinator shifts with optional filters and pagination
        /// </summary>
        [HttpGet("shifts")]
        public async Task<IActionResult> GetShifts(
            [FromQuery] DateTime? startUtc,
            [FromQuery] DateTime? endUtcExclusive,
            [FromQuery] bool? isAttended,
            [FromQuery] Guid? venueId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var coordinatorId = User.GetUserId();

            var query = new GetCoordinatorShiftsQuery(coordinatorId)
            {
                StartUtc = startUtc,
                EndUtcExclusive = endUtcExclusive,
                IsAttended = isAttended,
                VenueId = venueId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return this.FromOperationResult(result);
        }
    }
}
