using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Abstraction.Entities
{
    public interface ITransactionEntity : IBaseEntity
    {
        TransactionType Type { get; set; }
        Guid AccountId { get; set; }
        Guid OperationId { get; set; }
        string UserId { get; set; }
    }
}
