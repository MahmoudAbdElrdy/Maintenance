using AutoMapper;
using Maintenance.Application.Features.CheckLists.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.CheckLists.Commands
{
    public class PutCheckListComplanitCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        public long? CategoryComplanitId { set; get; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        class PutCheckListComplanit : IRequestHandler<PutCheckListComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<CheckListComplanit> _CheckListComplanitRepository;
            private readonly ILogger<PutCheckListComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PutCheckListComplanit(

                IGRepository<CheckListComplanit> CheckListComplanitRepository,
                ILogger<PutCheckListComplanitCommand> logger,
                IAuditService auditService,
                IMapper  mapper
            )
            {
                _CheckListComplanitRepository = CheckListComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
            }
            public async Task<ResponseDTO> Handle(PutCheckListComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var CheckListComplanit = new CheckListComplanit()
                    {
                        UpdatedBy = _auditService.UserId,
                        UpdatedOn = DateTime.Now,
                        DescriptionAr = request.DescriptionAr,
                        DescriptionEn = request.DescriptionEn,
                        NameAr = request.NameAr,
                        NameEn = request.NameEn,
                        Id=request.Id,
                        CategoryComplanitId = request.CategoryComplanitId
                    };

                     _CheckListComplanitRepository.Update(CheckListComplanit);
                    _CheckListComplanitRepository.Save();
                    _response.Result = _mapper.Map<CheckListComplanitDto>(CheckListComplanit);
                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CheckListComplanitUpdatedSuccessfully";

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
