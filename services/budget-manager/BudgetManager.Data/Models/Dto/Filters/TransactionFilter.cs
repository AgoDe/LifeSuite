using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Models.Dto.Filters;

public class TransactionFilter : UserOwnerListFilter
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }
    public TransactionType? Type { get; set; }
    public Guid? CategoryId { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public Guid? AccountId { get; set; }
    public List<Guid>? AccountIds { get; set; }
    
    public TransactionStatus? Status { get; set; }
    public TransactionStatus? StatusFrom { get; set; }
    public TransactionStatus? StatusTo { get; set; }
    public bool IncludeTransfers { get; set; } = true;
    public bool IncludeRecurrings { get; set; } = true;
}