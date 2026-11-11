namespace Konekto.API.Middlewares;

/// <summary>
/// 404 Guardian Middleware: Intercepts 404 Not Found responses (e.g. undefined routes,
/// missing endpoints) and returns a structured JSON error response explaining the reason.
/// </summary>
public class NotFoundGuardianMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<NotFoundGuardianMiddleware> _logger;

    public NotFoundGuardianMiddleware(RequestDelegate next, ILogger<NotFoundGuardianMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // If response is 404 Not Found and no content has been written yet
        if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
        {
            context.Response.ContentType = "application/json";

            var route = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;
            var reason = $"İstenen kaynak veya uç nokta bulunamadı (Route not found): '{method} {route}'. Lütfen URL ve HTTP metodunu kontrol ediniz.";

            // Write 404 error directly to logs/error.logs
            ErrorFileLogger.Log(404, method, route, reason);

            var errorResponse = new
            {
                status = 404,
                error = "Not Found",
                reason = reason,
                route = route,
                method = method,
                timestamp = DateTime.UtcNow
            };

            _logger.LogWarning("404 Guardian intercepted missing route: {Method} {Route}", method, route);

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}

public static class NotFoundGuardianMiddlewareExtensions
{
    public static IApplicationBuilder UseNotFoundGuardian(this IApplicationBuilder app)
    {
        return app.UseMiddleware<NotFoundGuardianMiddleware>();
    }
}
