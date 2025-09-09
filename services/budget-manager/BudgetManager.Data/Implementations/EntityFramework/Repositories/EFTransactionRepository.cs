using AutoMapper;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using BudgetManager.Data.Implementations.EntityFramework.Repositories;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using SecondBrain.Data.Abstraction.Repositories;
using SharpCompress.Common;

namespace BudgetManager.Data.Implementations.EntityFramework.Repositories
{
    public class EFTransactionRepository : EFCrudRepository<TransactionDM, EFTransactionEntity, TransactionFilter, TransactionFormDto>, ITransactionRepository
    {
        
        private readonly IMapper _mapper;
        public EFTransactionRepository(EFDbcontext db, IMapper mapper) : base(db, mapper)
        {
            
        }

        public async Task<(Guid, Guid)> CreateTransferAsync(TransactionFormDto formDto)
        {
            var entity = new EFTransactionEntity
            {
                Description = formDto.Description,
                Date = formDto.Date,
                Type = formDto.Type,
                Amount = formDto.Amount,
                CategoryId = formDto.CategoryId,
                Notes = formDto.Notes,
                AccountId = formDto.AccountId,
                Status = formDto.Status,
                UserId = formDto.UserId
            };

            await _db.AddAsync(entity);
            await SaveAsync();

          
            var relatedEntity = new EFTransactionEntity
            {
                Description = formDto.Description,
                Date = formDto.Date,
                Type = formDto.Type == TransactionType.Income ? TransactionType.Expense : TransactionType.Income,
                Amount = formDto.Amount,
                CategoryId = formDto.CategoryId,
                Notes = formDto.Notes,
                AccountId = formDto.RelatedAccountId!.Value,
                Status = formDto.Status,
                UserId = formDto.UserId,
                RelatedTransactionId = entity.Id
            };

            await _db.AddAsync(relatedEntity);
            await SaveAsync();

            entity.RelatedTransactionId = relatedEntity.Id;
            _db.Update(entity);
            await SaveAsync();


            return (entity.Id, relatedEntity.Id);
        }


        #region utility
        protected override IQueryable<EFTransactionEntity> GetFilteredQuery(IQueryable<EFTransactionEntity> query, TransactionFilter filter)
        {

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(
                    q => q.Description.ToLower().Contains(filter.SearchText.ToLower())
                         || q.Notes.ToLower().Contains(filter.SearchText.ToLower())
                );
            }

            if (filter.AccountIds != null && filter.AccountIds.Any())
                query = query.Where(t => filter.AccountIds.Contains(t.AccountId));

            if(filter.AccountId != null)
                query = query.Where(t => t.AccountId ==  filter.AccountId);

            if (filter.AmountFrom != null)
                query = query.Where(t => t.Amount >= filter.AmountFrom);

            if (filter.AmountTo != null && filter.AmountTo > 0)
                query = query.Where(t => t.Amount <= filter.AmountTo);

            if (filter.DateFrom != null)
                query = query.Where(t => t.Date >= filter.DateFrom);

            if (filter.DateTo != null)
                query = query.Where(t => t.Date <= filter.DateTo);

            if (filter.Type != null && filter.Type != 0)
                query = query.Where(t => t.Type == filter.Type);

            if (filter.Status != null)
                query = query.Where(t => t.Status == filter.Status);

            if(filter.StatusFrom != null)
                query = query.Where(t => t.Status >= filter.StatusFrom);
            
            if (filter.StatusTo!= null)
                query = query.Where(t => t.Status <= filter.StatusTo);

            if(filter.CategoryId != null)
                query = query.Where(t => t.CategoryId == filter.CategoryId);
            
            if (filter.CategoryIds != null && filter.CategoryIds.Any())
                query = query.Where(t => filter.CategoryIds.Contains(t.CategoryId));

            if (!filter.IncludeTransfers)
                query = query.Where(t => t.RelatedTransactionId == null);

            if (!filter.IncludeRecurrings)
                query = query.Where(t => t.RecurringId == null); 

            return query;
        }

        protected override IQueryable<EFTransactionEntity> IncludeProperties(IQueryable<EFTransactionEntity> query)
        {
            return query
                //.AsSplitQuery()
                .Include(t => t.Category).ThenInclude(c => c.Parent)
                .Include(t => t.Account)
                .Include(t => t.RelatedTransaction);
        }

        protected override IQueryable<EFTransactionEntity> IncludePropertiesForList(IQueryable<EFTransactionEntity> query)
        {
            return query
                //.AsSplitQuery()
                .Include(t => t.Category).ThenInclude(c => c.Parent)
                .Include(t => t.Account)
                .Include(t => t.RelatedTransaction);
        }

        protected override IQueryable<EFTransactionEntity> GetOrderedQuery(IQueryable<EFTransactionEntity> query, string orderBy, bool ascending = true)
        {
            switch (orderBy)
            {
                case "description":
                    if (ascending)
                        query = query.OrderBy(q => q.Description);
                    else
                        query = query.OrderByDescending(q => q.Description);
                    break;

                default:
                    if (ascending)
                        query = query.OrderBy(q => q.Id);
                    else
                        query = query.OrderByDescending(q => q.Id);
                    break;
            }

            return query;
        }
        
        #endregion
    }
}
