using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Abstractions.Factories;
using BudgetManager.Data.Customizations;
using BudgetManager.Data.Implementations.EntityFramework;
using BudgetManager.Data.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetManager.Data
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddBudgetManagerData(
            this IServiceCollection services, 
            IConfiguration configuration, 
            DBImplementations implementation = DBImplementations.EntityFramework
            )
        {
            
            if (implementation == DBImplementations.EntityFramework)
            {
                // Configurazione Entity Framework
                services.AddEntityFrameworkImplementation(configuration);
            }
            

            // Registrazione servizi comuni
            services.AddScoped<DateService>();
            services.AddScoped<AccountService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<TransactionService>();
            services.AddScoped<RecurringService>();
            
            return services;
        }
    }
}

// mongodb atlar user PW: seh20xcnO4m7tuSZ