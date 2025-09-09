using AutoMapper;
using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Abstractions.Factories;
using BudgetManager.Data.Abstractions.Models.DomainModels;
using BudgetManager.Data.Implementations.EntityFramework.Repositories;
using BudgetManager.Data.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Filters;
using BudgetManager.Data.Models.Dto.Interfaces;
using BudgetManager.Data.Services;
using SecondBrain.Data.Abstraction.Repositories;

namespace BudgetManager.Data.Implementations.EntityFramework.Factories
{
    public class EFRepositoryFactory : IRepositoryFactory
    {
        private readonly EFDbcontext _context;
        private readonly IMapper _mapper;
        private readonly DateService _dateService;

        public EFRepositoryFactory(EFDbcontext context, IMapper mapper, DateService dateService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));
        }

        public ICrudRepository<T, TFilter, TForm> CreateCrudRepository<T, TFilter, TForm>()
            where T : class, IBaseDomainModel
            where TFilter : class, IListFilter
            where TForm : class, IFormDto
        {
            // Determina il tipo di repository da creare in base al tipo dell'entità
            if (typeof(T) == typeof(AccountDM) && typeof(TFilter) == typeof(AccountFilter))
            {
                return (ICrudRepository<T, TFilter, TForm>)CreateAccountRepository();
            }
            
            if (typeof(T) == typeof(TransactionDM) && typeof(TFilter) == typeof(TransactionFilter))
            {
                return (ICrudRepository<T, TFilter, TForm>)CreateTransactionRepository();
            }
            
            if (typeof(T) == typeof(CategoryDM) && typeof(TFilter) == typeof(CategoryFilter))
            {
                return (ICrudRepository<T, TFilter, TForm>)CreateCategoryRepository();
            }
            
            if (typeof(T) == typeof(RecurringDM) && typeof(TFilter) == typeof(RecurringFilter))
            {
                return (ICrudRepository<T, TFilter, TForm>)CreateRecurringRepository();
            }

            throw new NotSupportedException($"Repository not implemented for entity type {typeof(T).Name} and filter type {typeof(TFilter).Name}");
        }

        public IAccountRepository CreateAccountRepository()
        {
            return new EFAccountRepository(_context, _mapper);
        }

        public ITransactionRepository CreateTransactionRepository()
        {
            return new EFTransactionRepository(_context, _mapper);
        }

        public ICategoryRepository CreateCategoryRepository()
        {
            return new EFCategoryRepository(_context, _mapper);
        }

        public IRecurringRepository CreateRecurringRepository()
        {
            return new EFRecurringRepository(_context, _mapper, _dateService);
        }
    }
}