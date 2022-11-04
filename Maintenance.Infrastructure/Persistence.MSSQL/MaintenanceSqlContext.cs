using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Persistence.MSSQL
{
    public class MaintenanceSqlContext : AppDbContext
    {
        public MaintenanceSqlContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
