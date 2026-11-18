using Gateway.Setup;

WebApplication
    // WebApplicationBuilder
    .CreateBuilder(args)
    .AddServices()
    // WebApplication
    .Build()
    .Configure()
    .Run();
