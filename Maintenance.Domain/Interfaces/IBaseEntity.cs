using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Interfaces
{
  public interface IBaseEntity
  {
    public long Id { get; set; }
  }
}
