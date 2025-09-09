using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Models.Dto.Interfaces;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Abstractions.Models.DomainModels;
using SecondBrain.Data.Abstraction.Repositories;

namespace BudgetManager.Data.Abstraction.UnitOfWork
{
    /// <summary>
    /// Interfaccia per Unit of Work
    /// Gestisce transazioni e accesso ai repository
    /// </summary>
    public interface IRepositoryUnitOfWork : IDisposable
    {
        /// <summary>
        /// Ottiene un repository generico per il tipo specificato
        /// </summary>
        ICrudRepository<T, TFilter, TForm> GetCrudRepository<T, TFilter, TForm>()
            where T : class, IBaseDomainModel
            where TFilter : class, IListFilter
            where TForm : class, IFormDto;
        
            
        /// <summary>
        /// Repository specifico per Account
        /// </summary>
        IAccountRepository AccountRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IRecurringRepository recurringRepository { get; }
        
        /// <summary>
        /// Inizia una transazione
        /// </summary>
        Task BeginTransactionAsync();
        
        /// <summary>
        /// Conferma le modifiche della transazione corrente
        /// </summary>
        Task CommitAsync();
        
        /// <summary>
        /// Annulla le modifiche della transazione corrente
        /// </summary>
        Task RollbackAsync();
        
        /// <summary>
        /// Salva tutte le modifiche senza utilizzare una transazione esplicita
        /// </summary>
        Task SaveChangesAsync();
    }
}