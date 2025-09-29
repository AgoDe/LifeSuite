namespace BudgetManager.Data.Models.Dto.Reports;

/// <summary>
/// DTO per la previsione di budget
/// </summary>
public class ForecastReportDto
{
    /// <summary>
    /// Periodo della previsione (es: "2025-10")
    /// </summary>
    public string Period { get; set; } = string.Empty;
    
    /// <summary>
    /// Saldo iniziale (attuale)
    /// </summary>
    public decimal CurrentBalance { get; set; }
    
    /// <summary>
    /// Entrate previste nel periodo
    /// </summary>
    public decimal PredictedIncome { get; set; }
    
    /// <summary>
    /// Spese previste nel periodo
    /// </summary>
    public decimal PredictedExpense { get; set; }
    
    /// <summary>
    /// Saldo previsto a fine periodo
    /// </summary>
    public decimal PredictedBalance { get; set; }
    
    /// <summary>
    /// Anteprima delle ricorrenze che si attiveranno nel periodo
    /// </summary>
    public List<RecurrencePreviewDto> Recurrences { get; set; } = new List<RecurrencePreviewDto>();
    
    /// <summary>
    /// Stime per spese variabili basate sulla media storica
    /// </summary>
    public List<VariableExpenseDto> VariableExpenseEstimate { get; set; } = new List<VariableExpenseDto>();
    
    /// <summary>
    /// Eventuali alert o avvisi
    /// </summary>
    public List<ForecastAlertDto> Alerts { get; set; } = new List<ForecastAlertDto>();
}

/// <summary>
/// DTO per l'anteprima di una ricorrenza
/// </summary>
public class RecurrencePreviewDto
{
    /// <summary>
    /// Nome/descrizione della ricorrenza
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Importo della ricorrenza
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Data prevista di attivazione
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Tipo di transazione (Income/Expense)
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Nome della categoria
    /// </summary>
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// DTO per la stima delle spese variabili
/// </summary>
public class VariableExpenseDto
{
    /// <summary>
    /// Nome della categoria
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Media delle spese mensili per questa categoria
    /// </summary>
    public decimal Average { get; set; }
    
    /// <summary>
    /// Numero di mesi considerati per il calcolo della media
    /// </summary>
    public int MonthsConsidered { get; set; }
}

/// <summary>
/// DTO per gli alert del forecast
/// </summary>
public class ForecastAlertDto
{
    /// <summary>
    /// Tipo di alert (threshold, negative_balance, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Messaggio dell'alert
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Livello di gravità (info, warning, error)
    /// </summary>
    public string Severity { get; set; } = "info";
}