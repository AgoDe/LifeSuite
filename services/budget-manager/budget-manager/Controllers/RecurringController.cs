using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Services;

namespace BudgetManager.Api.Controllers;

public class RecurringController : CrudController<RecurringDto, RecurringFormDto, RecurringFilter>
{
    
    private readonly RecurringService _recurringService;

    public RecurringController(RecurringService recurringService) : base(recurringService)
    {
        _recurringService = recurringService; 
    }
}