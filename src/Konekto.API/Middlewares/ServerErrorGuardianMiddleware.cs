using System.Net;

namespace Konekto.API.Middlewares;

/// <summary>
/// 5xx Guardian Middleware: Intercepts unhandled exceptions and 5xx server errors,
/// prevents server crashes, logs the full exception, and returns a clean, structured
/// JSON response with the error reason to the client.
/// </summary>
public class ServerErrorGuardianMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ServerErrorGuardianMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ServerErrorGuardianMiddleware(
        RequestDelegate next,
        ILogger<ServerErrorGuardianMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // In case an endpoint explicitly set 5xx status without throwing an exception
            if (context.Response.StatusCode >= 500 && !context.Response.HasStarted)
            {
                await HandleServerErrorAsync(context, null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "5xx Guardian caught an unhandled exception at {Method} {Path}",
                context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
                throw; // A response already sent cannot be replaced with JSON.

            await HandleServerErrorAsync(context, ex);
        }
    }

    private async Task HandleServerErrorAsync(HttpContext context, Exception? exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var route = context.Request.Path.Value ?? "/";
        var method = context.Request.Method;
        // Keep diagnostics in server logs; never expose exception details in Production.
        var detail = _env.IsDevelopment() ? exception?.Message : null;

        ErrorFileLogger.Log(500, method, route, "Internal Server Error",
            exception?.Message ?? "Sunucu isteği işlerken bir hata ile karşılaştı.",
            exception?.ToString());

        var errorResponse = new
        {
            status = 500,
            error = "Internal Server Error",
            reason = "Sunucu tarafında beklenmeyen bir hata meydana geldi (Internal server error).",
            detail,
            route = route,
            method = method,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}

public static class ServerErrorGuardianMiddlewareExtensions
{
    public static IApplicationBuilder UseServerErrorGuardian(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ServerErrorGuardianMiddleware>();
    }

    public static IApplicationBuilder Use5xxGuardian(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ServerErrorGuardianMiddleware>();
    }
}
