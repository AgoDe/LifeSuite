using AutoMapper;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Exceptions;
using BudgetManager.Data.Abstraction.Validation;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Services;
public class RecurringService : BaseCrudService<RecurringDM, RecurringDto, RecurringFormDto, RecurringFilter>
{
    private IMapper _mapper;
    
    public RecurringService(
        IMapper mapper, 
        IRepositoryUnitOfWork repositoryUow
    ) : base(repositoryUow, mapper)
    {
        _mapper = mapper;
    }
    
    public override async Task<Guid> CreateAsync(RecurringFormDto formDto)
    {
        if (formDto == null)
            throw new ArgumentNullException(nameof(formDto));

        // Validazione business
        var validationResult = await ValidateWithDetailsAsync(formDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Validazioni specifiche pre-creazione
        await ValidateForCreateAsync(formDto);

        Guid recurringId;
        if(formDto.RelatedAccountId.HasValue)
        {
            var guids = await _unitOfWork.recurringRepository.CreateTransferRecurring(formDto);
            recurringId = guids.Item1;
        }
        else
        {
            recurringId = await _repository.CreateAsync(formDto);
        }

        return recurringId;
    }
    
    protected override async Task ValidateForCreateAsync(RecurringFormDto formDto)
    {
        // Validazione che relatedAccountId non sia uguale ad accountId
        if (formDto.RelatedAccountId.HasValue && formDto.RelatedAccountId.Value == formDto.AccountId)
            throw new BusinessRuleException("RelatedAccountId cannot be the same as AccountId");
            
        // Chiama la validazione base
        await base.ValidateForCreateAsync(formDto);
    }
    
    protected override async Task ValidateBusinessRulesAsync(RecurringFormDto formDto, ValidationResult result)
    {
        await base.ValidateBusinessRulesAsync(formDto, result);
        
        // Validazione: descrizione obbligatoria
        if (string.IsNullOrWhiteSpace(formDto.Description))
        {
            result.AddError("Description", "La descrizione è obbligatoria");
        }
        else if (formDto.Description.Length > 200)
        {
            result.AddError("Description", "La descrizione non può superare i 200 caratteri");
        }
        
        // Validazione: istituzione
        if (!string.IsNullOrEmpty(formDto.Institution) && formDto.Institution.Length > 100)
        {
            result.AddError("Institution", "L'istituzione non può superare i 100 caratteri");
        }
        
        // Validazione: note
        if (!string.IsNullOrEmpty(formDto.Notes) && formDto.Notes.Length > 1000)
        {
            result.AddError("Notes", "Le note non possono superare i 1000 caratteri");
        }
        
        // Validazione: data di inizio obbligatoria
        if (formDto.ActiveFrom == default(DateTime))
        {
            result.AddError("ActiveFrom", "La data di inizio è obbligatoria");
        }
        
        // Validazione: giorno di addebito
        if (formDto.ChargeDay.HasValue && (formDto.ChargeDay.Value < 1 || formDto.ChargeDay.Value > 31))
        {
            result.AddError("ChargeDay", "Il giorno di addebito deve essere compreso tra 1 e 31");
        }
        
        // Validazione: importo obbligatorio e positivo
        if (formDto.Amount <= 0)
        {
            result.AddError("Amount", "L'importo deve essere maggiore di zero");
        }
        
        // Validazione: RelatedAccountId non può essere uguale ad AccountId
        if (formDto.RelatedAccountId.HasValue && formDto.RelatedAccountId.Value == formDto.AccountId)
        {
            result.AddError("RelatedAccountId", "L'account correlato non può essere lo stesso dell'account principale");
        }
        
        // Validazione: data di fine deve essere successiva alla data di inizio
        if (formDto.ActiveTo.HasValue && formDto.ActiveTo.Value <= formDto.ActiveFrom)
        {
            result.AddError("ActiveTo", "La data di fine deve essere successiva alla data di inizio");
        }
    }
}