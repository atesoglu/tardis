using Gateway.Endpoints.Mapper;
using Gateway.Setup.Middlewares;

namespace Gateway.Setup;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public WebApplication Configure()
        {
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseMiddleware<SecurityHeadersMiddleware>()
                .UseMiddleware<CorrelationIdMiddleware>();

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndpoints();

            return app;
        }
    }
}
