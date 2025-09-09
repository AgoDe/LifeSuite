using BudgetManager.Data.Models.Dto.Forms;

namespace SecondBrain.Data.Abstraction.Repositories
{
    public interface IRecurringRepository
    {
        Task<(Guid, Guid)> CreateTransferRecurring(RecurringFormDto formDto);
    }
}
