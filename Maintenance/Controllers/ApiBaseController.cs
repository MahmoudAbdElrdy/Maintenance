using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApiBaseController : ControllerBase
    {
    }
}
