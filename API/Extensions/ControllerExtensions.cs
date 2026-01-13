using Domain.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions
{
    /// <summary>
    /// Maps OperationResult<T> from the Application layer to HTTP IActionResult responses.
    /// Centralizes status-code logic (200/201/400/404) to keep controllers thin and consistent.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Handles generic OperationResult<T> for operations that return data
        /// </summary>
        public static IActionResult FromOperationResult<T>(
            this ControllerBase controller,
            OperationResult<T> result,
            bool created = false)
        {
            if (result.IsSuccess)
            {
                return created
                    ? controller.StatusCode(StatusCodes.Status201Created, result)
                    : controller.Ok(result);
            }

            var hasNotFound = result.Errors.Any(e =>
                e.Contains("not found", StringComparison.OrdinalIgnoreCase));

            return hasNotFound
                ? controller.NotFound(result)
                : controller.BadRequest(result);
        }

        /// <summary>
        /// Handles non-generic OperationResult for NoContent operations (like Delete)
        /// </summary>
        public static IActionResult FromOperationResultNoContent(
            this ControllerBase controller,
            OperationResult result)  
        {
            if (result.IsSuccess)
                return controller.NoContent();

            var hasNotFound = result.Errors.Any(e =>
                e.Contains("not found", StringComparison.OrdinalIgnoreCase));

            return hasNotFound
                ? controller.NotFound(result)
                : controller.BadRequest(result);
        }
    }
}
