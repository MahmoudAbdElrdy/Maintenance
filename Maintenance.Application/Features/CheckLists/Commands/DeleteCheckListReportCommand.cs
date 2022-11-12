using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.CheckLists.Commands
{
    public class DeleteCheckListComplanitCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        class DeleteCheckListComplanit : IRequestHandler<DeleteCheckListComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<CheckListComplanit> _CheckListComplanitRepository;
            private readonly ILogger<DeleteCheckListComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            public DeleteCheckListComplanit(
              
                IGRepository<CheckListComplanit> CheckListComplanitRepository,
                ILogger<DeleteCheckListComplanitCommand> logger,
                 IAuditService auditService
            )
            {
                _CheckListComplanitRepository = CheckListComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _auditService = auditService;

            }
            public async Task<ResponseDTO> Handle(DeleteCheckListComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entityObj = await _CheckListComplanitRepository.GetAll(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (entityObj == null)
                    {

                        _response.StatusEnum = StatusEnum.FailedToFindTheObject;
                        _response.Message = "CheckListComplanitNotFound";
                    }

                    entityObj.UpdatedBy = _auditService.UserId;
                    entityObj.UpdatedOn = DateTime.Now;
                    entityObj.State = Domain.Enums.State.Deleted;
                    _CheckListComplanitRepository.Update(entityObj);
                    _CheckListComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CheckListComplanitRemovedSuccessfully";

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
