using Maintenance.Application.Features.Categories.Commands;
using Maintenance.Application.Features.Categories.Queries;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class CategoryComplanitController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public CategoryComplanitController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public async Task<ResponseDTO> GetAll(int pageNumber, int pageSize)
        {
            return await _mediator.Send(new GetAllCategoryComplanitQuery()
            {
                PaginatedInputModel = new Application.Helpers.Paginations.PaginatedInputModel()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                },
            });
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("GetAllCategories")]
        public async Task<ResponseDTO> GetAll()
        {
            return await _mediator.Send(new GetAllCategoriesComplanitsQuery(){});
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ResponseDTO> GetCategoryComplanitQueryById(long id) 
        {
            return await _mediator.Send(new GetCategoryComplanitQueryById() { Id = id });
        }

        [HttpPost("PostCategoryComplanit")]
      
        public async Task<ResponseDTO> PostCategoryComplanit([FromBody] PostCategoryComplanitCommand command)
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
      
        [HttpPut("PutCategoryComplanit")]
        public async Task<ResponseDTO> PutCategoryComplanit( [FromBody] PutCategoryComplanitCommand command)
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
            return await _mediator.Send(new DeleteCategoryComplanitCommand() { Id = id });
        }
    }
}
