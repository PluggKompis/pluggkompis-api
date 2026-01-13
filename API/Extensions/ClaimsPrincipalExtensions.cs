using System.Security.Claims;

namespace API.Extensions
{
    /// <summary>
    /// Extension methods to extract user information from JWT claims
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user ID from JWT claims (ClaimTypes.NameIdentifier)
        /// </summary>
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token claims");
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID format in token claims");
            }

            return userId;
        }

        /// <summary>
        /// Gets the user email from JWT claims
        /// </summary>
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("Email not found in token claims");
        }

        /// <summary>
        /// Gets the user role from JWT claims
        /// </summary>
        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new UnauthorizedAccessException("Role not found in token claims");
        }

        /// <summary>
        /// Gets the user's first name from JWT claims
        /// </summary>
        public static string GetUserFirstName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Gets the user's last name from JWT claims
        /// </summary>
        public static string GetUserLastName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
        }
    }
}
