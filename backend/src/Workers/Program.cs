using Microsoft.Extensions.Hosting;

var host = Host.CreateApplicationBuilder(args)
    .Build();
host.Run();