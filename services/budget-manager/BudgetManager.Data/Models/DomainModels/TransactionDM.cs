using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetManager.Data.Abstraction.Entities;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Models.DomainModels;

public class TransactionDM : BaseDomainModel
{
    public string Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date {  get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public string? Notes { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? RecurringId { get; set; }
    public Guid AccountId { get; set; }

    // Navigational properties
    public virtual UserDM User { get; set; } = null!;
    public virtual CategoryDM Category { get; set; } = null!;
    public virtual RecurringDM Recurring { get; set; } = null!;
    public virtual TransactionDM RelatedTransaction { get; set; } = null!;
    public virtual AccountDM Account { get; set; } = null!;
    
}