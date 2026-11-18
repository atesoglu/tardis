using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Gateway.Setup.HealthChecks;

public sealed class EmptyHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Healthy."));
    }
}
