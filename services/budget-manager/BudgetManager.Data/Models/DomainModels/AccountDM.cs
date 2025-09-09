using BudgetManager.Data.Abstraction.Entities;
using BudgetManager.Data.Abstractions.Models.DomainModels;

namespace BudgetManager.Data.Models.DomainModels;

public class AccountDM : BaseDomainModel
{
    public string Name { get; set; }
    public string Institution { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal Balance { get; set; }
    public DateTime BalanceDate { get; set; }
    public int UserId { get; set; }
    
}
