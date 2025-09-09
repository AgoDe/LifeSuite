using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Abstractions.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Interfaces;
using SecondBrain.Data.Abstraction.Repositories;

namespace BudgetManager.Data.Abstractions.Factories
{
    /// <summary>
    /// Interfaccia per la factory di repository
    /// Implementa il pattern Abstract Factory
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Crea un repository generico per il tipo specificato
        /// </summary>
        ICrudRepository<T, TFilter, TForm> CreateCrudRepository<T, TFilter, TForm>()
            where T : class, IBaseDomainModel
            where TFilter : class, IListFilter
            where TForm : class, IFormDto;
            
        /// <summary>
        /// Crea un repository specifico per Account
        /// </summary>
        IAccountRepository CreateAccountRepository();
        ITransactionRepository CreateTransactionRepository();
        ICategoryRepository CreateCategoryRepository();
        IRecurringRepository CreateRecurringRepository();
        
    }
}