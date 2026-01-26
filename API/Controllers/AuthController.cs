using Application.Auth.Commands.Login;
using Application.Auth.Commands.RefreshToken;
using Application.Auth.Commands.Register;
using Application.Auth.Dtos;
using API.Extensions;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(OperationResult<AuthResponseDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var result = await _mediator.Send(new RegisterUserCommand(dto));
            return this.FromOperationResult(result, created: true);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(OperationResult<AuthResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var result = await _mediator.Send(new LoginUserCommand(dto));
            return this.FromOperationResult(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(OperationResult<AuthResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest dto)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(dto));
            return this.FromOperationResult(result);
        }
    }
}
