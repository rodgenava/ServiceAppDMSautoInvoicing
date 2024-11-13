using ServiceAppDMSautoInvoicing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<IAutoInvoicingService, AutoInvoicingService>();
        services.AddTransient<IDMSQueryService, DMSQueryService>();
        services.AddTransient<IAudilogsService, AudilogsService>();

        services.AddHostedService<Worker>();

        // Check if it's in development or production
        var env = context.HostingEnvironment;
        if (env.IsDevelopment())
        {
            // Development-specific services or configurations
        }
        else if (env.IsProduction())
        {
            // Production-specific services or configurations
        }
    })
    .Build();

await host.RunAsync();
