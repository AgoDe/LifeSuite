using BudgetManager.Data.Abstraction.Services;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Abstractions.Factories;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using BudgetManager.Data.Implementations.EntityFramework.Factories;
using BudgetManager.Data.Implementations.EntityFramework.Services;
using BudgetManager.Data.Implementations.EntityFramework.UnitOfWork;
using BudgetManager.Data.Services;
using Microsoft.AspNetCore.Identity;
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
        services
            .AddIdentity<EFUserEntity, EFRoleEntity>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<EFRoleEntity>()
            .AddRoleManager<RoleManager<EFRoleEntity>>()
            //.AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
            .AddEntityFrameworkStores<EFDbcontext>()
            .AddTokenProvider<DataProtectorTokenProvider<EFUserEntity>>(TokenOptions.DefaultProvider);
        
        // Registrazione Factory e Unit of Work
        services.AddScoped<IRepositoryFactory, EFRepositoryFactory>();
        services.AddScoped<IRepositoryUnitOfWork, EFRepositoryUnitOfWork>();
        
        // Identity Service
        services.AddScoped<IIdentityService, EFIdentityService>();
        services.AddHttpContextAccessor(); // Necessario per GetCurrentUserAsync
        
        return services;
    }
}