using BudgetManager.Data.Abstraction.Entities;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Abstraction.Repositories
{
    public interface IAccountRepository
    {
        Task UpdateBalanceAsync(Guid accountId, decimal amount, TransactionType type);
        //Task UpdateBalanceAsync(ITransactionEntity transaction);
        Task RefundBalanceAsync(Guid accountId, decimal amount, TransactionType type);
        //Task RefundBalanceAsync(ITransactionEntity transaction);
        Task<ICollection<AccountDM>> GetListByUserId(Guid userId);
    }
}
