using Microsoft.EntityFrameworkCore;
using andis2_api_cuentas.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AccountContext>(opt => opt.UseInMemoryDatabase("AccountList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddTokenBucketLimiter("TBRatelimiting", tbOptions =>
    {
        tbOptions.AutoReplenishment = true;
        tbOptions.QueueLimit = 0;
        tbOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        tbOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(60);
        tbOptions.TokensPerPeriod = 1;
        tbOptions.TokenLimit = 3;
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();
