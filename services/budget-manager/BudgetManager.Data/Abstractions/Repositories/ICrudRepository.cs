using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Abstractions.Models.DomainModels;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Abstraction.Repositories
{
    /// <summary>
    /// Interfaccia astratta per operazioni CRUD su entità
    /// Completamente database-agnostic
    /// </summary>
    /// <typeparam name="T">Tipo dell'entità che implementa IBaseEntity</typeparam>
    /// <typeparam name="TFilter">Tipo del filtro per le query di lista</typeparam>
    public interface ICrudRepository<T, TFilter, TForm> 
        where T : class, IBaseDomainModel
        where TFilter : class, IListFilter
        where TForm : class, IFormDto
    {
        /// <summary>
        /// Recupera un'entità per ID
        /// </summary>
        Task<T?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Recupera una lista paginata di entità con filtri
        /// </summary>
        Task<PaginatedList<T>> GetListAsync(TFilter filters);
        
        /// <summary>
        /// Crea una nuova entità
        /// </summary>
        Task<Guid> CreateAsync(TForm model);
        
        /// <summary>
        /// Aggiorna un'entità esistente usando Form DTO
        /// </summary>
        Task UpdateAsync(Guid id, TForm entity);
        
        /// <summary>
        /// Aggiorna un'entità esistente usando Domain Model
        /// </summary>
        Task UpdateAsync(T domainModel);
        
        /// <summary>
        /// Rimuove un'entità per ID
        /// </summary>
        Task RemoveAsync(Guid id);
        
        /// <summary>
        /// Verifica se un'entità esiste
        /// </summary>
        Task<bool> ExistsAsync(Guid id);
        
        /// <summary>
        /// Conta il numero totale di entità che soddisfano i filtri
        /// </summary>
        Task<int> CountAsync(TFilter filters);
        
        /// <summary>
        /// Operazioni batch per performance migliori
        /// </summary>
        Task<IEnumerable<Guid>> CreateManyAsync(IEnumerable<TForm> entities);
        //Task UpdateManyAsync(IEnumerable<T> entities);
        Task RemoveManyAsync(IEnumerable<Guid> ids);
    }
}