using BudgetManager.Api.Models;
using BudgetManager.Api.Models.Interfaces;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Services;
using Microsoft.AspNetCore.Mvc;
using BudgetManager.Data.Models.Enums;
using BudgetManager.Data.Exceptions;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using budget_manager.Models;

namespace BudgetManager.Api.Controllers
{
    public class AccountController : CrudController<AccountDto, AccountFormDto, AccountFilter>
    {
        private readonly AccountService _accountService;
        private readonly UserContext _userContext;

        public AccountController(AccountService accountService, UserContext userContext)
            : base(accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _userContext = userContext;
        }

        /// <summary>
        /// Recupera tutti gli account di un utente
        /// </summary>
        [HttpGet("user")]
        [ProducesResponseType(typeof(List<AccountDto>), 200)]
        public async Task<IActionResult> GetAccountsByUserId()
        {
            if (string.IsNullOrEmpty(_userContext.UserId))
                return Unauthorized();

            var accounts = await _accountService.GetAccountsByUserIdAsync(_userContext.UserId);
            return Ok(new ApiResponse<List<AccountDto>>(accounts.ToList()));
        }

        /// <summary>
        /// Aggiorna il saldo di un account
        /// </summary>
        [HttpPatch("{id}/balance")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateBalance(Guid id, [FromBody] UpdateBalanceRequest request)
        {
            try
            {
                await _accountService.UpdateBalanceAsync(id, request.Amount, request.Type);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [HttpPost]
        // [AutoSetUserId]
        // public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] AccountFormDto accountDto)
        // {
        //     // Il UserId è già impostato automaticamente
        //     var result = await _accountService.CreateAsync(accountDto);
        //     return CreatedAtAction(nameof(GetMyAccounts), new { id = result.Id }, result);
        // }
        [HttpGet("select-options/{userId}")]
        [ProducesResponseType(typeof(IApiResponse), 200)]
        public async Task<IActionResult> GetSelectOptions(string userId)
        {
            ApiResponse response;
            try
            {
                var result = await _accountService.GetSelectOptions(userId);

                response = new ApiResponse<List<SelectOption>>(result);
            }
            catch (Exception ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error);
            }

            return Ok(response);

        }
    }
    

    /// <summary>
    /// DTO per la richiesta di aggiornamento del saldo
    /// </summary>
    public class UpdateBalanceRequest
    {
        /// <summary>
        /// Importo da aggiungere o sottrarre
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Tipo di transazione (Income, Expense, Transfer)
        /// </summary>
        public TransactionType Type { get; set; }
    }
}