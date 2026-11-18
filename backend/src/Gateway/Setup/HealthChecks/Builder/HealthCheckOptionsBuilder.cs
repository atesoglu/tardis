using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Gateway.Setup.HealthChecks.Builder;

public static class HealthCheckOptionsBuilder
{
    public static HealthCheckOptions Create() => new() { ResponseWriter = ResponseWriter };

    private static async Task ResponseWriter(HttpContext context, HealthReport report)
    {
        await context.Response.WriteAsJsonAsync(new ReportResult(
            report.Status.ToString(),
            report.TotalDuration.TotalMilliseconds,
            report.Entries.ToDictionary(k => k.Key, v => new ReportEntryResult(v.Value.Status.ToString(), v.Value.Duration.TotalMilliseconds))
        ));
    }

    private sealed record ReportResult(string Status, double TotalDuration, Dictionary<string, ReportEntryResult> Entries);

    private sealed record ReportEntryResult(string Status, double Duration);
}
