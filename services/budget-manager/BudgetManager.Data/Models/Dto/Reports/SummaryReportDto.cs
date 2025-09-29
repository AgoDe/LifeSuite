namespace BudgetManager.Data.Models.Dto.Reports;

/// <summary>
/// DTO per il riepilogo mensile/annuale di spese e entrate
/// </summary>
public class SummaryReportDto
{
    /// <summary>
    /// Periodo del report (es: "2025-09" per mese, "2025" per anno)
    /// </summary>
    public string Period { get; set; } = string.Empty;
    
    /// <summary>
    /// Totale entrate nel periodo
    /// </summary>
    public decimal IncomeTotal { get; set; }
    
    /// <summary>
    /// Totale spese nel periodo
    /// </summary>
    public decimal ExpenseTotal { get; set; }
    
    /// <summary>
    /// Saldo netto (entrate - spese)
    /// </summary>
    public decimal NetBalance { get; set; }
    
    /// <summary>
    /// Breakdown per categoria
    /// </summary>
    public List<CategorySummaryDto> Categories { get; set; } = new List<CategorySummaryDto>();
    
    /// <summary>
    /// Dati di trend rispetto al periodo precedente
    /// </summary>
    public TrendDto? Trend { get; set; }
}

/// <summary>
/// DTO per il riepilogo di una categoria
/// </summary>
public class CategorySummaryDto
{
    /// <summary>
    /// Nome della categoria
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Totale spese per questa categoria
    /// </summary>
    public decimal Expense { get; set; }
    
    /// <summary>
    /// Totale entrate per questa categoria
    /// </summary>
    public decimal Income { get; set; }
    
    /// <summary>
    /// Colore associato alla categoria per i grafici
    /// </summary>
    public string Color { get; set; } = string.Empty;
}

/// <summary>
/// DTO per i dati di trend
/// </summary>
public class TrendDto
{
    /// <summary>
    /// Totale spese del periodo precedente
    /// </summary>
    public decimal PreviousPeriodExpense { get; set; }
    
    /// <summary>
    /// Variazione percentuale delle spese (es: "+3%", "-5%")
    /// </summary>
    public string ExpenseDeltaPercent { get; set; } = string.Empty;
    
    /// <summary>
    /// Totale entrate del periodo precedente
    /// </summary>
    public decimal PreviousPeriodIncome { get; set; }
    
    /// <summary>
    /// Variazione percentuale delle entrate (es: "0%", "+10%")
    /// </summary>
    public string IncomeDeltaPercent { get; set; } = string.Empty;
}