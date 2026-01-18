using API.Extensions;
using Application.Volunteers.Commands.CreateMyVolunteerProfile;
using Application.Volunteers.Commands.UpdateMyVolunteerProfile;
using Application.Volunteers.Dtos;
using Application.Volunteers.Queries.GetMyVolunteerProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/volunteer-profiles")]
    public class VolunteerProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VolunteerProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Volunteer")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new GetMyVolunteerProfileQuery(volunteerId));
            return this.FromOperationResult(result);
        }

        [Authorize(Roles = "Volunteer")]
        [HttpPost("me")]
        public async Task<IActionResult> CreateMe([FromBody] CreateVolunteerProfileRequest request)
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new CreateMyVolunteerProfileCommand(volunteerId, request));
            return this.FromOperationResult(result, created: true);
        }

        [Authorize(Roles = "Volunteer")]
        [HttpPatch("me")]
        public async Task<IActionResult> PatchMe([FromBody] UpdateVolunteerProfileRequest request)
        {
            var volunteerId = User.GetUserId();
            var result = await _mediator.Send(new UpdateMyVolunteerProfileCommand(volunteerId, request));
            return this.FromOperationResult(result);
        }
    }
}
