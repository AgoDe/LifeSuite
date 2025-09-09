using AutoMapper;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Models.Enums;
using BudgetManager.Data.Services;
using Microsoft.EntityFrameworkCore;
using SecondBrain.Data.Abstraction.Repositories;
using System.Linq.Expressions;

namespace BudgetManager.Data.Implementations.EntityFramework.Repositories
{
    public class EFRecurringRepository : EFCrudRepository<RecurringDM, EFRecurringEntity, RecurringFilter, RecurringFormDto>, IRecurringRepository
    {
        
        private readonly IMapper _mapper;
        private readonly DateService _dateService;
        public EFRecurringRepository(EFDbcontext db, IMapper mapper, DateService dateService) : base(db, mapper)
        {
            _dateService = dateService;
            _mapper = mapper;
        }

        public async Task<(Guid, Guid)> CreateTransferRecurring(RecurringFormDto formDto)
        {
            var activeToDate = GetActiveToDate(formDto);
            var chargeDay = GetChargeDay(formDto);

            var entity = new EFRecurringEntity
            {
                Description = formDto.Description,
                Institution = formDto.Institution,
                Notes = formDto.Notes,
                ActiveFrom = formDto.ActiveFrom,
                ActiveTo = activeToDate,
                ChargeDay = chargeDay,
                Type = formDto.Type,
                Amount = formDto.Amount,
                CategoryId = formDto.CategoryId,
                AccountId = formDto.AccountId,
                UserId = formDto.UserId,
            };

            await _db.AddAsync(entity);
            await SaveAsync();

            var relatedEntity = new EFRecurringEntity
            {
                Description = formDto.Description,
                Institution = formDto.Institution,
                Notes = formDto.Notes,
                ActiveFrom = formDto.ActiveFrom,
                ActiveTo = activeToDate,
                ChargeDay = chargeDay,
                Type = formDto.Type == TransactionType.Income ? TransactionType.Expense : TransactionType.Income,
                Amount = formDto.Amount,
                CategoryId = formDto.CategoryId,
                AccountId = formDto.RelatedAccountId!.Value,
                UserId = formDto.UserId,
                RelatedRecurringId = entity.Id
            };

            await _db.AddAsync(relatedEntity);
            await SaveAsync();

            entity.RelatedRecurringId = relatedEntity.Id;
            _db.Update(entity);
            await SaveAsync();


            return (entity.Id, relatedEntity.Id);
        }

        #region utility

        private Expression<Func<EFRecurringEntity, bool>> IsNotActiveExpression => t => t.ActiveFrom > _dateService.CurrentMonthStartDate || (t.ActiveTo != null && t.ActiveTo < _dateService.CurrentMonthStartDate);
        private Expression<Func<EFRecurringEntity, bool>> IsActiveExpression => t => t.ActiveFrom <= _dateService.CurrentMonthEndDate && (t.ActiveTo == null || t.ActiveTo >= _dateService.CurrentMonthStartDate);
        protected override IQueryable<EFRecurringEntity> GetFilteredQuery(IQueryable<EFRecurringEntity> query, RecurringFilter filter)
        {

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(
                    q => q.Description.ToLower().Contains(filter.SearchText.ToLower())
                         || q.Category.Name.ToLower().Contains(filter.SearchText.ToLower())
                         || q.Notes.ToLower().Contains(filter.SearchText.ToLower())
                         || q.Institution.ToLower().Contains(filter.SearchText.ToLower())
                );
            }

            if (filter.AmountFrom != null)
                query = query.Where(t => t.Amount >= filter.AmountFrom);

            if (filter.AmountTo != null)
                query = query.Where(t => t.Amount <= filter.AmountTo);

            if (filter.IsActive != null)
            {

                if ((bool)filter.IsActive)
                {
                    query = query.Where(IsActiveExpression);
                }
                else
                {
                    query = query.Where(IsNotActiveExpression);
                }
            }

            if (filter.Type != null && filter.Type != 0)
                query = query.Where(t => t.Type == filter.Type);

            if (filter.AccountId.HasValue)
                query = query.Where(t => t.AccountId == filter.AccountId);

            if (filter.AccountIds != null && filter.AccountIds.Any())
                query = query.Where(t => filter.AccountIds.Contains(t.AccountId));

            if (filter.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == filter.CategoryId);

            if (filter.CategoryIds != null && filter.CategoryIds.Any())
                query = query.Where(t => filter.CategoryIds.Contains(t.CategoryId));
            
            if (filter.ActiveFrom != null || filter.ActiveTo != null)
            {
                var filterFrom = filter.ActiveFrom ?? DateTime.MinValue;
                var filterTo = filter.ActiveTo ?? DateTime.MaxValue;
        
                query = query.Where(x => 
                    // Il recurring è attivo se c'è sovrapposizione tra i due range
                    x.ActiveFrom <= filterTo &&  x.ActiveTo >= filterFrom
                );
            }
            
            return query;
        }

        protected override IQueryable<EFRecurringEntity> IncludeProperties(IQueryable<EFRecurringEntity> query)
        {
            return query
                .Include(x => x.Account)
                .Include(x => x.Category).ThenInclude(x => x.Parent);
        }

        protected override IQueryable<EFRecurringEntity> IncludePropertiesForList(IQueryable<EFRecurringEntity> query)
        {
            return query
                .Include(x => x.Account)
                .Include(x => x.Category).ThenInclude(x => x.Parent)
                .Include(x => x.RelatedRecurring);
        }

        protected override IQueryable<EFRecurringEntity> GetOrderedQuery(IQueryable<EFRecurringEntity> query, string orderBy, bool ascending = true)
        {
            switch (orderBy)
            {
                case "description":
                    if (ascending)
                        query = query.OrderBy(q => q.Description);
                    else
                        query = query.OrderByDescending(q => q.Description);
                    break;

                case "amount":
                    if (ascending)
                        query = query.OrderBy(q => q.Amount);
                    else
                        query = query.OrderByDescending(q => q.Amount);
                    break;


                case "category":
                    if (ascending)
                        query = query.OrderBy(q => q.Category.Name);
                    else
                        query = query.OrderByDescending(q => q.Category.Name);
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

        public DateTime GetActiveToDate(RecurringFormDto formDto)
        {
            DateTime dateActiveTo = formDto.ActiveFrom.AddMonths(1);
            if(formDto.ActiveTo.HasValue)
            {
                dateActiveTo = formDto.ActiveTo.Value;
            } 
            
            if(formDto.ChargeMonthCount.HasValue)
            {
                dateActiveTo = formDto.ActiveFrom.AddMonths(formDto.ChargeMonthCount.Value);
            }

            return dateActiveTo;
        }

        public int GetChargeDay(RecurringFormDto formDto)
        { 
            if (formDto.ChargeDay.HasValue)
            {
                return formDto.ChargeDay.Value;
            }
            else
            {
                return formDto.ActiveFrom.Day;
            }
        }

        #endregion
    }
}
