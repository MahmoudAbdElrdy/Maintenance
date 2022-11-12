using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.Features.RequestsReport.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Commands
{
    public class PutRequestReportCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? Description { get; set; }

        class PutRequestReport : IRequestHandler<PutRequestReportCommand, ResponseDTO>
        {
            private readonly IGRepository<RequestReport> _RequestReportRepository;
            private readonly ILogger<PutRequestReportCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PutRequestReport(

                IGRepository<RequestReport> RequestReportRepository,
                ILogger<PutRequestReportCommand> logger,
                IAuditService auditService,
                IMapper  mapper
            )
            {
                _RequestReportRepository = RequestReportRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
            }
            public async Task<ResponseDTO> Handle(PutRequestReportCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var RequestReport = new RequestReport()
                    {
                        UpdatedBy = _auditService.UserId,
                        UpdatedOn = DateTime.Now,
                        Description = request.Description,
                        Id =request.Id
                    };

                     _RequestReportRepository.Update(RequestReport);
                    _RequestReportRepository.Save();
                    _response.Result = _mapper.Map<RequestReportDto>(RequestReport);
                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "RequestReportUpdatedSuccessfully";

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
