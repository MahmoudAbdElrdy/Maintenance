using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Commands
{
    public class DeleteCategoryReportCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        class DeleteCategoryReport : IRequestHandler<DeleteCategoryReportCommand, ResponseDTO>
        {
            private readonly IGRepository<CategoryReport> _CategoryReportRepository;
            private readonly ILogger<DeleteCategoryReportCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            public DeleteCategoryReport(
              
                IGRepository<CategoryReport> CategoryReportRepository,
                ILogger<DeleteCategoryReportCommand> logger,
                 IAuditService auditService
            )
            {
                _CategoryReportRepository = CategoryReportRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _auditService = auditService;

            }
            public async Task<ResponseDTO> Handle(DeleteCategoryReportCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entityObj = await _CategoryReportRepository.GetAll(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (entityObj == null)
                    {

                        _response.StatusEnum = StatusEnum.FailedToFindTheObject;
                        _response.Message = "CategoryReportNotFound";
                    }

                    entityObj.UpdatedBy = _auditService.UserId;
                    entityObj.UpdatedOn = DateTime.Now;
                    entityObj.State = Domain.Enums.State.Deleted;
                    _CategoryReportRepository.Update(entityObj);
                    _CategoryReportRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CategoryReportRemovedSuccessfully";

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
