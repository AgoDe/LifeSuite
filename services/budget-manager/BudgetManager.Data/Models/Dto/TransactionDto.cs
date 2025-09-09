using BudgetManager.Data.Models.Enums;
using BudgetManager.Data.Abstraction.Models.Dto;
using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Data.Models.Dto
{
    public class TransactionDto : BaseDto
    {
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date {  get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public string? Notes { get; set; }

        public AccountDto Account { get; set; } = null!;    
        public CategoryDto Category { get; set; } = null!;
        public RecurringDto? Recurring { get; set; }

        public TransactionDto? RelatedTransaction { get; set; }
        
    
    }
}