using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Abstraction.Validation;
using BudgetManager.Data.Exceptions;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Abstraction.Services
{
    /// <summary>
    /// Interfaccia astratta per servizi CRUD che lavorano con DTO
    /// Completamente disaccoppiata dalle implementazioni di repository e database
    /// Fornisce un layer di business logic con trasferimento dati tramite DTO
    /// </summary>
    /// <typeparam name="TDto">Tipo del DTO che implementa IBaseDto</typeparam>
    /// <typeparam name="TFormDto">Tipo del DTO per form che implementa IFormDto</typeparam>
    /// <typeparam name="TFilter">Tipo del filtro per le query di lista</typeparam>
    public interface ICrudService<TDto, TFormDto, TFilter> 
        where TDto : class, IBaseDto 
        where TFormDto : class, IFormDto
        where TFilter : class, IListFilter
    {
        /// <summary>
        /// Recupera un DTO per ID con validazione business
        /// </summary>
        /// <param name="id">ID dell'entità da recuperare</param>
        /// <returns>Il DTO se trovato, null altrimenti</returns>
        Task<TDto?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Recupera una lista paginata di DTO con filtri e validazioni
        /// </summary>
        /// <param name="filters">Filtri da applicare alla query</param>
        /// <returns>Lista paginata di DTO</returns>
        Task<PaginatedList<TDto>> GetListAsync(TFilter filters);
        
        /// <summary>
        /// Crea una nuova entità da FormDTO con validazioni business
        /// </summary>
        /// <param name="formDto">FormDTO da convertire e salvare</param>
        /// <returns>ID dell'entità creata</returns>
        /// <exception cref="ValidationException">Se il FormDTO non è valido</exception>
        Task<Guid> CreateAsync(TFormDto formDto);
        
        /// <summary>
        /// Aggiorna un'entità esistente da FormDTO con validazioni business
        /// </summary>
        /// <param name="id">ID dell'entità da aggiornare</param>
        /// <param name="formDto">FormDTO con i dati aggiornati</param>
        /// <exception cref="ValidationException">Se il FormDTO non è valido</exception>
        /// <exception cref="EntityNotFoundException">Se l'entità non esiste</exception>
        Task UpdateAsync(Guid id, TFormDto formDto);
        
        /// <summary>
        /// Rimuove un'entità per ID con controlli business
        /// </summary>
        /// <param name="id">ID dell'entità da rimuovere</param>
        /// <exception cref="EntityNotFoundException">Se l'entità non esiste</exception>
        /// <exception cref="BusinessRuleException">Se la rimozione viola regole business</exception>
        Task DeleteAsync(Guid id);
        
        /// <summary>
        /// Verifica se un'entità esiste
        /// </summary>
        /// <param name="id">ID dell'entità da verificare</param>
        /// <returns>True se l'entità esiste, false altrimenti</returns>
        Task<bool> ExistsAsync(Guid id);
        
        /// <summary>
        /// Conta il numero totale di entità che soddisfano i filtri
        /// </summary>
        /// <param name="filters">Filtri da applicare al conteggio</param>
        /// <returns>Numero totale di entità</returns>
        Task<int> CountAsync(TFilter filters);
        
        /// <summary>
        /// Operazioni batch per performance migliori con validazioni business
        /// </summary>
        /// <param name="formDtos">Collezione di FormDTO da convertire e salvare</param>
        /// <returns>Lista degli ID delle entità create</returns>
        /// <exception cref="ValidationException">Se uno o più FormDTO non sono validi</exception>
        Task<IEnumerable<Guid>> CreateManyAsync(IEnumerable<TFormDto> formDtos);
        
        /// <summary>
        /// Rimuove multiple entità con controlli business
        /// </summary>
        /// <param name="ids">Collezione di ID delle entità da rimuovere</param>
        /// <exception cref="BusinessRuleException">Se la rimozione viola regole business</exception>
        Task DeleteManyAsync(IEnumerable<Guid> ids);
        
        /// <summary>
        /// Valida un FormDTO secondo le regole business
        /// </summary>
        /// <param name="formDto">FormDTO da validare</param>
        /// <returns>True se il FormDTO è valido, false altrimenti</returns>
        Task<bool> ValidateAsync(TFormDto formDto);
        
        /// <summary>
        /// Valida un FormDTO e restituisce i dettagli degli errori
        /// </summary>
        /// <param name="formDto">FormDTO da validare</param>
        /// <returns>Risultato della validazione con eventuali errori</returns>
        Task<ValidationResult> ValidateWithDetailsAsync(TFormDto formDto);
    }
}