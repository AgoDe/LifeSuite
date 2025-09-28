using BudgetManager.Api.Middleware;
using BudgetManager.Api.Models;
using BudgetManager.Data;
using BudgetManager.Data.Implementations.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBudgetManagerData(builder.Configuration);
builder.Services.AddScoped<UserContext>();
builder.Services.AddControllers();

var app = builder.Build();

// Applica le migration al database all'avvio
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EFDbcontext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<UserContextMiddleware>();

// Solo in produzione usiamo HTTPS redirect
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


app.MapControllers();

app.Run();
