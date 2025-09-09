using System.Data;
using System.Reflection.Emit;
using System.Security;
using BudgetManager.Data.Customizations.Identity.Enums;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Data.Implementations.EntityFramework
{
    public class EFDbcontext : IdentityDbContext<EFUserEntity, EFRoleEntity, Guid>
    {
        public EFDbcontext(DbContextOptions<EFDbcontext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        /* Per fare le migration nel terminale, va specificato il progetto dove stanno le migration e il progetto di avvio
         * dotnet ef [operazione] --project BudgetManager.Data/BudgetManager.Data.csproj --startup-project BudgetManager.Api/BudgetManager.Api.csproj
         */

        protected override async void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<EFCategoryEntity>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Impedisce l'eliminazione di una categoria padre se ha sottocategorie
            


            builder
                .Entity<EFUserEntity>()
                .HasMany(user => user.Roles)
                .WithMany(role => role.Users)
                .UsingEntity<IdentityUserRole<Guid>>(
                    builder => builder.HasOne<EFRoleEntity>().WithMany().HasForeignKey(userRole => userRole.RoleId),
                    builder => builder.HasOne<EFUserEntity>().WithMany().HasForeignKey(userRole => userRole.UserId),
                    builder => builder.ToTable("AspNetUserRoles")
                );

            builder
                .Entity<EFRoleEntity>()
                .HasMany(role => role.RoleClaims)
                .WithOne()
                .HasForeignKey(claim => claim.RoleId);


            #region Roles
            var admin = new EFRoleEntity()
                {
                    Id = Guid.NewGuid(),
                    Name = Role.Superadmin.ToString(),
                    NormalizedName = "Amministratore",
                    Description = "Amministratore"
                };

            builder.Entity<EFRoleEntity>().HasData(
                admin,

                new EFRoleEntity
                {
                    Id = Guid.NewGuid(),
                    Name = Role.User.ToString(),
                    NormalizedName = "Utente",
                    Description = "Utente"
                },
                new EFRoleEntity
                {
                    Id = Guid.NewGuid(),
                    Name = Role.FamilyAdmin.ToString(),
                    NormalizedName = "Amministratore Famiglia",
                    Description = "Amministratore Famiglia"
                }
            );
            #endregion

            #region Permission
            builder.Entity<IdentityRoleClaim<Guid>>().HasData(

                new IdentityRoleClaim<Guid>
                {
                    Id = 1,
                    RoleId = admin.Id,
                    ClaimType = "Permission",
                    ClaimValue = Permission.AccessAdminArea.ToString()
                },

                new IdentityRoleClaim<Guid>
                {
                    Id = 2,
                    RoleId = admin.Id,
                    ClaimType = "Permission",
                    ClaimValue = Permission.AssignRoles.ToString()
                },

                new IdentityRoleClaim<Guid>
                {
                    Id =3,
                    RoleId = admin.Id,
                    ClaimType = "Permission",
                    ClaimValue = Permission.EditRole.ToString()
                }
            );
            #endregion
        }

        public DbSet<EFUserEntity> ApplicationUsers { get; set; }
        public DbSet<EFRoleEntity> ApplicationRoles { get; set; }
        public DbSet<EFAccountEntity> Accounts { get; set; }
        public DbSet<EFRecurringEntity> Recurrings { get; set; }
        public DbSet<EFTransactionEntity> Transactions { get; set; }
        public DbSet<EFCategoryEntity> Categories { get; set; }
    }
}
