using AutoMapper;
using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Abstraction.Services;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Abstraction.Validation;
using BudgetManager.Data.Abstractions.Models.DomainModels;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Interfaces;
using BudgetManager.Data.Exceptions;

namespace BudgetManager.Data.Services
{
    /// <summary>
    /// Classe astratta base per l'implementazione di servizi CRUD
    /// Fornisce implementazione comune mantenendo il disaccoppiamento
    /// </summary>
    /// <typeparam name="T">Tipo dell'entità che implementa IBaseEntity</typeparam>
    /// <typeparam name="TDto">Tipo del DTO che implementa IBaseDto</typeparam>
    /// <typeparam name="TFormDto">Tipo del DTO per form che implementa IFormDto</typeparam>
    /// <typeparam name="TFilter">Tipo del filtro per le query di lista</typeparam>
    public abstract class BaseCrudService<T, TDto, TFormDto, TFilter> : ICrudService<TDto, TFormDto, TFilter>
        where T : class, IBaseDomainModel
        where TDto : class, IBaseDto
        where TFormDto : class, IFormDto
        where TFilter : class, IListFilter
    {
        protected readonly IRepositoryUnitOfWork _unitOfWork;
        protected readonly ICrudRepository<T, TFilter, TFormDto> _repository;
        protected readonly IMapper _mapper;

        protected BaseCrudService(
            IRepositoryUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _repository = _unitOfWork.GetCrudRepository<T, TFilter, TFormDto>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Recupera un DTO per ID con validazione business
        /// </summary>
        public virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<TDto>(entity) : null;
        }

        /// <summary>
        /// Recupera una lista paginata di DTO con filtri
        /// </summary>
        public virtual async Task<PaginatedList<TDto>> GetListAsync(TFilter filters)
        {
            // Validazione filtri se necessario
            await ValidateFiltersAsync(filters);

            var entityList = await _repository.GetListAsync(filters);
            var dtoList = _mapper.Map<List<TDto>>(entityList.Items);

            return new PaginatedList<TDto>(dtoList, entityList.Pagination);
        }

        /// <summary>
        /// Crea una nuova entità da FormDTO
        /// </summary>
        public virtual async Task<Guid> CreateAsync(TFormDto formDto)
        {
            if (formDto == null)
                throw new ArgumentNullException(nameof(formDto));

            // Validazione business
            var validationResult = await ValidateWithDetailsAsync(formDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Validazioni specifiche pre-creazione
            await ValidateForCreateAsync(formDto);

            // Conversione e creazione
            //var entity = _mapper.Map<T>(formDto);
            //await BeforeCreateAsync(entity, formDto);
            
            var id = await _repository.CreateAsync(formDto);
            
            //await AfterCreateAsync(entity, formDto, id);
            return id;
        }

        /// <summary>
        /// Aggiorna un'entità esistente da FormDTO
        /// </summary>
        public virtual async Task UpdateAsync(Guid id, TFormDto formDto)
        {
            if (formDto == null)
                throw new ArgumentNullException(nameof(formDto));

            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty for update operation", nameof(id));

            // Verifica esistenza
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                throw new EntityNotFoundException($"Entity with ID {id} not found");

            // Validazione business
            var validationResult = await ValidateWithDetailsAsync(formDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Validazioni specifiche pre-aggiornamento
            await ValidateForUpdateAsync(id, formDto, existingEntity);

            // Aggiornamento
            _mapper.Map(formDto, existingEntity);
            await BeforeUpdateAsync(existingEntity, formDto);
            
            await _repository.UpdateAsync(existingEntity);
            
            await AfterUpdateAsync(existingEntity, formDto);
        }

        /// <summary>
        /// Rimuove un'entità per ID
        /// </summary>
        public virtual async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new EntityNotFoundException($"Entity with ID {id} not found");

            // Validazioni business per la cancellazione
            await ValidateForDeleteAsync(entity);

            await BeforeDeleteAsync(entity);
            await _repository.RemoveAsync(id);
            await AfterDeleteAsync(entity);
        }

        /// <summary>
        /// Verifica se un'entità esiste
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return id != Guid.Empty && await _repository.ExistsAsync(id);
        }

        /// <summary>
        /// Conta il numero totale di entità
        /// </summary>
        public virtual async Task<int> CountAsync(TFilter filters)
        {
            await ValidateFiltersAsync(filters);
            return await _repository.CountAsync(filters);
        }

        /// <summary>
        /// Operazioni batch - Creazione multipla
        /// </summary>
        public virtual async Task<IEnumerable<Guid>> CreateManyAsync(IEnumerable<TFormDto> formDtos)
        {
            if (formDtos == null || !formDtos.Any())
                return Enumerable.Empty<Guid>();

            var formDtoList = formDtos.ToList();
            
            // Validazione di tutti i FormDTO
            foreach (var formDto in formDtoList)
            {
                var validationResult = await ValidateWithDetailsAsync(formDto);
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);
                
                await ValidateForCreateAsync(formDto);
            }

            //var entities = _mapper.Map<List<T>>(formDtoList);
            
            //await BeforeCreateManyAsync(entities, formDtoList);
            var ids = await _repository.CreateManyAsync(formDtos);
            //await AfterCreateManyAsync(entities, formDtoList, ids);
            
            return ids;
        }
        

