using AutoMapper;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Models.Enums;
using BudgetManager.Data.Exceptions;
using System.Threading.Tasks;

namespace BudgetManager.Data.Services;
public class TransactionService : BaseCrudService<TransactionDM, TransactionDto, TransactionFormDto, TransactionFilter>
{
    private IMapper _mapper;
    
    public TransactionService(
        IMapper mapper, 
        IRepositoryUnitOfWork repositoryUow
    ) : base(repositoryUow, mapper)
    {
        _mapper = mapper;
    }
    
    public override async Task<Guid> CreateAsync(TransactionFormDto formDto)
    {
        if (formDto == null)
            throw new ArgumentNullException(nameof(formDto));

        // Validazione business
        var validationResult = await ValidateWithDetailsAsync(formDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Validazioni specifiche pre-creazione
        await ValidateForCreateAsync(formDto);

        Guid transactionId;
        // se è un trasferimento chiamo la funzione specifica
        if(formDto.RelatedAccountId.HasValue)
        {
            var guids = await _unitOfWork.TransactionRepository.CreateTransferAsync(formDto);

            transactionId = guids.Item1;
        } 
        else
        {
            transactionId = await _repository.CreateAsync(formDto);
        }
        
        // Se lo stato è Charged, aggiorna il bilancio dell'account
        if (formDto.Status == TransactionStatus.Charged)
        {
            await UpdateAccountBalance(formDto.AccountId, formDto.Amount, formDto.Type);
        }

        if(formDto.Status == TransactionStatus.Charged && formDto.RelatedAccountId.HasValue)
        {
            await UpdateAccountBalance(formDto.RelatedAccountId.Value, formDto.Amount, formDto.Type == TransactionType.Income ? TransactionType.Expense : TransactionType.Income);
        }

        return transactionId;
    }
    
    protected override async Task ValidateForCreateAsync(TransactionFormDto formDto)
    {
        // Validazione che relatedAccountId non sia uguale ad accountId
        if (formDto.RelatedAccountId.HasValue && formDto.RelatedAccountId.Value == formDto.AccountId)
            throw new BusinessRuleException("RelatedAccountId cannot be the same as AccountId");
            
        // Chiama la validazione base
        await base.ValidateForCreateAsync(formDto);
    }
    
    private async Task UpdateAccountBalance(Guid accountId, decimal amount, TransactionType type)
    {
        // Usa il repository specifico per AccountDM che ha i metodi per aggiornare il balance
        await _unitOfWork.AccountRepository.UpdateBalanceAsync(accountId, amount, type);
    }
    
    /// <summary>
    /// Recupera transazioni esistenti e genera transazioni stimate dalle ricorrenze attive per un account e periodo
    /// </summary>
    public async Task<PaginatedList<TransactionDto>> GetTransactionsWithActiveRecurringsAsync(TransactionFilter filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));
            
        if (!filter.AccountId.HasValue)
            throw new ArgumentException("AccountId is required", nameof(filter));
            
        if (!filter.DateFrom.HasValue || !filter.DateTo.HasValue)
            throw new ArgumentException("DateFrom and DateTo are required", nameof(filter));

        // Recupera le transazioni esistenti
        var existingTransactions = await GetListAsync(filter);
        var allTransactions = existingTransactions.Items.ToList();

        // Recupera le ricorrenze attive per l'account nel periodo specificato
        var recurringFilter = new RecurringFilter
        {
            AccountId = filter.AccountId,
            UserId = filter.UserId,
            PageNumber = 1,
            PageSize = int.MaxValue, // Recupera tutte le ricorrenze attive
            IsActive = true // Solo ricorrenze attive
        };

        var recurringService = new RecurringService(_mapper, _unitOfWork);
        var activeRecurrings = await recurringService.GetListAsync(recurringFilter);

        // Genera transazioni stimate dalle ricorrenze attive
        foreach (var recurring in activeRecurrings.Items)
        {
            if (IsRecurringActiveInPeriod(recurring, filter.DateFrom.Value, filter.DateTo.Value))
            {
                var estimatedTransactions = await GenerateEstimatedTransactionsAsync(recurring, filter.DateFrom.Value, filter.DateTo.Value);
                allTransactions.AddRange(estimatedTransactions);
            }
        }

