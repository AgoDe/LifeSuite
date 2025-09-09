using System.ComponentModel.DataAnnotations;
using System.Transactions;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Implementations.EntityFramework.Entities;

public class EFRecurringEntity : EFBaseEntity
{
    public string Description { get; set; }
    public string? Institution { get; set; }
    public string? Notes { get; set; }
    [DataType(DataType.Date)]
    public DateTime ActiveFrom { get; set; }
    [DataType(DataType.Date)]
    public DateTime ActiveTo { get; set; }
    public int ChargeDay { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Guid? RelatedRecurringId { get; set; }
    
    public Guid AccountId { get; set; }
    public Guid CategoryId { get; set; }
    public int UserId { get; set; }

    // Proprietà concrete per Entity Framework
    public EFAccountEntity Account { get; set; } = null!;
    public EFCategoryEntity Category { get; set; } = null!;
    public EFRecurringEntity? RelatedRecurring { get; set; }
}