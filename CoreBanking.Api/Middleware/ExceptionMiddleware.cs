using System.Net;
using System.Text.Json;
using CoreBanking.Domain.Common.Exceptions;

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