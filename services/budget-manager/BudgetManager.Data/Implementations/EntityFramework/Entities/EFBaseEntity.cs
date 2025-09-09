using BudgetManager.Data.Abstraction.Entities;
using BudgetManager.Data.Abstractions.Models.DomainModels;

namespace BudgetManager.Data.Implementations.EntityFramework.Entities;

public class EFBaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; }
    public byte[] RowVersion { get; }
    public bool IsDeleted { get; set; }
}