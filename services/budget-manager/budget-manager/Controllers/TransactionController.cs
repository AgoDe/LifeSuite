using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Services;
using Microsoft.AspNetCore.Mvc;
using BudgetManager.Api.Models;

namespace BudgetManager.Api.Controllers;

public class TransactionController : CrudController<TransactionDto, TransactionFormDto, TransactionFilter>
{
    
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService) : base(transactionService)
    {
        _transactionService = transactionService; 
    }
    
    /// <summary>
    /// Recupera transazioni esistenti e genera transazioni stimate dalle ricorrenze attive per un account e periodo
    /// </summary>
    /// <param name="filter">Filtri per la ricerca (AccountId, DateFrom e DateTo sono obbligatori)</param>
    /// <returns>Lista paginata di transazioni esistenti e stimate</returns>
    [HttpGet("with-active-recurrings")]
    public async Task<ActionResult<ApiResponse<PaginatedList<TransactionDto>>>> GetTransactionsWithActiveRecurrings([FromQuery] TransactionFilter filter)
    {
        ApiResponse response;
        try
        {
            var result = await _transactionService.GetTransactionsWithActiveRecurringsAsync(filter);
            response = new ApiResponse<PaginatedList<TransactionDto>>(result);
        }
        catch (Exception ex)
        {
           response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
        }

        return Ok(response);
    }
}