using Maintenance.Application.Helper;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Interfaces
{
    public interface IRoom
    {
        [Get("/api/v1/Room/GetRoomIdByRoomNumber/{roomNumber}")]
        Task<long> GetRoomId(long roomNumber);   
    }
}
