using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Interfaces
{
    public interface IRoom
    {
        [Get("/api/1/GetRoomId/{id}")]
        Task<int> GetRoomId(long roomNumber);   
    }
}
