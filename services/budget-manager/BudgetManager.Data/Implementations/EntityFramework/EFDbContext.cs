using BudgetManager.Data.Implementations.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Data.Implementations.EntityFramework
{
    public class EFDbcontext : DbContext
    {
        public EFDbcontext(DbContextOptions<EFDbcontext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        /* Per fare le migration nel terminale, va specificato il progetto dove stanno le migration e il progetto di avvio
         * dotnet ef [operazione] --project BudgetManager.Data/BudgetManager.Data.csproj --startup-project budget-manager/BudgetManager.Api.csproj
         */

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EFCategoryEntity>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Impedisce l'eliminazione di una categoria padre se ha sottocategorie
        }

        public DbSet<EFAccountEntity> Accounts { get; set; }
        public DbSet<EFRecurringEntity> Recurrings { get; set; }
        public DbSet<EFTransactionEntity> Transactions { get; set; }
        public DbSet<EFCategoryEntity> Categories { get; set; }
    }
}