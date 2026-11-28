using Microsoft.AspNetCore.Mvc;
using TrainBooking.Api.Services.Exceptions;

namespace TrainBooking.Api.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteProblem(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteProblem(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (BadRequestException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    private static Task WriteProblem(HttpContext context, int statusCode, string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;
        var problem = new ProblemDetails { Status = statusCode, Detail = detail };
        return context.Response.WriteAsJsonAsync(problem);
    }
}
