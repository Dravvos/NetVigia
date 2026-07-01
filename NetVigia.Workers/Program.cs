using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NetVigia.BLL.Command;
using NetVigia.BLL.Repository;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.BLL.Services;
using NetVigia.BLL.Services.Interfaces;
using NetVigia.Data;
using NetVigia.Workers.Workers;
using NetVigia.Workers.Workers.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Core;
using System.Security.Cryptography.X509Certificates;

namespace NetVigia.Workers
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            try
            {
                Logger logger = CreateSerilogMongoDBLogger();

                var host = Host.CreateDefaultBuilder(args).ConfigureHostOptions(options =>
                {
                    options.ShutdownTimeout = TimeSpan.FromSeconds(10);
                })
                    .ConfigureServices((context, services) =>
                {

                    services.AddSerilog(logger);

                    services.AddScoped<ICheckService, HttpCheckService>();

                    services.AddSingleton<ICheckScheduler, CheckScheduler>();
                    services.AddHostedService<CheckOrchestratorWorker>();

                    services.AddScoped<IIntegrationRepository, IntegrationRepository>();
                    services.AddScoped<IIntegrationService, IntegrationService>();

                    services.AddScoped<IIoTDBRepository, IoTDBRepository>();
                    services.AddScoped<IIoTDBService, IoTDBService>();

                    services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
                    services.AddScoped<IMaintenanceService, MaintenanceService>();

                    services.AddScoped<IServerRepository, ServerRepository>();
                    services.AddScoped<IServerService, ServerService>();

                    services.AddScoped<ITabelaGeralRepository, TabelaGeralRepository>();
                    services.AddScoped<ITabelaGeralService, TabelaGeralService>();

                    services.AddScoped<ITabelaGeralItemRepository, TabelaGeralItemRepository>();
                    services.AddScoped<ITabelaGeralItemService, TabelaGeralItemService>();

                    services.AddDbContext<UptimeContext>(options =>
                        options.UseNpgsql(Environment.GetEnvironmentVariable("NetVigiaCon")));

                    // Configurar serviços
                    services.AddHttpClient("UptimeChecker")
                    .ConfigureAdditionalHttpMessageHandlers((action, service) => new SocketsHttpHandler
                    {
                        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
                        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                    }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);

                    services.AddMediatR(cfg =>
                        cfg.RegisterServicesFromAssembly(typeof(AddSiteCheckCommand).Assembly));

                    // Adicionar serviço para controlar tempo de vida do console
                    services.AddHostedService<ConsoleHostedService>();

                    // Registrar workers
                    services.AddHostedService<CheckOrchestratorWorker>();
                })
                .Build();


                await host.RunAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Fatal failure: {ex.Message}");
            }
            finally
            {
                await Task.Delay(500);
            }
        }

        private static Logger CreateSerilogMongoDBLogger()
        {
            var mongoConnectionString = Environment.GetEnvironmentVariable("MongoDBConnection");
            var settings = MongoClientSettings.FromUrl(new MongoUrl(mongoConnectionString));

            var client = new MongoClient(settings);

            var database = client.GetDatabase("NetVigiaLogs");

            var logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.MongoDBCapped(database, collectionName: "Logs", cappedMaxSizeMb: 100, cappedMaxDocuments: 50000)
                .CreateLogger();

            return logger;
        }
    }
}
