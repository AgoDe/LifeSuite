namespace BudgetManager.Data.Abstraction.Entities
{
    public interface IAccountEntity : IBaseEntity
    {
        string Name { get; set; }
        string Institution { get; set; }
        decimal InitialBalance { get; set; }
        decimal Balance { get; set; }
        DateTime BalanceDate { get; set; }
        string UserId { get; set; }
    }
}
