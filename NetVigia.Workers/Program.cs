using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NetVigia.BLL.CommandHandler;
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
using System.Security.Cryptography.X509Certificates;

var mongoConnectionString = Environment.GetEnvironmentVariable("MongoDBConnection");
var settings = MongoClientSettings.FromUrl(new MongoUrl(mongoConnectionString));

var caCert = new X509Certificate2(@"C:\Users\supero\mongodb-ca.crt");
var clientCert = new X509Certificate2(@"C:\Users\supero\mongodb-client.pfx", "YqY,&soTB_fQ!r5#",
     X509KeyStorageFlags.MachineKeySet |
X509KeyStorageFlags.PersistKeySet |
X509KeyStorageFlags.Exportable);

settings.SslSettings = new SslSettings
{
    ClientCertificates = new List<X509Certificate> { clientCert },
    ServerCertificateValidationCallback = (sender, cert, chain, errors) => true
};

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

Log.Logger = logger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UptimeContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("NetVigiaCon");
    options.UseNpgsql(connectionString);
});

builder.Logging.ClearProviders();

builder.Services.AddSerilog(logger);

builder.Services.AddScoped<ICheckService, HttpCheckService>();

builder.Services.AddSingleton<ICheckScheduler, CheckScheduler>();
builder.Services.AddHostedService<CheckOrchestratorWorker>();

builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();
builder.Services.AddScoped<IIntegrationService, IntegrationService>();

builder.Services.AddScoped<IIoTDBRepository, IoTDBRepository>();
builder.Services.AddScoped<IIoTDBService, IoTDBService>();

builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();

builder.Services.AddScoped<IServerRepository, ServerRepository>();
builder.Services.AddScoped<IServerService, ServerService>();

builder.Services.AddScoped<ITabelaGeralRepository, TabelaGeralRepository>();
builder.Services.AddScoped<ITabelaGeralService, TabelaGeralService>();

builder.Services.AddScoped<ITabelaGeralItemRepository, TabelaGeralItemRepository>();
builder.Services.AddScoped<ITabelaGeralItemService, TabelaGeralItemService>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<IRequest>());

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(AddSiteCheckCommandHandler).Assembly));


builder.Services.AddHttpClient("UptimeChecker")
    .ConfigureAdditionalHttpMessageHandlers((action, service) => new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
    }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();