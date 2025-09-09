using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Models.Dto.Filters;

public class ListFilter : IListFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? OrderBy { get; set; }
    public bool Ascending { get; set; }
    public string? SearchText { get; set; }
}