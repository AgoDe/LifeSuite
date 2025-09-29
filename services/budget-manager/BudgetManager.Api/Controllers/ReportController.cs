using BudgetManager.Api.Models;
using BudgetManager.Data.Abstractions.Services;
using BudgetManager.Data.Models.Dto.Reports;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers;

/// <summary>
/// Controller per i report e le previsioni di budget
/// </summary>
[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly UserContext _userContext;

    public ReportController(IReportService reportService, UserContext userContext)
    {
        _reportService = reportService;
        _userContext = userContext;
    }

    /// <summary>
    /// Genera un report di riepilogo mensile o annuale
    /// </summary>
    /// <param name="period">Periodo nel formato "YYYY-MM" per mese o "YYYY" per anno (es: "2025-09" o "2025")</param>
    /// <param name="accountId">ID dell'account (opzionale, se non specificato considera tutti gli account dell'utente)</param>
    /// <returns>Report di riepilogo con totali, categorie e trend</returns>
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<SummaryReportDto>>> GetSummaryReport(
        [FromQuery] string period,
        [FromQuery] Guid? accountId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(period))
            {
                return BadRequest(new ApiResponse(ApiResponse.ApiResponseType.Error, "Period parameter is required"));
            }

            // Validazione del formato del periodo
            if (!IsValidPeriodFormat(period))
            {
                return BadRequest(new ApiResponse(ApiResponse.ApiResponseType.Error, 
                    "Invalid period format. Use YYYY for year or YYYY-MM for month"));
            }

            var userId = _userContext.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse(ApiResponse.ApiResponseType.Error, "User not authenticated"));
            }

            var result = await _reportService.GetSummaryReportAsync(userId, accountId, period);
            return Ok(new ApiResponse<SummaryReportDto>(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(ApiResponse.ApiResponseType.Error, 
                $"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Genera una previsione di budget per un account e periodo
    /// </summary>
    /// <param name="period">Periodo nel formato "YYYY-MM" (es: "2025-10")</param>
    /// <param name="accountId">ID dell'account per cui generare la previsione</param>
    /// <returns>Previsione con saldo atteso, ricorrenze e alert</returns>
    [HttpGet("forecast")]
    public async Task<ActionResult<ApiResponse<ForecastReportDto>>> GetForecastReport(
        [FromQuery] string period,
        [FromQuery] Guid accountId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(period))
            {
                return BadRequest(new ApiResponse(ApiResponse.ApiResponseType.Error, "Period parameter is required"));
            }

            if (accountId == Guid.Empty)
            {
                return BadRequest(new ApiResponse(ApiResponse.ApiResponseType.Error, "AccountId parameter is required"));
            }

            // Validazione del formato del periodo (solo mensile per forecast)
            if (!IsValidMonthlyPeriodFormat(period))
            {
                return BadRequest(new ApiResponse(ApiResponse.ApiResponseType.Error, 
                    "Invalid period format. Use YYYY-MM for month"));
            }

            var userId = _userContext.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse(ApiResponse.ApiResponseType.Error, "User not authenticated"));
            }

            var result = await _reportService.GetForecastReportAsync(userId, accountId, period);
            return Ok(new ApiResponse<ForecastReportDto>(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(ApiResponse.ApiResponseType.Error, 
                $"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Valida il formato del periodo (supporta sia YYYY che YYYY-MM)
    /// </summary>
    private bool IsValidPeriodFormat(string period)
    {
        if (string.IsNullOrWhiteSpace(period))
            return false;

        // Formato anno (YYYY)
        if (period.Length == 4 && int.TryParse(period, out var year))
        {
            return year >= 1900 && year <= 2100;
        }

        // Formato mese (YYYY-MM)
        if (period.Length == 7 && period[4] == '-')
        {
            var parts = period.Split('-');
            if (parts.Length == 2 && 
                int.TryParse(parts[0], out year) && 
                int.TryParse(parts[1], out var month))
            {
                return year >= 1900 && year <= 2100 && month >= 1 && month <= 12;
            }
        }

        return false;
    }

    /// <summary>
    /// Valida il formato del periodo mensile (solo YYYY-MM)
    /// </summary>
    private bool IsValidMonthlyPeriodFormat(string period)
    {
        if (string.IsNullOrWhiteSpace(period) || period.Length != 7 || period[4] != '-')
            return false;

        var parts = period.Split('-');
        if (parts.Length == 2 && 
            int.TryParse(parts[0], out var year) && 
            int.TryParse(parts[1], out var month))
        {
            return year >= 1900 && year <= 2100 && month >= 1 && month <= 12;
        }

        return false;
    }
}