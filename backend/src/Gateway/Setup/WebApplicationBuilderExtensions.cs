using Gateway.Setup.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Setup;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        AddProblemDetails(builder);
        AddAuthenticationAndAuthorization(builder);
        AddCors(builder);
        AddHealthChecks(builder);

        return builder;
    }

    private static void AddHealthChecks(WebApplicationBuilder builder)
    {
        var healthChecks = builder.Services.AddHealthChecks();
        if (builder.Environment.IsDevelopment())
        {
            healthChecks.AddCheck<EmptyHealthCheck>("development");
        }
        else
        {
            //healthChecks.AddCheck<EventHubHealthCheck>("event-hub").AddCheck<RedisHealthCheck>("redis").AddCheck<PostgresHealthCheck>("postgres");
        }
    }

    private static void AddCors(WebApplicationBuilder builder)
    {
        builder.Services
            .AddCors(cors => cors.AddDefaultPolicy(policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? []);
                }

                policy.AllowAnyHeader().AllowAnyMethod();
            }));
    }

    private static void AddAuthenticationAndAuthorization(WebApplicationBuilder builder)
    {
        var instance = builder.Configuration["Azure:Ad:Instance"] ?? throw new InvalidOperationException("Azure:Ad:Instance is required.");
        var tenantId = builder.Configuration["Azure:Ad:TenantId"] ?? throw new InvalidOperationException("Azure:Ad:TenantId is required.");
        var audience = builder.Configuration["Azure:Ad:Audience"] ?? throw new InvalidOperationException("Azure:Ad:Audience is required.");
        var authority = $"{instance.TrimEnd('/')}/{tenantId}";

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = $"{authority}/v2.0"
                };
            });

        builder.Services
            .AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }

    private static void AddProblemDetails(WebApplicationBuilder builder)
    {
        builder.Services
            .AddProblemDetails(options => options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["path"] = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["trace-identifier"] = context.HttpContext.TraceIdentifier;

                if (builder.Environment.IsProduction())
                {
                    context.ProblemDetails.Detail = "The server encountered an internal error and was unable to complete your request.";
                    return;
                }

                var exception = context.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
                context.ProblemDetails.Detail = exception?.Message;
                context.ProblemDetails.Extensions["stack-trace"] = exception?.StackTrace;
                context.ProblemDetails.Extensions["inner-exception"] = exception?.InnerException?.Message;
            });
    }
}
