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
            var route = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;
            var reason = $"İstenen kaynak veya uç nokta bulunamadı (Route not found): '{method} {route}'. Lütfen URL ve HTTP metodunu kontrol ediniz.";

            // Write 404 error directly to logs/error.logs
            ErrorFileLogger.Log(404, method, route, reason);

            // Check if request is an API request (Postman, REST client, JSON accept)
            // Postman/curl usually accept */*; only HTML-navigation requests redirect.
            var acceptsHtml = context.Request.Headers.Accept.ToString()
                .Contains("text/html", StringComparison.OrdinalIgnoreCase);
            var isApi = route.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
                     || !acceptsHtml;

            if (isApi)
            {
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    status = 404,
                    error = "Not Found",
                    reason = reason,
                    route = route,
                    method = method,
                    timestamp = DateTime.UtcNow
                };

                _logger.LogWarning("404 Guardian intercepted missing API route: {Method} {Route}", method, route);

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else if (!route.Equals("/not-found", StringComparison.OrdinalIgnoreCase) &&
                     !route.Equals("/404", StringComparison.OrdinalIgnoreCase))
            {
                // Redirect invalid web browser requests to the generic 404 page
                _logger.LogInformation("404 Guardian redirecting invalid request {Route} to /not-found", route);
                context.Response.Redirect("/not-found");
            }
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
