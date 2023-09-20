using Microsoft.EntityFrameworkCore;
using andis2_api_cuentas.Models;
using andis2_api_cuentas.Types;
using andis2_api_cuentas.Services;
using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;
using andis2_api_cuentas.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AccountContext>(opt => opt.UseInMemoryDatabase("AccountList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITaskQueue, TaskQueue>();
builder.Services.AddSingleton<IStatusService, StatusService>();
builder.Services.AddHostedService<TaskProcessingService>();

builder.Services.AddMediatR(cfg =>
     cfg.RegisterServicesFromAssembly(typeof(AccountDeposit).Assembly));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var accContext = scope.ServiceProvider.GetRequiredService<AccountContext>();
    accContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
