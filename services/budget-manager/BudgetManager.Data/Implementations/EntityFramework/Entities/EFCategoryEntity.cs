namespace BudgetManager.Data.Implementations.EntityFramework.Entities;

public class EFCategoryEntity : EFBaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    
    public Guid? ParentId { get; set; }
    public string UserId { get; set; }
        
    // Proprietà concrete per Entity Framework
    public virtual EFCategoryEntity? Parent { get; set; }
    public virtual ICollection<EFCategoryEntity> Children { get; set; } = new List<EFCategoryEntity>();
    public virtual ICollection<EFTransactionEntity> Transactions { get; set; } = new List<EFTransactionEntity>();
    public virtual ICollection<EFRecurringEntity> Recurrings { get; set; } = new List<EFRecurringEntity>();
}