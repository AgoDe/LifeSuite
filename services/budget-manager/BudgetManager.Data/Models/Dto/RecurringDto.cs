using BudgetManager.Data.Models.Enums;
using BudgetManager.Data.Abstraction.Models.Dto;

namespace BudgetManager.Data.Models.Dto
{
    public class RecurringDto : BaseDto
    {
        
        public string Description { get; set; } = null!;
        public string? Institution { get; set; }
        public string? Notes { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public int ChargeDay { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string UserId { get; set; }
        public virtual AccountDto Account { get; set; } = null!;
        public virtual CategoryDto Category { get; set; } = null!;
        public virtual RecurringDto? RelatedRecurring { get; set; }
    }
}