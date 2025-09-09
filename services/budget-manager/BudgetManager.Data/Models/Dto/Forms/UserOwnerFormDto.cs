using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Models.Dto.Interfaces;

namespace BudgetManager.Data.Models.Dto.Forms;

public class UserOwnerFormDto : IUserOwnedFormDto
{
    public int UserId { get; set; }
}