using System.ComponentModel.DataAnnotations;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Models.DomainModels;

public class RecurringDM : BaseDomainModel
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
 
    // Proprietà concrete per Entity Framework
    public AccountDM Account { get; set; } = null!;
    public virtual RecurringDM? RelatedRecurring { get; set; }
    public virtual CategoryDM Category { get; set; } = null!;
    public UserDM User { get; set; }
}