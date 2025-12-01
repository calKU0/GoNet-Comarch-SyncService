using GoNet_Comarch_SyncService;
using GoNet_Comarch_SyncService.Repositories;
using GoNet_Comarch_SyncService.Repositories.Interfaces;
using GoNet_Comarch_SyncService.Services;
using GoNet_Comarch_SyncService.Services.Interfaces;
using GoNet_Comarch_SyncService.Settings;
using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .UseSerilog((context, services, configuration) =>
    {
        var baseDir = AppContext.BaseDirectory;
        var logDir = Path.Combine(baseDir, "logs");
        Directory.CreateDirectory(logDir);

        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(logDir, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .Enrich.FromLogContext();
    })
    .ConfigureServices((context, services) =>
    {
        // Settings
        services.Configure<ErpApiSettings>(context.Configuration.GetSection("ErpApiSettings"));
        services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));

        // Repositories
        services.AddScoped<IClientRepository, ClientRepository>();

        // Services
        services.AddScoped<IErpApiClient, ErpApiClient>();
        services.AddScoped<IClientImportService, ClientImportService>();

        // Hosted Service
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();