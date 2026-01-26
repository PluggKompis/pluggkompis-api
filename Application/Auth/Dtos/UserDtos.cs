using Domain.Models.Enums;

namespace Application.Auth.Dtos
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public UserRole Role { get; set; }

    }

    public class LoginUserDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class UserDtoResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDtoResponse? User { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
