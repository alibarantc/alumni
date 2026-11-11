using System.Text;

namespace Konekto.API.Middlewares;

/// <summary>
/// File logger utility that writes application errors and exceptions
/// directly to the 'logs/error.logs' file.
/// </summary>
public static class ErrorFileLogger
{
    private static readonly object _lock = new();

    public static void Log(int statusCode, string method, string route, string reason, string? detail = null, string? stackTrace = null)
    {
        try
        {
            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            var logFile = Path.Combine(logDir, "error.logs");
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var sb = new StringBuilder();
            sb.AppendLine($"[{timestamp}] [{statusCode}] {method.ToUpperInvariant()} {route}");
            sb.AppendLine($"Reason: {reason}");

            if (!string.IsNullOrWhiteSpace(detail))
            {
                sb.AppendLine($"Detail: {detail}");
            }

            if (!string.IsNullOrWhiteSpace(stackTrace))
            {
                sb.AppendLine($"StackTrace: {stackTrace}");
            }

            sb.AppendLine(new string('-', 60));

            lock (_lock)
            {
                File.AppendAllText(logFile, sb.ToString(), Encoding.UTF8);
            }
        }
        catch
        {
            // Suppress I/O failures to prevent affecting active HTTP response pipelines
        }
    }
}
