using Maintenance.Application.Features.CheckLists.Commands;
using Maintenance.Application.Features.CheckLists.Queries;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class CheckListComplanitController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public CheckListComplanitController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public async Task<ResponseDTO> GetAll(int pageNumber, int pageSize)
        {
            return await _mediator.Send(new GetAllCheckListComplanitQuery()
            {
                PaginatedInputModel = new Application.Helpers.Paginations.PaginatedInputModel()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                },
            });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ResponseDTO> GetCheckListComplanitQueryById(long id) 
        {
            return await _mediator.Send(new GetCheckListComplanitQueryById() { Id = id });
        }

        [HttpPost("PostCheckListComplanit")]
      
        public async Task<ResponseDTO> PostCheckListComplanit([FromBody] PostCheckListComplanitCommand command)
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
       
        [HttpPut("PutCheckListComplanit")]
        public async Task<ResponseDTO> PutCheckListComplanit( [FromBody] PutCheckListComplanitCommand command)
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
            return await _mediator.Send(new DeleteCheckListComplanitCommand() { Id = id });
        }
    }
}
