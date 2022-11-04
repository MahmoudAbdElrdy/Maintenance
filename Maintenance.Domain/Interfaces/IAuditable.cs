using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Interfaces 
{
  public interface IAuditable
  {
    long? CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
    long? UpdatedBy { get; set; }
    DateTime? UpdatedOn { get; set; }
  }
}
