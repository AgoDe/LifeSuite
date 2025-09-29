using BudgetManager.Data.Models.Dto.Reports;

namespace BudgetManager.Data.Abstractions.Services;

/// <summary>
/// Interfaccia per i servizi di reportistica e previsioni
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Genera un report di riepilogo mensile o annuale per un account
    /// </summary>
    /// <param name="userId">ID dell'utente</param>
    /// <param name="accountId">ID dell'account (opzionale, se null considera tutti gli account dell'utente)</param>
    /// <param name="period">Periodo nel formato "YYYY-MM" per mese o "YYYY" per anno</param>
    /// <returns>Report di riepilogo con totali, categorie e trend</returns>
    Task<SummaryReportDto> GetSummaryReportAsync(string userId, Guid? accountId, string period);
    
    /// <summary>
    /// Genera una previsione di budget per un account e periodo
    /// </summary>
    /// <param name="userId">ID dell'utente</param>
    /// <param name="accountId">ID dell'account</param>
    /// <param name="period">Periodo nel formato "YYYY-MM"</param>
    /// <returns>Previsione con saldo atteso, ricorrenze e alert</returns>
    Task<ForecastReportDto> GetForecastReportAsync(string userId, Guid accountId, string period);
}