﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Entities.Reports;

namespace Maintenance.Domain.Interfaces 
{
    public interface IDatabaseContext : IDisposable
    {
        public DatabaseFacade Database { get; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<CategoryReport> CategoriesReport { get; set; }
        public DbSet<CheckListReport> CheckListsReport { get; set; }
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
