namespace Gateway.Endpoints;

public static class DashboardEndpoint
{
    public static async Task<IResult> Handle(HttpContext context, CancellationToken cancellationToken = default)
    {
        var user = context.User.Identity?.Name ?? context.User.FindFirst("name")?.Value;
        return Results.Ok(new { user = user ?? "anonymous (dev)", prCount = 0 });
    }
}
