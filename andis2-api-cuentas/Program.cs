using Microsoft.EntityFrameworkCore;
using andis2_api_cuentas.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Establish rate limiting.
builder.Services.AddRateLimiter(_ =>
{
    _.AddSlidingWindowLimiter("sliding", options =>
    {
        options.PermitLimit = 3;
        options.Window = TimeSpan.FromSeconds(12);
        options.SegmentsPerWindow = 3;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });

    _.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });

    _.AddTokenBucketLimiter("token", tbOptions =>
    {
        tbOptions.AutoReplenishment = true;
        tbOptions.QueueLimit = 0;
        tbOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        tbOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(60);
        tbOptions.TokensPerPeriod = 1;
        tbOptions.TokenLimit = 3;
    });
    
    _.AddConcurrencyLimiter(policyName: "concurrencyPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.QueueLimit = 100;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    _.RejectionStatusCode = 429;
    _.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("muchas llamadas, por favor prueba mas tarde ");
    };
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AccountContext>(opt => opt.UseInMemoryDatabase("AccountList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();
