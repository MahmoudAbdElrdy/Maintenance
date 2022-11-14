using AutoMapper;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.RequestsComplanit.Commands
{
    public class PutRequestComplanitCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? Description { get; set; }

        class PutRequestComplanit : IRequestHandler<PutRequestComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository;
            private readonly IGRepository<AttachmentComplanit> _AttachmentComplanitRepository;
            private readonly ILogger<PutRequestComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PutRequestComplanit(

                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository,
                IGRepository<AttachmentComplanit> AttachmentComplanitRepository,
                ILogger<PutRequestComplanitCommand> logger,
                IAuditService auditService,
                IMapper  mapper
            )
            {
                _RequestComplanitRepository = RequestComplanitRepository;
                _CheckListRequestRepository = CheckListRequestRepository;
                _AttachmentComplanitRepository = AttachmentComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
            }
            public async Task<ResponseDTO> Handle(PutRequestComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var RequestComplanit = new RequestComplanit()
                    {
                        UpdatedBy = _auditService.UserId,
                        UpdatedOn = DateTime.Now,
                        Description = request.Description,
                        Id =request.Id
                    };

                     _RequestComplanitRepository.Update(RequestComplanit);
                    _RequestComplanitRepository.Save();
                    _response.Result = _mapper.Map<RequestComplanitDto>(RequestComplanit);
                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "RequestComplanitUpdatedSuccessfully";

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
