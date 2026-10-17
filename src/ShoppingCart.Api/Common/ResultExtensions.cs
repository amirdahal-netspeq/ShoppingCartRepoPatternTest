using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.Common;

namespace ShoppingCart.Api.Common
{
    internal static class ResultExtensions
    {
        public static OkObjectResult OkResponse<T>(this ControllerBase controller, T? data, string? message = null)
        => controller.Ok(ApiResponseDto<T>.Ok(data, message));

        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
            => result.IsSuccess ? controller.OkResponse(result.Value) : result.Error!.ToFailure(controller);

        // For results that carry no data.
        public static IActionResult ToActionResult(this Result result, ControllerBase controller, string successMessage)
            => result.IsSuccess ? controller.OkResponse<object>(null, successMessage) : result.Error!.ToFailure(controller);

        public static IActionResult ToFailure(this Error error, ControllerBase controller)
        {
            var status = error.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };
            return controller.StatusCode(status, ApiResponseDto<object>.Error(error.Message));
        }
    }
}
