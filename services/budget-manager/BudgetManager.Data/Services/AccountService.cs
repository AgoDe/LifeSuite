using AutoMapper;
using BudgetManager.Data.Abstraction.Validation;
using BudgetManager.Data.Models.Dto;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Exceptions;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Forms;
using BudgetManager.Data.Models.Enums;

namespace BudgetManager.Data.Services
{
    /// <summary>
    /// Servizio per la gestione degli Account
    /// Implementa la logica business specifica per gli account
    /// </summary>
    public class AccountService : BaseCrudService<AccountDM, AccountDto, AccountFormDto, AccountFilter>
    {
        public AccountService(
            IRepositoryUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Validazioni specifiche per la creazione di un account
        /// </summary>
        protected override async Task ValidateForCreateAsync(AccountFormDto formDto)
        {
            await base.ValidateForCreateAsync(formDto);
            
            // Validazione: verifica che l'utente esista (se necessario)
            // Questa logica potrebbe richiedere un repository utenti
            
            // Validazione: verifica unicità nome account per utente
            // Implementazione dipende dal repository specifico
        }

        /// <summary>
        /// Validazioni specifiche per l'aggiornamento di un account
        /// </summary>
        protected override async Task ValidateForUpdateAsync(Guid id, AccountFormDto formDto, AccountDM existingEntity)
        {
            await base.ValidateForUpdateAsync(id, formDto, existingEntity);
            
            // Validazione: verifica unicità nome account per utente (escludendo l'account corrente)
        }

        /// <summary>
        /// Validazioni specifiche per la cancellazione di un account
        /// </summary>
        protected override async Task ValidateForDeleteAsync(AccountDM entity)
        {
            await base.ValidateForDeleteAsync(entity);
            
            // Validazione: verifica che l'account non abbia transazioni associate
            // Questa logica richiederebbe un repository transazioni
            
            // Validazione: verifica che il saldo sia zero
            if (entity.Balance != 0)
            {
                throw new BusinessRuleException("Non è possibile eliminare un account con saldo diverso da zero");
            }
        }

        /// <summary>
        /// Validazioni delle regole business specifiche per gli account
        /// </summary>
        protected override async Task ValidateBusinessRulesAsync(AccountFormDto formDto, ValidationResult result)
        {
            await base.ValidateBusinessRulesAsync(formDto, result);
            
            // Validazione: nome account non vuoto dopo trim
            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                result.AddError("Name", "Il nome dell'account non può essere vuoto");
            }
            
            // Validazione: saldo iniziale ragionevole
            if (formDto.Balance < -1000000 || formDto.Balance > 1000000)
            {
                result.AddError("Balance", "Il saldo deve essere compreso tra -1,000,000 e 1,000,000");
            }
        }

        /// <summary>
        /// Hook eseguito prima della creazione
        /// Imposta valori di default e calcoli
        /// </summary>
        protected override async Task BeforeCreateAsync(AccountDM entity, AccountFormDto formDto)
        {
            await base.BeforeCreateAsync(entity, formDto);
            
            // Imposta il saldo corrente uguale al saldo iniziale
            entity.Balance = entity.InitialBalance;
            entity.BalanceDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Hook eseguito dopo la creazione
        /// </summary>
        protected override async Task AfterCreateAsync(AccountDM entity, AccountFormDto formDto, Guid id)
        {
            await base.AfterCreateAsync(entity, formDto, id);
            
            // Log dell'operazione
            // Notifiche
            // Altre operazioni post-creazione
        }

        /// <summary>
        /// Hook eseguito prima dell'aggiornamento
        /// </summary>
        protected override async Task BeforeUpdateAsync(AccountDM entity, AccountFormDto formDto)
        {
            await base.BeforeUpdateAsync(entity, formDto);
            
            // Mantieni il saldo corrente (non modificabile tramite form)
            // Il saldo viene modificato solo tramite transazioni
            
            // Aggiorna solo i campi modificabili
            // Balance e BalanceDate rimangono invariati
        }

        /// <summary>
        /// Metodi specifici per Account (estensioni del servizio base)
        /// </summary>
        
        /// <summary>
        /// Aggiorna il saldo di un account
        /// </summary>
        public async Task UpdateBalanceAsync(Guid accountId, decimal amount, TransactionType type)
        {
            var account = await _repository.GetByIdAsync(accountId);
            if (account == null)
                throw new EntityNotFoundException($"Account with ID {accountId} not found");

            switch (type)
            {
                case TransactionType.Income:
                    account.Balance += amount;
                    break;
                case TransactionType.Expense:
                    account.Balance -= amount;
                    break;
                // case TransactionType.Transfer:
                //     // La logica di trasferimento richiede gestione speciale
                //     throw new NotImplementedException("Transfer logic not implemented in this method");
                default:
                    throw new ArgumentException($"Unsupported transaction type: {type}");
            }

            account.BalanceDate = DateTime.UtcNow;
            await _repository.UpdateAsync(account);
        }

        /// <summary>
        /// Recupera tutti gli account di un utente
        /// </summary>
        public async Task<ICollection<AccountDto>> GetAccountsByUserIdAsync(Guid userId)
        {
            var filter = new AccountFilter
            {
                UserId = userId,
                PageSize = int.MaxValue // Recupera tutti gli account
            };

            var result = await GetListAsync(filter);
            return result.Items;
        }
        
        public async Task<List<SelectOption>> GetSelectOptions(Guid userId)
        {
            var accounts = await _unitOfWork.AccountRepository.GetListByUserId(userId);

            return accounts
                .Select(a => new SelectOption
                {
                    Title = a.Name,
                    Value = a.Id.ToString(),
                    ParentTitle = a.Institution
                })
                .OrderBy(opt => opt.Title)
                .OrderBy(opt => opt.ParentTitle)
                .ToList();
        }
    }
}