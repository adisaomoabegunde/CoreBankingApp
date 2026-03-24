using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var request = context.Request;

        _logger.LogInformation(
            "Incoming Request: {Method} {Path} from {IP}",
            request.Method,
            request.Path,
            context.Connection.RemoteIpAddress
        );

        try
        {
            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "Outgoing Response: {StatusCode} in {Elapsed}ms",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "Request failed: {Method} {Path} in {Elapsed}ms",
                request.Method,
                request.Path,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}