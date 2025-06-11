using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetVigia.API.Extensions;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Repository;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.BLL.Service;
using NetVigia.BLL.Command;
using MediatR;
using NetVigia.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UptimeContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("NetVigiaCon");
    options.UseNpgsql(connectionString);
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<IRequest>());


builder.Services.AddSingleton<IIoTDBRepository, IoTDBRepository>();
builder.Services.AddSingleton<IIoTDBService, IoTDBService>();

builder.Services.AddScoped<IServerRepository, ServerRepository>();
builder.Services.AddScoped<IServerService, ServerService>();

builder.Services.AddScoped<ITabelaGeralRepository, TabelaGeralRepository>();
builder.Services.AddScoped<ITabelaGeralService, TabelaGeralService>();

builder.Services.AddScoped<ITabelaGeralItemRepository, TabelaGeralItemRepository>();
builder.Services.AddScoped<ITabelaGeralItemService, TabelaGeralItemService>();

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
