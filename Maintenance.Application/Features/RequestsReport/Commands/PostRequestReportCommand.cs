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
    public class PostRequestReportCommand : IRequest<ResponseDTO>
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? Description { get; set; }
        public long[]? CheckListsRequest { get; set; }
        public long[]? AttachmentsReport { get; set; }
        class PostRequestReport : IRequestHandler<PostRequestReportCommand, ResponseDTO>
        {
            private readonly IGRepository<RequestReport> _RequestReportRepository;
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository; 
            private readonly IGRepository<AttachmentReport> _AttachmentReportRepository;  
            private readonly ILogger<PostRequestReportCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PostRequestReport(

                IGRepository<RequestReport> RequestReportRepository,
                ILogger<PostRequestReportCommand> logger,
                IAuditService auditService,
                IMapper mapper,
                IGRepository<CheckListRequest>  CheckListRequestRepository,
                IGRepository<AttachmentReport> AttachmentReportRepository
            )
            {
                _RequestReportRepository = RequestReportRepository;
                _CheckListRequestRepository = CheckListRequestRepository;
                _AttachmentReportRepository =  AttachmentReportRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;

            }
            public async Task<ResponseDTO> Handle(PostRequestReportCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var RequestReport = new RequestReport()
                    {
                        CreatedBy = _auditService.UserId,
                        CreatedOn = DateTime.Now,
                        State = Domain.Enums.State.NotDeleted,
                        Description = request.Description,
                    };

                    await _RequestReportRepository.AddAsync(RequestReport);
                    _RequestReportRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "RequestReportSavedSuccessfully";
                    _response.Result = _mapper.Map<RequestReportDto>(RequestReport);
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
