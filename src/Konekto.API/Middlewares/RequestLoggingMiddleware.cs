using System.Diagnostics;

namespace Konekto.API.Middlewares;

/// <summary>
/// Custom logger middleware that writes HTTP request information to the terminal:
/// Format: time method route restype duration (e.g. 10:20:30 post /api/job-postings 201 18ms)
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var time = DateTime.Now.ToString("HH:mm:ss");
            var method = context.Request.Method.ToLower();
            var route = context.Request.Path.Value ?? "/";
            var statusCode = context.Response.StatusCode;
            var duration = $"{stopwatch.ElapsedMilliseconds}ms";

            // Print raw log directly to the terminal console
            // Example: 10:20:30 post /law 201 18ms
            var logMessage = $"{time} {method} {route} {statusCode} {duration}";
            Console.WriteLine(logMessage);

            _logger.LogInformation("{Time} {Method} {Route} {StatusCode} {Duration}", time, method, route, statusCode, duration);
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
