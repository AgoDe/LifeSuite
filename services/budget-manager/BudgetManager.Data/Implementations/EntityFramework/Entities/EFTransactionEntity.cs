using System.Buffers;
using System.ComponentModel.DataAnnotations;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Implementations.EntityFramework.Entities;

public class EFTransactionEntity : EFBaseEntity
{
    public string Description { get; set; }
    [DataType(DataType.Date)]
    public DateTime Date {  get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public string? Notes { get; set; }
    public TransactionType Type { get; set; }
   
    public Guid AccountId { get; set; }
    public Guid CategoryId { get; set; }
    public int UserId { get; set; }
    public Guid? RecurringId { get; set; }
    public Guid? RelatedTransactionId { get; set; }
    
    // Concrete navigation properties for Entity Framework
    public virtual EFAccountEntity Account { get; set; } = null!;
    public virtual EFCategoryEntity Category { get; set; } = null!;
    public virtual EFRecurringEntity? Recurring { get; set; }
    public virtual EFTransactionEntity? RelatedTransaction { get; set; }
}