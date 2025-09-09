

using AutoMapper;
using BudgetManager.Data.Abstraction.Entities;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Data.Implementations.EntityFramework.Repositories
{
    public class EFAccountRepository : EFCrudRepository<AccountDM, EFAccountEntity, AccountFilter, AccountFormDto>, IAccountRepository
    {
        
        private readonly IMapper _mapper;
        public EFAccountRepository(EFDbcontext db, IMapper mapper) : base(db, mapper)
        {
            _mapper = mapper;
        }

        public override async Task<Guid> CreateAsync(AccountFormDto model)
        {
            model.Balance = model.Balance;
            return await base.CreateAsync(model);   
        }

        public async Task UpdateBalanceAsync(TransactionDM transaction)
        {
            EFAccountEntity account = await GetEntityAsync(transaction.AccountId);
            if (transaction.Type == TransactionType.Income)
                account.Balance += transaction.Amount;
            if (transaction.Type == TransactionType.Expense)
                account.Balance -= transaction.Amount;

            account.BalanceDate = DateTime.Now;
            await UpdateEntityAsync(account);
        }

        public async Task UpdateBalanceAsync(Guid accountId, decimal amount, TransactionType type)
        {

            EFAccountEntity account = await GetEntityAsync(accountId);
            if (account == null) return;

            if (type == TransactionType.Income)
                account.Balance += amount;
            if (type == TransactionType.Expense)
                account.Balance -= amount;

            account.BalanceDate = DateTime.Now;
            await UpdateEntityAsync(account);
        }

        public async Task RefundBalanceAsync(TransactionDM transaction)
        {
            EFAccountEntity account = await GetEntityAsync(transaction.AccountId);
            if (transaction.Type == TransactionType.Income)
                account.Balance -= transaction.Amount;
            if (transaction.Type == TransactionType.Expense)
                account.Balance += transaction.Amount;

            account.BalanceDate = DateTime.Now;
            await UpdateEntityAsync(account);
        }

        public async Task RefundBalanceAsync(Guid accountId, decimal amount, TransactionType type)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<AccountDM>> GetListByUserId(int userId)
        {
            IQueryable<EFAccountEntity> query = dbSet;
            query = query.AsNoTracking(); // Modificato AsNoTracking() per applicarlo prima del Where
            query = query.Where(x => x.UserId == userId); // Modificato per applicare il filtro correttamente
            query = IncludeProperties(query);
            var accounts = await query.ToListAsync(); // Ottiene List<AccountEntity>
            return _mapper.Map<List<AccountDM>>(accounts); // Converte e restituisce ICollection<IAccountEntity>
        }


        #region utility
        protected override IQueryable<EFAccountEntity> IncludeProperties(IQueryable<EFAccountEntity> query)
        {
            return query;
        }

        protected override IQueryable<EFAccountEntity> GetFilteredQuery(IQueryable<EFAccountEntity> query, AccountFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(
                    q => q.Name.ToLower().Contains(filter.SearchText.ToLower())
                        || q.Institution.ToLower().Contains(filter.SearchText.ToLower())
                );
            }

            if (filter.UserId != null)
            {
                query = query.Where(c => c.UserId == filter.UserId);
            }

            return query;
        }

        protected override IQueryable<EFAccountEntity> GetOrderedQuery(IQueryable<EFAccountEntity> query, string orderBy = "", bool ascending = true)
        {
            switch (orderBy)
            {
                case "name":
                    if (ascending)
                        query = query.OrderBy(q => q.Name);
                    else
                        query = query.OrderByDescending(q => q.Name);
                    break;

                case "institution":
                    if (ascending)
                        query = query.OrderBy(q => q.Institution);
                    else
                        query = query.OrderByDescending(q => q.Name);
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
