using Maintenance.Application.Features.Categories.Commands;
using Maintenance.Application.Features.Categories.Queries;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class CategoryReportController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public CategoryReportController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public async Task<ResponseDTO> GetAll(int pageNumber, int pageSize)
        {
            return await _mediator.Send(new GetAllCategoryReportQuery()
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
        public async Task<ResponseDTO> GetCategoryReportQueryById(long id) 
        {
            return await _mediator.Send(new GetCategoryReportQueryById() { Id = id });
        }

        [HttpPost("PostCategoryReport")]
      
        public async Task<ResponseDTO> PostCategoryReport([FromBody] PostCategoryReportCommand command)
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
        [HttpPut]
       
        public async Task<ResponseDTO> PutCategoryReport( [FromBody] PutCategoryReportCommand command)
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
            return await _mediator.Send(new DeleteCategoryReportCommand() { Id = id });
        }
    }
}
