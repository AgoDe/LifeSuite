using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Models.Dto.Filters;

public class UserOwnerListFilter : ListFilter, IUserOwnedFilter
{
    public string? UserId { get; set; }
}