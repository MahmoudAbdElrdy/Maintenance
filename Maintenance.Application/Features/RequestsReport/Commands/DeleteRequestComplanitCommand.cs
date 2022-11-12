using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.RequestsComplanit.Commands 
{
    public class DeleteRequestComplanitCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        class DeleteRequestComplanit : IRequestHandler<DeleteRequestComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
            private readonly ILogger<DeleteRequestComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            public DeleteRequestComplanit(
              
                IGRepository<RequestComplanit> RequestComplanitRepository,
                ILogger<DeleteRequestComplanitCommand> logger,
                 IAuditService auditService
            )
            {
                _RequestComplanitRepository = RequestComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _auditService = auditService;

            }
            public async Task<ResponseDTO> Handle(DeleteRequestComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entityObj = await _RequestComplanitRepository.GetAll(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (entityObj == null)
                    {

                        _response.StatusEnum = StatusEnum.FailedToFindTheObject;
                        _response.Message = "RequestComplanitNotFound";
                    }

                    entityObj.UpdatedBy = _auditService.UserId;
                    entityObj.UpdatedOn = DateTime.Now;
                    entityObj.State = Domain.Enums.State.Deleted;
                    _RequestComplanitRepository.Update(entityObj);
                    _RequestComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "RequestComplanitRemovedSuccessfully";

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
