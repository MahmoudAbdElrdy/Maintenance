using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AuthDomain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Maintenance.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Entities.Complanits;

namespace Maintenance.Domain.Persistence 
{
    public class AppDbContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>,
      UserRole,IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>, IDatabaseContext

    {

       
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<CategoryComplanit> CategoriesComplanit { get; set; }
        public DbSet<CheckListComplanit> CheckListsComplanit { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<RequestComplanit> RequestComplanit { get; set; }
        public DbSet<AttachmentComplanit> AttachmentComplanit { get; set; }
        public DbSet<AttachmentComplanitHistory> AttachmentComplanitHistory { get; set; }
        public DbSet<ComplanitHistory> ComplanitHistory { get; set; }
        public DbSet<CheckListRequest> CheckListRequest { get; set; }
        public DbSet<ComplanitFilter> ComplanitFilters { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
          
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>().HasKey(p => new { p.UserId, p.RoleId });
           modelBuilder.Entity<PermissionRole>().HasKey(p => new { p.RoleId, p.PermissionId });
            modelBuilder.Entity<User>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
          
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedOn = DateTime.Now;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public bool AllMigrationsApplied()
        {
            var applied = this.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = this.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public void Migrate()
        {
            this.Database.Migrate();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync();
        }
    }
}
