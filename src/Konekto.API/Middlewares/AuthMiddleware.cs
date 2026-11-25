using System.Security.Claims;
using Konekto.API.Services;

namespace Konekto.API.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, JwtTokenService jwtTokenService)
    {
        // 1. Extract token from Header or Cookie
        string? token = null;

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
        }
        else if (context.Request.Headers.TryGetValue("X-Auth-Token", out var customHeader))
        {
            token = customHeader.FirstOrDefault();
        }
        else if (context.Request.Cookies.TryGetValue("konekto_token", out var cookieToken))
        {
            token = cookieToken;
        }

        // 2. Validate token and attach ClaimsPrincipal if valid and User is not already authenticated via Cookie
        if (!string.IsNullOrEmpty(token) && (context.User.Identity == null || !context.User.Identity.IsAuthenticated))
        {
            var principal = jwtTokenService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value ?? "unknown";
                var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
                _logger.LogInformation("[AuthMiddleware] Valid JWT Token: User '{Email}' (Role: {Role}) for path {Path}", email, role, context.Request.Path);
            }
            else
            {
                _logger.LogWarning("[AuthMiddleware] Invalid or expired token supplied for path {Path}", context.Request.Path);
            }
        }

        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        bool isApiRequest = path.StartsWith("/api/");

        // 3. Route Guard: Admin Panel & Admin API
        if (path.StartsWith("/admin") || path.StartsWith("/api/admin"))
        {
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                if (isApiRequest)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = 401,
                        error = "Unauthorized",
                        message = "Bu yönetim kaynağına erişmek için kimlik doğrulaması (giriş) yapmalısınız."
                    });
                    return;
                }
                else
                {
                    context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(context.Request.Path)}");
                    return;
                }
            }

            if (!context.User.IsInRole("Admin"))
            {
                if (isApiRequest)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = 403,
                        error = "Forbidden",
                        message = "Bu işlemi gerçekleştirmek için 'Admin' rolüne sahip olmalısınız."
                    });
                    return;
                }
                else
                {
                    context.Response.Redirect("/Account/AccessDenied");
                    return;
                }
            }
        }

        await _next(context);
    }
}

public static class AuthMiddlewareExtensions
{
    public static IApplicationBuilder UseKonektoAuthMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthMiddleware>();
    }
}
