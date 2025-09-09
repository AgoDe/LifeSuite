using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Models.Dto.Filters
{
    public class RecurringFilter : UserOwnerListFilter
    {
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public TransactionType? Type { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public Guid? AccountId { get; set; }
        public List<Guid>? AccountIds { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
    }
}