using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Commands
{
    public class DeleteCategoryComplanitCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        class DeleteCategoryComplanit : IRequestHandler<DeleteCategoryComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<CategoryComplanit> _CategoryComplanitRepository;
            private readonly ILogger<DeleteCategoryComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            public DeleteCategoryComplanit(
              
                IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<DeleteCategoryComplanitCommand> logger,
                 IAuditService auditService
            )
            {
                _CategoryComplanitRepository = CategoryComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _auditService = auditService;

            }
            public async Task<ResponseDTO> Handle(DeleteCategoryComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entityObj = await _CategoryComplanitRepository.GetAll(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (entityObj == null)
                    {

                        _response.StatusEnum = StatusEnum.FailedToFindTheObject;
                        _response.Message = "CategoryComplanitNotFound";
                    }

                    entityObj.UpdatedBy = _auditService.UserId;
                    entityObj.UpdatedOn = DateTime.Now;
                    entityObj.State = Domain.Enums.State.Deleted;
                    _CategoryComplanitRepository.Update(entityObj);
                    _CategoryComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CategoryComplanitRemovedSuccessfully";

                    return _response;
                }
                catch (Exception ex)
                {
                    _response.StatusEnum = StatusEnum.Exception;
                    _response.Result = null;
                    _response.Message = (ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                    _logger.LogError(ex, ex.Message, (ex != null && ex.InnerException != null ? ex.InnerException.Message : ""));

                    return _response;
                }
            }

        }
    }
  
}
