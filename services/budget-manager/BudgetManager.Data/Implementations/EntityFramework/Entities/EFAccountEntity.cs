namespace BudgetManager.Data.Implementations.EntityFramework.Entities;

public class EFAccountEntity : EFBaseEntity
{
    public string Name { get; set; }
    public string Institution { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal Balance { get; set; }
    public DateTime BalanceDate { get; set; }
    public int UserId { get; set; }
        
    // Concrete navigation properties for Entity Framework
    public ICollection<EFRecurringEntity> Recurrings { get; set; } = new List<EFRecurringEntity>();
    public ICollection<EFTransactionEntity> Transactions { set; get; } = new List<EFTransactionEntity>();
}