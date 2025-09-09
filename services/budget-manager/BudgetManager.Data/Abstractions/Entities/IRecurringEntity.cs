using BudgetManager.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Data.Abstraction.Entities
{
    public interface IRecurringEntity : IBaseEntity
    {
        string Description { get; set; }
        string? Institution { get; set; }
        string? Notes { get; set; }
        [DataType(DataType.Date)]
        DateTime ActiveFrom { get; set; }
        [DataType(DataType.Date)]
        DateTime ActiveTo { get; set; }
        int ChargeDay { get; set; }
        decimal Amount { get; set; }
        TransactionType Type { get; set; }
        Guid? OriginAccountId { get; set; }
        Guid? DestinationAccountId { get; set; }
        Guid? CategoryId { get; set; }
        string UserId { get; set; }
    }
}
