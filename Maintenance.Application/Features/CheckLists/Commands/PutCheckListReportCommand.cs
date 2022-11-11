using AutoMapper;
using Maintenance.Application.Features.CheckLists.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.CheckLists.Commands
{
    public class PutCheckListReportCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        public long? CategoryReportId { set; get; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        class PutCheckListReport : IRequestHandler<PutCheckListReportCommand, ResponseDTO>
        {
            private readonly IGRepository<CheckListReport> _CheckListReportRepository;
            private readonly ILogger<PutCheckListReportCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PutCheckListReport(

                IGRepository<CheckListReport> CheckListReportRepository,
                ILogger<PutCheckListReportCommand> logger,
                IAuditService auditService,
                IMapper  mapper
            )
            {
                _CheckListReportRepository = CheckListReportRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
            }
            public async Task<ResponseDTO> Handle(PutCheckListReportCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var CheckListReport = new CheckListReport()
                    {
                        UpdatedBy = _auditService.UserId,
                        UpdatedOn = DateTime.Now,
                        DescriptionAr = request.DescriptionAr,
                        DescriptionEn = request.DescriptionEn,
                        NameAr = request.NameAr,
                        NameEn = request.NameEn,
                        Id=request.Id,
                        CategoryReportId = request.CategoryReportId
                    };

                     _CheckListReportRepository.Update(CheckListReport);
                    _CheckListReportRepository.Save();
                    _response.Result = _mapper.Map<CheckListReportDto>(CheckListReport);
                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CheckListReportUpdatedSuccessfully";

                    return _response;
                }
                catch (Exception ex)
                {
                    _response.StatusEnum = StatusEnum.Exception;
                    _response.Result = null;
                    _response.Message = ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _logger.LogError(ex, ex.Message, ex != null && ex.InnerException != null ? ex.InnerException.Message : "");

                    return _response;
                }
            }

        }
    }
}
