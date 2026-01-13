using Application.Auth.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Auth.Commands.Register
{
    public record RegisterUserCommand(RegisterUserDto Dto) : IRequest<OperationResult<AuthResponseDto>>;
}
