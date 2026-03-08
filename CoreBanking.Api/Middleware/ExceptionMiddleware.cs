using System.Net;
using System.Text.Json;
using CoreBanking.Domain.Common.Exceptions;
using FluentValidation;

namespace CoreBanking.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
            await HandleValidationException(context, errors);
        }
        catch (BadRequestException ex)
        {
            await HandleException(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleException(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleException(context, HttpStatusCode.InternalServerError, ex.Message);
        }

    }

    private static async Task HandleValidationException(HttpContext context, List<string> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var response = new
        {
            status = (int)HttpStatusCode.BadRequest,
            message = "Validation failed",
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static async Task HandleException(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

}