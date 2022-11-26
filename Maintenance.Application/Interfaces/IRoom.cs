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
        Task<RoomsDTO> GetRoomId(string roomNumber);
        [Get("/api/v1/Office/GetOffices")]
       // Task<List<OfficeDto>> GetOffices();   
        Task<List<OfficeDto>> GetOffices();   
    }
    public class OfficeDto
    {
        public string Name { get; set; }
        public string Code { get; set; } 
    }
    public class RoomsDTO
    {
        public long? Id { get; set; }
        public string? RoomNumber { get; set; }
       // public RoomType RoomType { get; set; }
        public string? QRImage { get; set; }
        public long? CarvanId { get; set; }
        public long? OfficeId { get; set; }
        public long? RegionId { get; set; }

    }
}
