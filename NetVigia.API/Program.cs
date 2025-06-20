using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using NetVigia.BLL.CommandHandler.TabelaGeral;
using NetVigia.BLL.Repository;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.BLL.Services;
using NetVigia.BLL.Services.Interfaces;
using NetVigia.Data;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UptimeContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("NetVigiaCon");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<ICheckService, HttpCheckService>();

builder.Services.AddHttpClient("UptimeChecker")
    .ConfigureAdditionalHttpMessageHandlers((action, service) => new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
    }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);

builder.Services.AddScoped<IIoTDBRepository, IoTDBRepository>();
builder.Services.AddScoped<IIoTDBService, IoTDBService>();

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
    cfg.RegisterServicesFromAssemblies(typeof(SaveTabelaGeralCommandHandler).Assembly));

var jwtSecret = builder.Configuration.GetSection("JwtSettings:Secret");

if (string.IsNullOrEmpty(jwtSecret.Value))
{
    throw new InvalidOperationException("JWT SECRET IS NOT SET");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret.Value)),
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Digite 'Bearer' [espaço] e seu token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference=new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header
       },
        new List<string>()
    }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(cors =>
    {
        cors.AllowAnyHeader();
        cors.AllowAnyMethod();
        cors.WithOrigins("http://localhost:5173", "https://localhost:44380");
        cors.AllowCredentials();
    });
}
else
{
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