        // Ordina per data
        allTransactions = allTransactions.OrderBy(t => t.Date).ToList();

        var totalCount = allTransactions.Count;
        // Applica paginazione manuale
        if(filter.PageNumber > 0 && filter.PageSize > 0)
        {
            allTransactions = allTransactions
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

        }

        return new PaginatedList<TransactionDto>(allTransactions, filter.PageNumber, filter.PageSize, totalCount);
    
    }
    
    /// <summary>
    /// Verifica se una ricorrenza è attiva nel periodo specificato
    /// </summary>
    private bool IsRecurringActiveInPeriod(RecurringDto recurring, DateTime dateFrom, DateTime dateTo)
    {
        // La ricorrenza deve essere iniziata prima della fine del periodo
        if (recurring.ActiveFrom > dateTo)
            return false;
            
        // Se ha una data di fine, deve essere dopo l'inizio del periodo
        if (recurring.ActiveTo < dateFrom)
            return false;
            
        return true;
    }
    
    /// <summary>
    /// Genera le transazioni stimate per una ricorrenza nel periodo specificato
    /// </summary>
    private async Task<List<TransactionDto>> GenerateEstimatedTransactionsAsync(RecurringDto recurring, DateTime dateFrom, DateTime dateTo)
    {
        var estimatedTransactions = new List<TransactionDto>();
        var currentDate = GetNextOccurrence(recurring, dateFrom);
        var monthsGenerated = 0;

        while (currentDate <= dateTo)
        {
               
            // Crea la transazione stimata
            var estimatedTransaction = new TransactionDto
            {
                Id = Guid.Empty, // ID temporaneo per la transazione stimata
                Description = recurring.Description,
                Date = currentDate,
                Type = recurring.Type,
                Amount = recurring.Amount,
                Status = TransactionStatus.Estimated,
                Notes = recurring.Notes,
                Account = recurring.Account,
                Category = recurring.Category,
                Recurring = recurring
            };

            if(recurring.RelatedRecurring != null)
            {
                var recurringService = new RecurringService(_mapper, _unitOfWork);
                var related = await recurringService.GetByIdAsync(recurring.RelatedRecurring.Id);
                estimatedTransaction.RelatedTransaction = new TransactionDto
                {
                    Id = Guid.Empty, // ID temporaneo per la transazione stimata correlata
                    Description = related.Description,
                    Date = currentDate,
                    Type = related.Type == TransactionType.Income ? TransactionType.Expense : TransactionType.Income,
                    Amount = recurring.Amount,
                    Status = TransactionStatus.Estimated,
                    Notes = related.Notes,
                    Account = related.Account,
                    Category = related.Category,
                    Recurring = related
                };
            }

            estimatedTransactions.Add(estimatedTransaction);

            // Calcola la prossima occorrenza
            currentDate = GetNextOccurrence(recurring, currentDate.AddDays(1));
            monthsGenerated++;
        }

        return estimatedTransactions;
    }
    
    /// <summary>
    /// Calcola la prossima occorrenza di una ricorrenza a partire da una data
    /// </summary>
    private DateTime GetNextOccurrence(RecurringDto recurring, DateTime fromDate)
    {
        var targetDay = recurring.ChargeDay;
        var currentMonth = fromDate.Month;
        var currentYear = fromDate.Year;
        
        // Se siamo prima del giorno target del mese corrente, usa il mese corrente
        if (fromDate.Day <= targetDay)
        {
            var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
            var actualDay = Math.Min(targetDay, daysInMonth);
            var candidateDate = new DateTime(currentYear, currentMonth, actualDay);
            
            if (candidateDate >= fromDate)
                return candidateDate;
        }
        
        // Altrimenti, vai al mese successivo
        currentMonth++;
        if (currentMonth > 12)
        {
            currentMonth = 1;
            currentYear++;
        }
        
        var nextMonthDays = DateTime.DaysInMonth(currentYear, currentMonth);
        var nextActualDay = Math.Min(targetDay, nextMonthDays);
        
        return new DateTime(currentYear, currentMonth, nextActualDay);
    }
}