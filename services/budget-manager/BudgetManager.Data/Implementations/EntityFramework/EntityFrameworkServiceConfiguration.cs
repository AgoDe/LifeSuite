using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Abstractions.Factories;
using BudgetManager.Data.Implementations.EntityFramework.Factories;
using BudgetManager.Data.Implementations.EntityFramework.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetManager.Data.Implementations.EntityFramework;

public static class EntityFrameworkServiceConfiguration
{
    public static IServiceCollection AddEntityFrameworkImplementation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurazione AutoMapper per Entity Framework
        services.AddAutoMapper(typeof(EFMappingProfile));
        
        // Configurazione DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<EFDbcontext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging(); // Solo per development
            options.EnableDetailedErrors(); // Solo per development
        });
        
        // Registrazione Factory e Unit of Work
        services.AddScoped<IRepositoryFactory, EFRepositoryFactory>();
        services.AddScoped<IRepositoryUnitOfWork, EFRepositoryUnitOfWork>();
        
        return services;
    }
}