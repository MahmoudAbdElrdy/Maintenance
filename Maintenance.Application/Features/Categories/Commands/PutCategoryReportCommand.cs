using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Commands
{
    public class PutCategoryReportCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? Description { get; set; }
        class PutCategoryReport : IRequestHandler<PutCategoryReportCommand, ResponseDTO>
        {
            private readonly IGRepository<CategoryReport> _CategoryReportRepository;
            private readonly ILogger<PutCategoryReportCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PutCategoryReport(

                IGRepository<CategoryReport> CategoryReportRepository,
                ILogger<PutCategoryReportCommand> logger,
                IAuditService auditService,
                IMapper  mapper
            )
            {
                _CategoryReportRepository = CategoryReportRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
            }
            public async Task<ResponseDTO> Handle(PutCategoryReportCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var CategoryReport = new CategoryReport()
                    {
                        UpdatedBy = _auditService.UserId,
                        UpdatedOn = DateTime.Now,
                        Description = request.Description,
                        NameAr = request.NameAr,
                        NameEn = request.NameEn,
                        Id=request.Id
                    };

                     _CategoryReportRepository.Update(CategoryReport);
                    _CategoryReportRepository.Save();
                    _response.Result = _mapper.Map<CategoryReportDto>(CategoryReport);
                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CategoryReportUpdatedSuccessfully";

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
