using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.RequestsReport.Commands 
{
    public class DeleteRequestReportCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        class DeleteRequestReport : IRequestHandler<DeleteRequestReportCommand, ResponseDTO>
        {
            private readonly IGRepository<RequestReport> _RequestReportRepository;
            private readonly ILogger<DeleteRequestReportCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            public DeleteRequestReport(
              
                IGRepository<RequestReport> RequestReportRepository,
                ILogger<DeleteRequestReportCommand> logger,
                 IAuditService auditService
            )
            {
                _RequestReportRepository = RequestReportRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _auditService = auditService;

            }
            public async Task<ResponseDTO> Handle(DeleteRequestReportCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entityObj = await _RequestReportRepository.GetAll(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (entityObj == null)
                    {

                        _response.StatusEnum = StatusEnum.FailedToFindTheObject;
                        _response.Message = "RequestReportNotFound";
                    }

                    entityObj.UpdatedBy = _auditService.UserId;
                    entityObj.UpdatedOn = DateTime.Now;
                    entityObj.State = Domain.Enums.State.Deleted;
                    _RequestReportRepository.Update(entityObj);
                    _RequestReportRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "RequestReportRemovedSuccessfully";

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
