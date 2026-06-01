using System.Diagnostics;

namespace CourseBank.api;

public static class RequestLoggingMiddleware
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            var sw = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = $"{context.Request.Path}{context.Request.QueryString}";

            await next();

            sw.Stop();
            Console.WriteLine(
                $"[{DateTime.Now:HH:mm:ss}] {method} {path} -> {context.Response.StatusCode} ({sw.ElapsedMilliseconds} ms)");
        });
}
