using API.Extensions;
using Application.Coordinator.Commands.ApproveVolunteerApplication;
using Application.Coordinator.Commands.DeclineVolunteerApplication;
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

        [Authorize(Roles = "Coordinator")]
        [HttpGet("applications")]
        public async Task<IActionResult> GetPendingApplications()
        {
            var coordinatorId = User.GetUserId();
            var result = await _mediator.Send(new GetPendingApplicationsQuery(coordinatorId));
            return this.FromOperationResult(result);
        }

        [Authorize(Roles = "Coordinator")]
        [HttpPut("applications/{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveVolunteerRequest request)
        {
            var coordinatorId = User.GetUserId();
            var result = await _mediator.Send(
                new ApproveVolunteerApplicationCommand(coordinatorId, id, request));

            return this.FromOperationResult(result);
        }

        [Authorize(Roles = "Coordinator")]
        [HttpPut("applications/{id:guid}/decline")]
        public async Task<IActionResult> Decline(Guid id, [FromBody] ApproveVolunteerRequest request)
        {
            var coordinatorId = User.GetUserId();
            var result = await _mediator.Send(
                new DeclineVolunteerApplicationCommand(coordinatorId, id, request));

            return this.FromOperationResult(result);
        }

        [HttpPut("shifts/{id:guid}/attendance")]
        [Authorize(Roles = nameof(UserRole.Coordinator))]
        public async Task<IActionResult> MarkAttendance([FromRoute] Guid id, [FromBody] MarkAttendanceRequest request)
        {
            var coordinatorId = User.GetUserId();

            var result = await _mediator.Send(new MarkShiftAttendanceCommand(coordinatorId, id, request));
            return this.FromOperationResult(result);
        }
    }
}
