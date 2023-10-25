using Microsoft.EntityFrameworkCore;
using andis2_api_cuentas.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AccountContext>(opt => opt.UseInMemoryDatabase("AccountList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; // Too many request
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("muchas llamadas, por favor prueba mas tarde ");
    };
    options.AddConcurrencyLimiter(policyName: "concurrencyPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.QueueLimit = 100;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    });
}

app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
