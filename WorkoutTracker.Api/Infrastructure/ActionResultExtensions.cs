using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;
using WorkoutTracker.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkoutTracker.Api.Infrastructure;

/// Provides extension methods to map Service Result objects to ASP.NET Core ActionResults
/// This centralizes the logic for mapping domain status codes to HTTP status codes
/// So you don't have to write the logic in every single controller method
public static class ActionResultExtensions
{

  // The "Action Only" Result(Result)
  // This version is for operations that don't return data if they succeed. You just want to know: "Did it work?"
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

    // The "Data" Result (Result<T>)
    // This version is for operations that must return data if they succeed.
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

     // For common codes like 404 and 400, I use helper classes like NotFoundObjectResult because they make the code more readable.For less common scenarios,
     // or cases like 403 Forbidden where we want to ensure a JSON error message is returned to the client rather than a default system redirect, I use the base
     // ObjectResult and explicitly assign the StatusCode property using an object initializer. This gives me total control over the response while maintaining a
     // consistent JSON format for the UI.
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

  //Could these methods be combined?
  //Technically, yes. In C# a "Result<T>" actually inherits from "Result".

  //However, by having two separate functions, you make the API very clear:
  // If a controller calls result.ToActionResult(), you know no data is being sent back.
  // If a controller calls result.ToActionResult<T>(), you know data is being sent back.
  // So if you see a "T" you know there is a expected return type.