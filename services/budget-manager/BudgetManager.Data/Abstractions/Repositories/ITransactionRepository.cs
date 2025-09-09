using BudgetManager.Data.Models.Dto.Forms;

namespace SecondBrain.Data.Abstraction.Repositories
{
    public interface ITransactionRepository
    {
        Task<(Guid, Guid)> CreateTransferAsync(TransactionFormDto formDto);
    }
}
