using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Persistence.MSSQL
{
    public static class MaintenanceSqlServiceCollectionExtensions
    {
        public static IServiceCollection AddMssqlDbContext(
       this IServiceCollection serviceCollection,
       IConfiguration config = null)
        {


            serviceCollection.AddDbContext<AppDbContext, MaintenanceSqlContext>(options => {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                  b => b
#if Sql2008
          .UseRowNumberForPaging()
#endif
                  .MigrationsAssembly(typeof(MaintenanceSqlContext).Assembly.FullName));
            });
            return serviceCollection;
        }
    }
    //public static bool AllMigrationsApplied(this MaintenanceSqlContext context)
    //{
    //    var applied = context.GetService<IHistoryRepository>()
    //        .GetAppliedMigrations()
    //        .Select(m => m.MigrationId);

    //    var total = context.GetService<IMigrationsAssembly>()
    //        .Migrations
    //        .Select(m => m.Key);

    //    return !total.Except(applied).Any();
    //}
}

