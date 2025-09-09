using System.ComponentModel.DataAnnotations;
using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Models.Dto.Forms
{
    public class TransactionFormDto : UserOwnerFormDto
    {
        public string Description { get; set; }

        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public Guid CategoryId { get; set; }
        public string? Notes { get; set; }
        public Guid AccountId {  get; set; }
        public Guid? RecurringId { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public TransactionStatus Status { get; set; }
        
    }
}