namespace BudgetManager.Data.Abstractions.Models.DomainModels;

public interface IBaseDomainModel
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; }
    byte[] RowVersion { get; }
    bool IsDeleted { get; set; }
}