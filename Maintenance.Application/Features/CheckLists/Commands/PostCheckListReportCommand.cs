﻿using AutoMapper;
using Maintenance.Application.Features.CheckLists.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.CheckLists.Commands
{
    public class PostCheckListReportCommand : IRequest<ResponseDTO>
    {
      
        public long? CategoryReportId { set; get; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        class PostCheckListReport : IRequestHandler<PostCheckListReportCommand, ResponseDTO>
        {
            private readonly IGRepository<CheckListReport> _CheckListReportRepository;
            private readonly ILogger<PostCheckListReportCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PostCheckListReport(

                IGRepository<CheckListReport> CheckListReportRepository,
                ILogger<PostCheckListReportCommand> logger,
                IAuditService auditService,
                IMapper mapper
            )
            {
                _CheckListReportRepository = CheckListReportRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;

            }
            public async Task<ResponseDTO> Handle(PostCheckListReportCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.CategoryReportId == null)
                    {
                        _response.StatusEnum = StatusEnum.Failed;
                        _response.Message = "MustSelectCategory";
                        _response.Result = null;
                        return _response;
                    }
                    var CheckListReport = new CheckListReport()
                    {
                        CreatedBy = _auditService.UserId,
                        CreatedOn = DateTime.Now,
                        State = Domain.Enums.State.NotDeleted,
                        DescriptionAr = request.DescriptionAr,
                        DescriptionEn = request.DescriptionEn,
                        NameAr = request.NameAr,
                        NameEn = request.NameEn,
                        CategoryReportId=request.CategoryReportId
                    };

                    await _CheckListReportRepository.AddAsync(CheckListReport);
                    _CheckListReportRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CheckListReportSavedSuccessfully";
                    _response.Result = _mapper.Map<CheckListReportDto>(CheckListReport);
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