        /// <summary>
        /// Operazioni batch - Cancellazione multipla
        /// </summary>
        public virtual async Task DeleteManyAsync(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return;

            var idList = ids.Where(id => id != Guid.Empty).ToList();
            if (!idList.Any())
                return;

            var entities = new List<T>();
            foreach (var id in idList)
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    throw new EntityNotFoundException($"Entity with ID {id} not found");
                
                await ValidateForDeleteAsync(entity);
                entities.Add(entity);
            }

            await BeforeDeleteManyAsync(entities);
            await _repository.RemoveManyAsync(idList);
            await AfterDeleteManyAsync(entities);
        }

        /// <summary>
        /// Validazione semplice di un DTO
        /// </summary>
        public virtual async Task<bool> ValidateAsync(TFormDto dto)
        {
            var result = await ValidateWithDetailsAsync(dto);
            return result.IsValid;
        }

        /// <summary>
        /// Validazione dettagliata di un DTO
        /// </summary>
        public virtual async Task<ValidationResult> ValidateWithDetailsAsync(TFormDto dto)
        {
            if (dto == null)
                return ValidationResult.Failure(new ValidationError("dto", "DTO cannot be null"));

            var result = new ValidationResult { IsValid = true };

            // Validazioni base
            await ValidateBasePropertiesAsync(dto, result);
            
            // Validazioni business specifiche
            await ValidateBusinessRulesAsync(dto, result);

            return result;
        }

        #region Metodi Virtuali per Estensioni Specifiche

        /// <summary>
        /// Validazione dei filtri - può essere sovrascritto
        /// </summary>
        protected virtual Task ValidateFiltersAsync(TFilter filters)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validazioni specifiche per la creazione
        /// </summary>
        protected virtual Task ValidateForCreateAsync(TFormDto dto)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validazioni specifiche per l'aggiornamento
        /// </summary>
        protected virtual Task ValidateForUpdateAsync(Guid id, TFormDto dto, T existingEntity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validazioni specifiche per la cancellazione
        /// </summary>
        protected virtual Task ValidateForDeleteAsync(T entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validazione delle proprietà base del DTO
        /// </summary>
        protected virtual Task ValidateBasePropertiesAsync(TFormDto dto, ValidationResult result)
        {
            // Implementazione base - può essere estesa
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validazioni delle regole business specifiche
        /// </summary>
        protected virtual Task ValidateBusinessRulesAsync(TFormDto dto, ValidationResult result)
        {
            // Da implementare nelle classi derivate
            return Task.CompletedTask;
        }

        #endregion

        #region Hook Methods per Estensioni

        protected virtual Task BeforeCreateAsync(T entity, TFormDto dto) => Task.CompletedTask;
        protected virtual Task AfterCreateAsync(T entity, TFormDto dto, Guid id) => Task.CompletedTask;
        
        protected virtual Task BeforeUpdateAsync(T entity, TFormDto dto) => Task.CompletedTask;
        protected virtual Task AfterUpdateAsync(T entity, TFormDto dto) => Task.CompletedTask;
        
        protected virtual Task BeforeDeleteAsync(T entity) => Task.CompletedTask;
        protected virtual Task AfterDeleteAsync(T entity) => Task.CompletedTask;
        
        protected virtual Task BeforeCreateManyAsync(List<T> entities, List<TFormDto> dtos) => Task.CompletedTask;
        protected virtual Task AfterCreateManyAsync(List<T> entities, List<TFormDto> dtos, IEnumerable<Guid> ids) => Task.CompletedTask;
        
        protected virtual Task BeforeUpdateManyAsync(List<T> entities, List<TFormDto> dtos) => Task.CompletedTask;
        protected virtual Task AfterUpdateManyAsync(List<T> entities, List<TFormDto> dtos) => Task.CompletedTask;
        
        protected virtual Task BeforeDeleteManyAsync(List<T> entities) => Task.CompletedTask;
        protected virtual Task AfterDeleteManyAsync(List<T> entities) => Task.CompletedTask;

        #endregion
    }
}