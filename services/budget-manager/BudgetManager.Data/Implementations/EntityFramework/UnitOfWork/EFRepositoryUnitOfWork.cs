using BudgetManager.Data.Abstraction.Models.Dto;
using BudgetManager.Data.Abstraction.Repositories;
using BudgetManager.Data.Abstraction.UnitOfWork;
using BudgetManager.Data.Abstractions.Factories;
using BudgetManager.Data.Abstractions.Models.DomainModels;
using BudgetManager.Data.Models.Dto.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using SecondBrain.Data.Abstraction.Repositories;

namespace BudgetManager.Data.Implementations.EntityFramework.UnitOfWork
{
    public class EFRepositoryUnitOfWork : IRepositoryUnitOfWork
    {
        private readonly EFDbcontext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        // Lazy-loaded repositories
        private IAccountRepository? _accountRepository;
        private ITransactionRepository? _transactionRepository;
        private ICategoryRepository? _categoryRepository;
        private IRecurringRepository? _recurringRepository;

        public EFRepositoryUnitOfWork(EFDbcontext context, IRepositoryFactory repositoryFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public ICrudRepository<T, TFilter, TForm> GetCrudRepository<T, TFilter, TForm>()
            where T : class, IBaseDomainModel
            where TFilter : class, IListFilter
            where TForm : class, IFormDto
        {
            return _repositoryFactory.CreateCrudRepository<T, TFilter, TForm>();
        }

        public IAccountRepository AccountRepository => 
            _accountRepository ??= _repositoryFactory.CreateAccountRepository();
            
        public ITransactionRepository TransactionRepository =>
            _transactionRepository ??= _repositoryFactory.CreateTransactionRepository();
            
        public ICategoryRepository CategoryRepository =>
            _categoryRepository ??= _repositoryFactory.CreateCategoryRepository();
            
        public IRecurringRepository recurringRepository =>
            _recurringRepository ??= _repositoryFactory.CreateRecurringRepository();

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}