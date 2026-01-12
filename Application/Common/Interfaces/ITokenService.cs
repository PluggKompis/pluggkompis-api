using Domain.Models.Entities.Users;

namespace Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwt(User user);

    }
}
