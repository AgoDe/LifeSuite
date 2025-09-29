using AutoMapper;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Abstractions.Services;
using BudgetManager.Data.Models.Dto.Reports;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Enums;
using BudgetManager.Data.Models.Dto;
using System.Globalization;

namespace BudgetManager.Data.Services;

/// <summary>
/// Servizio per la generazione di report e previsioni
/// </summary>
public class ReportService : IReportService
{
    private readonly IRepositoryUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TransactionService _transactionService;
    private readonly RecurringService _recurringService;
    private readonly AccountService _accountService;

    public ReportService(
        IRepositoryUnitOfWork unitOfWork, 
        IMapper mapper,
        TransactionService transactionService,
        RecurringService recurringService,
        AccountService accountService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _transactionService = transactionService;
        _recurringService = recurringService;
        _accountService = accountService;
    }

    /// <summary>
    /// Genera un report di riepilogo mensile o annuale
    /// </summary>
    public async Task<SummaryReportDto> GetSummaryReportAsync(string userId, Guid? accountId, string period)
    {
        var (startDate, endDate) = ParsePeriod(period);
        var (previousStartDate, previousEndDate) = GetPreviousPeriod(period);

        // Filtro per le transazioni del periodo corrente
        var currentFilter = new TransactionFilter
        {
            UserId = userId,
            AccountId = accountId,
            DateFrom = startDate,
            DateTo = endDate,
            PageNumber = 1,
            PageSize = int.MaxValue,
            Status = TransactionStatus.Charged // Solo transazioni effettive
        };

        // Filtro per le transazioni del periodo precedente
        var previousFilter = new TransactionFilter
        {
            UserId = userId,
            AccountId = accountId,
            DateFrom = previousStartDate,
            DateTo = previousEndDate,
            PageNumber = 1,
            PageSize = int.MaxValue,
            Status = TransactionStatus.Charged
        };

        // Recupera le transazioni per entrambi i periodi
        var currentTransactions = await _transactionService.GetListAsync(currentFilter);
        var previousTransactions = await _transactionService.GetListAsync(previousFilter);

        // Calcola i totali correnti
        var currentIncomeTotal = currentTransactions.Items
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var currentExpenseTotal = currentTransactions.Items
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        // Calcola i totali precedenti per il trend
        var previousIncomeTotal = previousTransactions.Items
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var previousExpenseTotal = previousTransactions.Items
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        // Raggruppa per categoria
        var categoryGroups = currentTransactions.Items
            .GroupBy(t => new { t.Category.Id, t.Category.Name, t.Category.Color })
            .Select(g => new CategorySummaryDto
            {
                Name = g.Key.Name,
                Color = g.Key.Color,
                Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderByDescending(c => c.Income + c.Expense)
            .ToList();

        // Calcola le variazioni percentuali
        var expenseDelta = CalculatePercentageChange(previousExpenseTotal, currentExpenseTotal);
        var incomeDelta = CalculatePercentageChange(previousIncomeTotal, currentIncomeTotal);

        return new SummaryReportDto
        {
            Period = period,
            IncomeTotal = currentIncomeTotal,
            ExpenseTotal = currentExpenseTotal,
            NetBalance = currentIncomeTotal - currentExpenseTotal,
            Categories = categoryGroups,
            Trend = new TrendDto
            {
                PreviousPeriodExpense = previousExpenseTotal,
                ExpenseDeltaPercent = expenseDelta,
                PreviousPeriodIncome = previousIncomeTotal,
                IncomeDeltaPercent = incomeDelta
            }
        };
    }

    /// <summary>
    /// Genera una previsione di budget per un periodo futuro
    /// </summary>
    public async Task<ForecastReportDto> GetForecastReportAsync(string userId, Guid accountId, string period)
    {
        var (startDate, endDate) = ParsePeriod(period);
        var today = DateTime.Now.Date;

        // Recupera il saldo attuale dell'account
        var account = await _accountService.GetByIdAsync(accountId);
        if (account == null)
            throw new ArgumentException($"Account with ID {accountId} not found");

        var currentBalance = account.Balance;

        // Recupera le transazioni già programmate per il periodo
        var scheduledFilter = new TransactionFilter
        {
            UserId = userId,
            AccountId = accountId,
            DateFrom = startDate,
            DateTo = endDate,
            PageNumber = 1,
            PageSize = int.MaxValue
        };

        var scheduledTransactions = await _transactionService.GetListAsync(scheduledFilter);

        // Recupera le ricorrenze attive
        var recurringFilter = new RecurringFilter
        {
            UserId = userId,
            AccountId = accountId,
            PageNumber = 1,
            PageSize = int.MaxValue,
            IsActive = true
        };

        var activeRecurrings = await _recurringService.GetListAsync(recurringFilter);

        // Genera le transazioni dalle ricorrenze per il periodo
        var recurrencePreview = new List<RecurrencePreviewDto>();
        var predictedIncome = scheduledTransactions.Items
            .Where(t => t.Type == TransactionType.Income && t.Status != TransactionStatus.Cancelled)
            .Sum(t => t.Amount);

        var predictedExpense = scheduledTransactions.Items
            .Where(t => t.Type == TransactionType.Expense && t.Status != TransactionStatus.Cancelled)
            .Sum(t => t.Amount);

        // Processa le ricorrenze attive
        foreach (var recurring in activeRecurrings.Items)
        {
            if (IsRecurringActiveInPeriod(recurring, startDate, endDate))
            {
                var recurrenceDates = GetRecurrenceDatesInPeriod(recurring, startDate, endDate);
                
                foreach (var date in recurrenceDates)
                {
                    recurrencePreview.Add(new RecurrencePreviewDto
                    {
                        Name = recurring.Description,
                        Amount = recurring.Amount,
                        Date = date,
                        Type = recurring.Type.ToString(),
                        Category = recurring.Category?.Name ?? "Senza categoria"
                    });

                    if (recurring.Type == TransactionType.Income)
                        predictedIncome += recurring.Amount;
                    else
                        predictedExpense += recurring.Amount;
                }
            }
        }

        //// Calcola le stime per le spese variabili (media degli ultimi 3 mesi)
        //var variableExpenses = await CalculateVariableExpenseEstimatesAsync(userId, accountId, startDate);

        //// Aggiungi le stime variabili alle spese previste
        //predictedExpense += variableExpenses.Sum(v => v.Average);

        var predictedBalance = currentBalance + predictedIncome - predictedExpense;

        // Genera alert se necessario
        var alerts = GenerateAlerts(currentBalance, predictedBalance, predictedExpense);

        return new ForecastReportDto
        {
            Period = period,
            CurrentBalance = currentBalance,
            PredictedIncome = predictedIncome,
            PredictedExpense = predictedExpense,
            PredictedBalance = predictedBalance,
            Recurrences = recurrencePreview.OrderBy(r => r.Date).ToList(),
            //VariableExpenseEstimate = variableExpenses,
            Alerts = alerts
        };
    }

    private (DateTime start, DateTime end) ParsePeriod(string period)
    {
        if (period.Length == 4) // Anno (es: "2025")
        {
            var year = int.Parse(period);
            return (new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }
        else if (period.Length == 7) // Mese (es: "2025-09")
        {
            var parts = period.Split('-');
            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return (start, end);
        }
        else
        {
            throw new ArgumentException($"Invalid period format: {period}. Use YYYY for year or YYYY-MM for month.");
        }
    }

    private (DateTime start, DateTime end) GetPreviousPeriod(string period)
    {
        if (period.Length == 4) // Anno precedente
        {
            var year = int.Parse(period) - 1;
            return (new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }
        else if (period.Length == 7) // Mese precedente
        {
            var parts = period.Split('-');
            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var currentDate = new DateTime(year, month, 1);
            var previousMonth = currentDate.AddMonths(-1);
            var start = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return (start, end);
        }
        else
        {
            throw new ArgumentException($"Invalid period format: {period}");
        }
    }

    private string CalculatePercentageChange(decimal oldValue, decimal newValue)
    {
        if (oldValue == 0)
        {
            return newValue > 0 ? "+∞%" : "0%";
        }

        var percentageChange = ((newValue - oldValue) / oldValue) * 100;
        var sign = percentageChange >= 0 ? "+" : "";
        return $"{sign}{percentageChange:F1}%";
    }

    private bool IsRecurringActiveInPeriod(RecurringDto recurring, DateTime startDate, DateTime endDate)
    {
        return recurring.ActiveFrom <= endDate && recurring.ActiveTo >= startDate;
    }

    private List<DateTime> GetRecurrenceDatesInPeriod(RecurringDto recurring, DateTime startDate, DateTime endDate)
    {
        var dates = new List<DateTime>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
       
            var candidateDate = new DateTime(currentDate.Year, currentDate.Month, recurring.ChargeDay);

            if (candidateDate >= startDate && candidateDate <= endDate && 
                candidateDate >= recurring.ActiveFrom && candidateDate <= recurring.ActiveTo)
            {
                dates.Add(candidateDate);
            }

            currentDate = currentDate.AddMonths(1);
        }

        return dates;
    }

    private async Task<List<VariableExpenseDto>> CalculateVariableExpenseEstimatesAsync(string userId, Guid accountId, DateTime periodStart)
    {
        // Calcola la media delle spese degli ultimi 3 mesi per categoria
        var threeMonthsAgo = periodStart.AddMonths(-3);
        
        var historicalFilter = new TransactionFilter
        {
            UserId = userId,
            AccountId = accountId,
            DateFrom = threeMonthsAgo,
            DateTo = periodStart.AddDays(-1),
            PageNumber = 1,
            PageSize = int.MaxValue,
            Status = TransactionStatus.Charged,
            Type = TransactionType.Expense
        };

        var historicalTransactions = await _transactionService.GetListAsync(historicalFilter);

        var categoryAverages = historicalTransactions.Items
            .GroupBy(t => t.Category.Name)
            .Select(g => new VariableExpenseDto
            {
                Category = g.Key,
                Average = g.Sum(t => t.Amount) / 3, // Media su 3 mesi
                MonthsConsidered = 3
            })
            .Where(v => v.Average > 0)
            .OrderByDescending(v => v.Average)
            .ToList();

        return categoryAverages;
    }

    private List<ForecastAlertDto> GenerateAlerts(decimal currentBalance, decimal predictedBalance, decimal predictedExpense)
    {
        var alerts = new List<ForecastAlertDto>();

        // Alert per saldo negativo previsto
        if (predictedBalance < 0)
        {
            alerts.Add(new ForecastAlertDto
            {
                Type = "negative_balance",
                Message = $"Il saldo previsto sarà negativo: {predictedBalance:C}",
                Severity = "error"
            });
        }

        // Alert per saldo basso
        if (predictedBalance > 0 && predictedBalance < 100)
        {
            alerts.Add(new ForecastAlertDto
            {
                Type = "low_balance",
                Message = $"Il saldo previsto sarà molto basso: {predictedBalance:C}",
                Severity = "warning"
            });
        }

        // Alert per spese elevate (più di 80% del saldo attuale)
        if (currentBalance > 0 && predictedExpense > (currentBalance * 0.8m))
        {
            alerts.Add(new ForecastAlertDto
            {
                Type = "high_expenses",
                Message = "Le spese previste superano l'80% del saldo attuale",
                Severity = "warning"
            });
        }

        return alerts;
    }
}