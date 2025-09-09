using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Models.Dto.Filters;

public class UserOwnerListFilter : ListFilter, IUserOwnedFilter
{
    public int? UserId { get; set; }
}