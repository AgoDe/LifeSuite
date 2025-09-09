using AutoMapper;
using BudgetManager.Data.Abstraction.Entities;
using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Abstractions.Models.DomainModels;
using BudgetManager.Data.Exceptions;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Data.Implementations.EntityFramework.Repositories
{
    public abstract class EFCrudRepository<T, TEntity, Filter, TForm> : ICrudRepository<T, Filter, TForm> 
        where T : class, IBaseDomainModel 
        where TEntity : class, IBaseEntity
        where Filter : class, IListFilter
        where TForm : class, IFormDto
    {
        protected DbSet<TEntity> dbSet { get; set; }
        protected readonly EFDbcontext _db;
        private readonly IMapper _mapper;

        public EFCrudRepository(EFDbcontext db, IMapper mapper)
        {
            _db = db;
            dbSet = _db.Set<TEntity>();
            _mapper = mapper;
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await GetEntityAsync(id);
            return entity != null ? _mapper.Map<T>(entity) : null;
        }

        public virtual async Task<TEntity?> GetEntityAsync(Guid id)
        {
            IQueryable<TEntity> query = dbSet;
            query = query.AsNoTracking();
            query = query.Where(x => x.Id == id);
            query = IncludeProperties(query);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<PaginatedList<T>> GetListAsync(Filter filters)
        {
            IQueryable<TEntity> query = dbSet;
            query = query.AsNoTracking();
            query = GetFilteredQuery(query, filters);
            query = GetOrderedQuery(query, filters.OrderBy, filters.Ascending);
            
            int count = await query.CountAsync();
            query = GetPagedQuery(query, filters);
            query = IncludePropertiesForList(query);
            
            var entities = await query.ToListAsync();
            var items = _mapper.Map<List<T>>(entities);
            
            PaginatedList<T> paged = new(items, filters.PageNumber, filters.PageSize, count);
            return paged;
        }

        public virtual async Task<Guid> CreateAsync(TForm model)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(model);
                // Usa il metodo helper per gestire il tracking
                DetachIfTracked(entity);
                
                await dbSet.AddAsync(entity);
                await SaveAsync();
                return entity.Id;
            }
            catch (Exception ex) 
            {
                throw ex.InnerException ?? ex;
            }
        }

        // Aggiungi questo metodo helper nella sezione #region Utility
        protected virtual void DetachIfTracked(TEntity entity)
        {
            var trackedEntity = _db.ChangeTracker.Entries<TEntity>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);
                
            if (trackedEntity != null)
            {
                trackedEntity.State = EntityState.Detached;
            }
        }
        
        public virtual async Task<IEnumerable<Guid>> CreateManyAsync(IEnumerable<TForm> domainModels)
        {
            try
            {
                var ids = new List<Guid>();
                var entities = new List<TEntity>();
                
                foreach (var domainModel in domainModels)
                {
                    var entity = _mapper.Map<TEntity>(domainModel);
                    entity.Id = Guid.NewGuid();
                    ids.Add(entity.Id);
                    entities.Add(entity);
                }
                
                await dbSet.AddRangeAsync(entities);
                await SaveAsync();
                return ids;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public virtual async Task UpdateAsync(Guid id, TForm model)
        {
            try
            {
                // Verifica se l'entità esiste nel database
                var existingEntity = await GetEntityAsync(id);
                if (existingEntity == null)
                {
                    throw new EntityNotFoundException($"Entity with ID {id} not found");
                }
                
                // Mappa il form DTO sull'entità esistente
                var entity = _mapper.Map(model, existingEntity);
                
                // Gestisci il tracking
                DetachIfTracked(entity);
                
                dbSet.Update(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task UpdateAsync(T domainModel)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(domainModel);
                
                // Gestisci il tracking anche per l'update
                DetachIfTracked(entity);
                
                dbSet.Update(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task UpdateEntityAsync(TEntity entity)
        {
            try
            {
                dbSet.Update(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public virtual async Task RemoveAsync(Guid id)
        {
            try
            {
                var entity = await GetEntityAsync(id);
                if (entity != null)
                {
                    dbSet.Remove(entity);
                    await SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task RemoveManyAsync(IEnumerable<Guid> ids)
        {
            try
            {
                var entities = await dbSet.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (entities.Any())
                {
                    dbSet.RemoveRange(entities);
                    await SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await dbSet.AnyAsync(x => x.Id == id);
        }

        public virtual async Task<int> CountAsync(Filter filters)
        {
            IQueryable<TEntity> query = dbSet;
            query = query.AsNoTracking();
            query = GetFilteredQuery(query, filters);
            return await query.CountAsync();
        }

        #region Utility
        
        protected virtual void DetachEntityById(Guid id)
        {
            var trackedEntity = _db.ChangeTracker.Entries<TEntity>()
                .FirstOrDefault(e => e.Entity.Id == id);
                
            if (trackedEntity != null)
            {
                trackedEntity.State = EntityState.Detached;
            }
        }
        protected virtual async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public bool ModelExists(Guid id)
        {
            return dbSet.Any(x => x.Id == id);
        }

        public bool ModelExists(T domainModel)
        {
            return dbSet.Any(x => x.Id == domainModel.Id);
        }

        protected virtual IQueryable<TEntity> GetDbSet()
        {
            return dbSet;
        }

        protected virtual IQueryable<TEntity> GetFilteredQuery(IQueryable<TEntity> query, Filter filter)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> GetOrderedQuery(IQueryable<TEntity> query, string orderBy, bool ascending = true)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> GetPagedQuery(IQueryable<TEntity> query, IListFilter filters)
        {
            // se il valore pageSize è 0 ritorna tutti i valori della query
            if (filters.PageSize == 0) return query;
            return query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize);
        }

        protected virtual IQueryable<TEntity> IncludeProperties(IQueryable<TEntity> query)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> IncludePropertiesForList(IQueryable<TEntity> query)
        {
            return query;
        }

        #endregion
    }
}