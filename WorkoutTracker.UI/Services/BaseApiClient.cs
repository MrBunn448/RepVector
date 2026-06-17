using System.Net;
using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Base class for API clients that provides common utilities for handling responses and error mapping.
public abstract class BaseApiClient
{
    protected readonly HttpClient HttpClient;

    protected BaseApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    /// Extracts an error message from an HTTP response, attempting to parse a JSON body if available.
    protected async Task<string> TryGetErrorMessage(HttpResponseMessage response)
    {
        try
        {
            var errorData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (errorData != null && errorData.TryGetValue("message", out var message))
            {
                return message;
            }
        }
        catch { /* Fallback to generic status message */ }

        return $"Server error: {(int)response.StatusCode} ({response.ReasonPhrase})";
    }

    /// Maps a standard HTTP status code to the application's internal ResultType.
    protected ResultType MapToResultType(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.NotFound => ResultType.NotFound,
            HttpStatusCode.Forbidden => ResultType.Forbidden,
            HttpStatusCode.Conflict => ResultType.Conflict,
            HttpStatusCode.BadRequest => ResultType.BadRequest,
            HttpStatusCode.Unauthorized => ResultType.Forbidden,
            _ => ResultType.Error
        };
    }

    /// Handles an unsuccessful HTTP response by creating a failed Result object with mapped error details.
    /// Post requests that expect a return value. By using T we can return a Result<T> with the appropriate type, while still providing error details.
    protected async Task<Result<T>> HandleFailure<T>(HttpResponseMessage response)
    {
        var message = await TryGetErrorMessage(response);
        var type = MapToResultType(response.StatusCode);
        return Result<T>.Failure(message, type);
    }

    /// Handles an unsuccessful HTTP response by creating a failed non-generic Result object.
    /// For Get requests that don't return data.
    protected async Task<Result> HandleFailure(HttpResponseMessage response)
    {
        var message = await TryGetErrorMessage(response);
        var type = MapToResultType(response.StatusCode);
        return Result.Failure(message, type);
    }
}
