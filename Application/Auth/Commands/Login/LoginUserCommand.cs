using Application.Auth.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Auth.Commands.Login
{
    public record LoginUserCommand(LoginUserDto Dto) : IRequest<OperationResult<AuthResponseDto>>;
}
