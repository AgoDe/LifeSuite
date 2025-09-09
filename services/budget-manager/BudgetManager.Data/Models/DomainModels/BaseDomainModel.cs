using BudgetManager.Data.Abstractions.Models.DomainModels;

namespace BudgetManager.Data.Models.DomainModels;

public class BaseDomainModel : IBaseDomainModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } 
    public byte[] RowVersion { get; set; }
    public bool IsDeleted { get; set; }
}