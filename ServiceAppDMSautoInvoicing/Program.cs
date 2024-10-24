using ServiceAppDMSautoInvoicing;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IAutoInvoicingService, AutoInvoicingService>();
        services.AddTransient<IDMSQueryService, DMSQueryService>();
        services.AddTransient<IAudilogsService, AudilogsService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
