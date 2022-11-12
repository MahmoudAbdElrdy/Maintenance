using Maintenance.Application.Features.Categories.Commands;
using Maintenance.Application.Features.RequestsComplanit;
using Maintenance.Application.Features.RequestsComplanit.Commands;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
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


        //[HttpGet]
        //public async Task<ResponseDTO> GetAll(int pageNumber, int pageSize)
        //{
        //    return await _mediator.Send(new GetAllRequestComplanitQuery()
        //    {
        //        PaginatedInputModel = new Application.Helpers.Paginations.PaginatedInputModel()
        //        {
        //            PageNumber = pageNumber,
        //            PageSize = pageSize
        //        },
        //    });
        //}

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
    }
}
