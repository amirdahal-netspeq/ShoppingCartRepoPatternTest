using Microsoft.AspNetCore.Diagnostics;
using ShoppingCart.Application.Common;
using ShoppingCart.Domain.Common;

namespace ShoppingCart.Api.Middleware
{
    internal sealed class ApiExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ApiExceptionHandler> _logger;

        public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
            => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
        {
            var (status, message) = exception switch
            {
                // DomainException messages are written for users. Never put sensitive data in them.
                DomainException e => (StatusCodes.Status400BadRequest, e.Message),
                DataConflictException => (StatusCodes.Status409Conflict,
                    "The change conflicts with existing data, or the data was changed at the same time. Reload and try again."),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Unhandled exception on {Path}", httpContext.Request.Path);
            else
                _logger.LogWarning("Request rejected with {Status} on {Path}: {Message}",
                    status, httpContext.Request.Path, exception.Message);

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(ApiResponseDto<object>.Error(message), ct);

            return true;
        }
    }
}
