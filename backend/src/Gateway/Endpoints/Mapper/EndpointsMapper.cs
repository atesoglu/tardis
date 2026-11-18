using Gateway.Setup.HealthChecks.Builder;

namespace Gateway.Endpoints.Mapper;

public static class EndpointsMapper
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/.ops/health", HealthCheckOptionsBuilder.Create()).AllowAnonymous();
        app.MapGet("/.ops/health/ready", () => Results.Ok("ready")).AllowAnonymous();

        app.MapFallback(() => Results.NotFound()).AllowAnonymous();

        app.MapGet("/bff/dashboard", DashboardEndpoint.Handle).RequireAuthorization();

        return app;
    }
}
