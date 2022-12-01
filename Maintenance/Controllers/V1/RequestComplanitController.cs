using Maintenance.Application.Features.Categories.Commands;
using Maintenance.Application.Features.Categories.Queries;
using Maintenance.Application.Features.RequestsComplanit;
using Maintenance.Application.Features.RequestsComplanit.Command;
using Maintenance.Application.Features.RequestsComplanit.Commands;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.Helper;
using Maintenance.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestComplanitController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public RequestComplanitController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }


      
        [HttpPost("GetComplanitsByStatus")]
        public async Task<ResponseDTO> GetComplanitsByStatus([FromBody] FilterComplanitDto filterComplanit)
        {
            return await _mediator.Send(new GetComplanitQueryByStatus()
            {
                PaginatedInputModel = new Application.Helpers.Paginations.PaginatedInputModel()
                {
                    PageNumber = filterComplanit.PageNumber,
                    PageSize = filterComplanit.PageSize,
                    
                },
                CategoryId= filterComplanit.CategoryId,
                RegionId= filterComplanit.RegionId,
                ComplanitStatus= filterComplanit.ComplanitStatus,
                OfficeId= filterComplanit.OfficeId
            });
        }    
        [HttpGet("GetComplanitQueryByCode")]
        public async Task<ResponseDTO> GetComplanitQueryByCode(int pageNumber, int pageSize, string code)
        {
            return await _mediator.Send(new GetComplanitQueryByCode()
            {
                PaginatedInputModel = new Application.Helpers.Paginations.PaginatedInputModel()
                {
                    PageNumber =pageNumber,
                    PageSize = pageSize,
                    
                },
              Code= code
            });
        }
        [HttpGet("GetComplanitDetailsQuery")]
        public async Task<ResponseDTO> GetComplanitDetailsQuery(int pageNumber, int pageSize,long requestComplanitId)
        {
            return await _mediator.Send(new GetComplanitDetailsQuery()
            {
                PaginatedInputModel = new Application.Helpers.Paginations.PaginatedInputModel()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,

                },
                RequestComplanitId = requestComplanitId
            });
        }

        //[HttpGet]
        //[Route("{id}")]
        //public async Task<ResponseDTO> GetRequestComplanitQueryById(long id)
        //{
        //    return await _mediator.Send(new GetRequestComplanitQueryById() { Id = id });
        //}

        [HttpPost("PostRequestComplanit")]

        public async Task<ResponseDTO> PostRequestComplanit([FromBody] PostRequestComplanitCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO()
                {
                    StatusEnum = StatusEnum.FailedToSave,
                    Message = "error"
                };
            }
            return await _mediator.Send(command);
        }  
        [HttpPost("PostListRequestComplanitCommand")]

        public async Task<ResponseDTO> PostListRequestComplanitCommand([FromBody] PostListRequestComplanitCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO()
                {
                    StatusEnum = StatusEnum.FailedToSave,
                    Message = "error"
                };
            }
            return await _mediator.Send(command);
        } 
        [HttpPost("PostComplanitHistory")]

        public async Task<ResponseDTO> PostApproveComplanitHistoryCommand([FromBody] PostApproveComplanitHistoryCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO()
                {
                    StatusEnum = StatusEnum.FailedToSave,
                    Message = "error"
                };
            }
            return await _mediator.Send(command);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("VerificationCodeComplanit")]
        public async Task<ResponseDTO> VerificationCode([FromBody] VerificationCodeComplanit command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("PutRequestComplanit")]
        public async Task<ResponseDTO> PutRequestComplanit([FromBody] PutRequestComplanitCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO()
                {
                    StatusEnum = StatusEnum.FailedToSave,
                    Message = "error"
                };
            }

            return await _mediator.Send(command);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResponseDTO> Delete(long id)
        {
            return await _mediator.Send(new DeleteRequestComplanitCommand() { Id = id });
        }

        [HttpPost("ReportQueryByDate")]

        public async Task<ResponseDTO> ReportQueryByDate([FromBody] ReportQueryByDate command)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO()
                {
                    StatusEnum = StatusEnum.FailedToSave,
                    Message = "error"
                };
            }
            return await _mediator.Send(command);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("addRequest")]
        public async Task<ResponseDTO> AddRequestForm([FromForm] PostRequestComplanitWebCommand command)
        {
            command.AttachmentsComplanit = Request.Form.Files;
            return await _mediator.Send(command);
        }

    }
}
