using API.Extensions;
using Application.Users.Commands.ChangeMyPassword;
using Application.Users.Commands.SoftDeleteMe;
using Application.Users.Commands.UpdateMyProfile;
using Application.Users.Dtos;
using Application.Users.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(new GetMyProfileQuery(userId));
            return this.FromOperationResult(result);
        }

        [HttpPatch("me")]
        public async Task<IActionResult> PatchMe([FromBody] UpdateMyProfileDto dto)
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(new UpdateMyProfileCommand(userId, dto));
            return this.FromOperationResult(result);
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(new ChangeMyPasswordCommand(userId, dto));
            return this.FromOperationResult(result);
        }

        [HttpDelete("me")]
        public async Task<IActionResult> SoftDeleteMe()
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(new SoftDeleteMeCommand(userId));
            return this.FromOperationResultNoContent(result);
        }
    }
}
