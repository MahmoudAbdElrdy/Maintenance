using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Interfaces
{
    public interface IDatabaseContext : IDisposable
    {
        public DatabaseFacade Database { get; }


        public ChangeTracker ChangeTracker { get; }

        public int SaveChanges();

        public Task<int> SaveChangesAsync();

        public DbSet<T> Set<T>() where T : class;

        public EntityEntry<T> Update<T>(T entity) where T : class;

        public void AddRange(IEnumerable<object> entities);

        public bool AllMigrationsApplied();

        public void Migrate();
    }
}
