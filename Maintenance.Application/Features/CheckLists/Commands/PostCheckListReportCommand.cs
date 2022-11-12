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
    public class PostCheckListComplanitCommand : IRequest<ResponseDTO>
    {
      
        public long? CategoryComplanitId { set; get; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        class PostCheckListComplanit : IRequestHandler<PostCheckListComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<CheckListComplanit> _CheckListComplanitRepository;
            private readonly ILogger<PostCheckListComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PostCheckListComplanit(

                IGRepository<CheckListComplanit> CheckListComplanitRepository,
                ILogger<PostCheckListComplanitCommand> logger,
                IAuditService auditService,
                IMapper mapper
            )
            {
                _CheckListComplanitRepository = CheckListComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;

            }
            public async Task<ResponseDTO> Handle(PostCheckListComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.CategoryComplanitId == null)
                    {
                        _response.StatusEnum = StatusEnum.Failed;
                        _response.Message = "MustSelectCategory";
                        _response.Result = null;
                        return _response;
                    }
                    var CheckListComplanit = new CheckListComplanit()
                    {
                        CreatedBy = _auditService.UserId,
                        CreatedOn = DateTime.Now,
                        State = Domain.Enums.State.NotDeleted,
                        DescriptionAr = request.DescriptionAr,
                        DescriptionEn = request.DescriptionEn,
                        NameAr = request.NameAr,
                        NameEn = request.NameEn,
                        CategoryComplanitId=request.CategoryComplanitId
                    };

                    await _CheckListComplanitRepository.AddAsync(CheckListComplanit);
                    _CheckListComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CheckListComplanitSavedSuccessfully";
                    _response.Result = _mapper.Map<CheckListComplanitDto>(CheckListComplanit);
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
