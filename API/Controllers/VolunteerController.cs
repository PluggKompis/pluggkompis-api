using API.Extensions;
using Application.Volunteers.Commands.CreateMyVolunteerProfile;
using Application.Volunteers.Commands.UpdateMyVolunteerProfile;
using Application.Volunteers.Dtos;
using Application.Volunteers.Queries.ExportMyVolunteerHoursPdf;
using Application.Volunteers.Queries.GetMyVolunteerApplications;
using Application.Volunteers.Queries.GetMyVolunteerProfile;
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
    public class VolunteersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VolunteersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // =========================================================
        // Volunteer Profile
        // =========================================================

        /// <summary>
        /// Get my volunteer profile
        /// GET /api/volunteers/me/profile
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpGet("me/profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new GetMyVolunteerProfileQuery(volunteerId));
            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Create my volunteer profile (Submit application creates profile)
        /// POST /api/volunteers/me/profile
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpPost("me/profile")]
        public async Task<IActionResult> CreateMyProfile([FromBody] CreateVolunteerProfileRequest request)
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new CreateMyVolunteerProfileCommand(volunteerId, request));
            return this.FromOperationResult(result, created: true);
        }

        /// <summary>
        /// Update my volunteer profile (bio, experience, subjects)
        /// PATCH /api/volunteers/me/profile
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpPatch("me/profile")]
        public async Task<IActionResult> PatchMyProfile([FromBody] UpdateVolunteerProfileRequest request)
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new UpdateMyVolunteerProfileCommand(volunteerId, request));
            return this.FromOperationResult(result);
        }

        // =========================================================
        // Volunteer Shifts
        // =========================================================

        /// <summary>
        /// Volunteer sign up for a shift in a TimeSlot
        /// POST /api/volunteers/shifts
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpPost("shifts")]
        public async Task<IActionResult> SignUpForShift([FromBody] CreateShiftSignupRequest request)
        {
            var volunteerId = User.GetUserId();

            var result = await _mediator.Send(new SignupForShiftCommand(volunteerId, request));
            return this.FromOperationResult(result, created: true);
        }

        /// <summary>
        /// Get all my shifts (upcoming + past)
        /// GET /api/volunteers/me/shifts
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpGet("me/shifts")]
        public async Task<IActionResult> GetMyShifts()
        {
            var volunteerId = User.GetUserId();

            // NOTE:
            // Keeping your current query to avoid changing Application layer now.
            // If/when you implement "upcoming + past", swap to a new query like:
            // GetMyVolunteerShiftsQuery(volunteerId)
            var result = await _mediator.Send(new GetVolunteerUpcomingShiftsQuery(volunteerId));

            return this.FromOperationResult(result);
        }

        /// <summary>
        /// Cancel a Volunteer Shift
        /// DELETE /api/volunteers/shifts/{id}
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpDelete("shifts/{id:guid}")]
        public async Task<IActionResult> CancelShift([FromRoute] Guid id)
        {
            var volunteerId = User.GetUserId();

            var result = await _mediator.Send(new CancelVolunteerShiftCommand(volunteerId, id));
            return this.FromOperationResultNoContent(result);
        }

        // =========================================================
        // Volunteer Applications
        // =========================================================

        /// <summary>
        /// Get all my applications (pending/approved/declined)
        /// GET /api/volunteers/me/applications
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpGet("me/applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new GetMyVolunteerApplicationsQuery(volunteerId));
            return this.FromOperationResult(result);
        }

        // =========================================================
        // Volunteer Reports
        // =========================================================

        /// <summary>
        /// Export my volunteer hours as PDF
        /// GET /api/volunteers/me/reports/hours.pdf?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        /// </summary>
        [Authorize(Roles = nameof(UserRole.Volunteer))]
        [HttpGet("me/reports/hours.pdf")]
        public async Task<IActionResult> ExportHoursPdf([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new ExportMyVolunteerHoursPdfQuery(volunteerId, startDate, endDate));
            return this.FromOperationResultFile(result);
        }

    }
}
