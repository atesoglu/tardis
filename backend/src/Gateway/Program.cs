using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Auth (Entra ID)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["AzureAd:Instance"]
            + builder.Configuration["AzureAd:TenantId"];
        options.Audience = builder.Configuration["AzureAd:Audience"];
        options.TokenValidationParameters = new()
        {
            ValidIssuer = GetValidIssuer(builder.Configuration),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("react", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            policy.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// edgespace middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("react");

// security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("healthy")).AllowAnonymous();
app.MapGet("/ready", () => Results.Ok("ready")).AllowAnonymous();
app.MapFallback(() => Results.NotFound()).AllowAnonymous();

// BFF placeholder endpoint consumed by frontend
app.MapGet("/bff/dashboard", async (HttpContext ctx) =>
{
    var user = ctx.User.Identity?.Name ?? ctx.User.FindFirst("name")?.Value;
    return Results.Ok(new { user = user ?? "anonymous", prCount = 0 });
}).RequireAuthorization();

app.Run();

static string? GetValidIssuer(IConfiguration cfg)
{
    return cfg["AzureAd:Instance"] + cfg["AzureAd:TenantId"] + "/v2.0";
}