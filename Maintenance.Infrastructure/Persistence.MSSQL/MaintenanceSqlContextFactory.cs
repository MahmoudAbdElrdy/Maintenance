using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maintenance.Domain.Persistence;

namespace Maintenance.Infrastructure.Persistence.MSSQL
{
    public class MaintenanceSqlContextFactory : IDesignTimeDbContextFactory<MaintenanceSqlContext>
    {
        public MaintenanceSqlContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                // .AddJsonFile("appsettings.Stage.json", false)
               // .AddJsonFile("appsettings.Development.json", false)
                .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = config.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString, c => {
#if (Sql2008)
        c.UseRowNumberForPaging();
#endif
                c.MigrationsAssembly(typeof(MaintenanceSqlContext).Assembly.FullName);
            });

            return new MaintenanceSqlContext(builder.Options);
        }
    
    }
}
