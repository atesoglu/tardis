using System.Diagnostics;

namespace Gateway.Setup.Middlewares;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private const string CorrelationIdTagName = "correlation_id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationIdHeaderValue = context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var headerValue) ? headerValue.ToString() : null;
        var correlationId = string.IsNullOrWhiteSpace(correlationIdHeaderValue)
            ? Guid.NewGuid().ToString("N")
            : correlationIdHeaderValue.Trim();

        context.Response.Headers[CorrelationIdHeaderName] = correlationId;

        if (Activity.Current != null)
        {
            Activity.Current.SetTag(CorrelationIdTagName, correlationId);
            Activity.Current.AddBaggage(CorrelationIdTagName, correlationId);
        }

        using var logScope = logger.BeginScope(new Dictionary<string, object?>(1)
        {
            [CorrelationIdTagName] = correlationId
        });

        await next(context);
    }
}
