using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Models;

namespace WorkoutTracker.Api.Infrastructure;

/// Provides extension methods to map Service Result objects to ASP.NET Core ActionResults.
/// This centralizes the logic for mapping domain status codes to HTTP status codes.
/// // So you don't have to write the logic in every single controller method
public static class ActionResultExtensions
{

    /// Converts a basic Result object into an appropriate IActionResult.
    /// returnsAn IActionResult such as OkResult, NotFoundObjectResult, etc.
    public static IActionResult ToActionResult(this Result result)
    {
        var response = new { message = result.ErrorMessage };
        return result.Type switch
        {
            ResultType.Ok => new OkResult(),
            ResultType.NotFound => new NotFoundObjectResult(response),
            ResultType.Forbidden => new ObjectResult(response) { StatusCode = 403 },
            ResultType.Conflict => new ConflictObjectResult(response),
            ResultType.BadRequest => new BadRequestObjectResult(response),
            _ => new ObjectResult(new { message = result.ErrorMessage ?? "An unexpected error occurred." }) { StatusCode = 500 }
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        var response = new { message = result.ErrorMessage };
        return result.Type switch
        {
            ResultType.NotFound => new NotFoundObjectResult(response),
            ResultType.Forbidden => new ObjectResult(response) { StatusCode = 403 },
            ResultType.Conflict => new ConflictObjectResult(response),
            ResultType.BadRequest => new BadRequestObjectResult(response),
            _ => new ObjectResult(new { message = result.ErrorMessage ?? "An unexpected error occurred." }) { StatusCode = 500 }
        };
    }
}
