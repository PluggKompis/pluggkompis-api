using Application.Auth.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequest Dto) : IRequest<OperationResult<AuthResponseDto>>;
}
