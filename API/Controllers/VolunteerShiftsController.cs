using API.Extensions;
using Application.VolunteerShifts.Commands.CancelVolunteerShift;
using Application.VolunteerShifts.Commands.SignupForShift;
using Application.VolunteerShifts.Dtos;
using Application.VolunteerShifts.Queries.GetVolunteerUpcomingShifts;
using Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/volunteers")]
    public class VolunteerShiftsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VolunteerShiftsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Volunteer sign up for a shift in a TimeSlot
        /// </summary>
        [HttpPost("shifts")]
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        public async Task<IActionResult> SignUpForShift([FromBody] CreateShiftSignupRequest request)
        {
            var volunteerId = User.GetUserId();

            var result = await _mediator.Send(new SignupForShiftCommand(volunteerId, request));
            return this.FromOperationResult(result, created: true);
        }

        /// <summary>
        /// Get all my upcoming shifts as a Volunteer
        /// </summary>
        [HttpGet("me")]
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        public async Task<IActionResult> GetMyUpcomingShifts()
        {
            var volunteerId = User.GetUserId();

            var result = await _mediator.Send(
                new GetVolunteerUpcomingShiftsQuery(volunteerId));

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Cancel a Volunteer Shift
        /// </summary>
        [HttpDelete("shifts/{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        public async Task<IActionResult> CancelShift([FromRoute] Guid id)
        {
            var volunteerId = User.GetUserId();

            var result = await _mediator.Send(new CancelVolunteerShiftCommand(volunteerId, id));
            return this.FromOperationResultNoContent(result);
        }
    }
}
