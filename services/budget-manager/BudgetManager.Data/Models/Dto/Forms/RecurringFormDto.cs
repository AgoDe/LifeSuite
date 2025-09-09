using BudgetManager.Data.Models.Enums;
using BudgetManager.Data.Abstraction.Models.Dto;

namespace BudgetManager.Data.Models.Dto.Forms
{
    public class RecurringFormDto : UserOwnerFormDto
    {
        public string Description { get; set; } = null!;
        public string? Institution { get; set; }
        public string? Notes { get; set; }
        
        public DateTime ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        
        public int? ChargeDay { get; set; }
        public int? ChargeMonthCount { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        
        public Guid AccountId { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public Guid? RelatedRecurringId { get; set; }
        public Guid CategoryId { get; set; }
    }
}