using Maintenance.Application.Features.Categories.Commands;
using Maintenance.Application.Features.RequestsReport;
using Maintenance.Application.Features.RequestsReport.Commands;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestReportController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public RequestReportController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }


        //[HttpGet]
        //public async Task<ResponseDTO> GetAll(int pageNumber, int pageSize)
        //{
        //    return await _mediator.Send(new GetAllRequestReportQuery()
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
        //public async Task<ResponseDTO> GetRequestReportQueryById(long id)
        //{
        //    return await _mediator.Send(new GetRequestReportQueryById() { Id = id });
        //}

        [HttpPost("PostRequestReport")]

        public async Task<ResponseDTO> PostRequestReport([FromBody] PostRequestReportCommand command)
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

        [HttpPut("PutRequestReport")]
        public async Task<ResponseDTO> PutRequestReport([FromBody] PutRequestReportCommand command)
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
            return await _mediator.Send(new DeleteRequestReportCommand() { Id = id });
        }
    }
}
