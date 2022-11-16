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
    public class PostRequestComplanitCommand : IRequest<ResponseDTO>
    {
        
        //public string? Description { get; set; }
        //public long[]? CheckListsRequest { get; set; }
        //public string[]? AttachmentsComplanit { get; set; }
        public List<RequestComplanitDto> requests { get; set; }
        public long? OfficeId { get; set; }
        public long? RegionId { get; set; }
        class PostRequestComplanit : IRequestHandler<PostRequestComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
            private readonly ILogger<PostRequestComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
          
            public PostRequestComplanit(

                IGRepository<RequestComplanit> RequestComplanitRepository,
                ILogger<PostRequestComplanitCommand> logger,
                IAuditService auditService,
                IMapper mapper
            )
            {
                _RequestComplanitRepository = RequestComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
              
            }
            public async Task<ResponseDTO> Handle(PostRequestComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var RequestComplanitList = new List<RequestComplanit>();

                    foreach (var requestObj in request.requests)
                    {
                        var RequestComplanit = new RequestComplanit()
                        {
                            CreatedBy = _auditService.UserId,
                            CreatedOn = DateTime.Now,
                            State = Domain.Enums.State.NotDeleted,
                            Description = requestObj.Description,
                            OfficeId=request.OfficeId,
                            RegionId = request.RegionId
                        };

                        foreach (var item in requestObj.AttachmentsComplanit)
                        {
                            RequestComplanit.AttachmentsComplanit.Add(new AttachmentComplanit()
                            {
                                Path = item,
                                CreatedBy = _auditService.UserId,
                                CreatedOn = DateTime.Now,
                            });
                        }
                        foreach (var item in requestObj.CheckListsRequest)
                        {
                            RequestComplanit.CheckListRequests.Add(new CheckListRequest()
                            {
                                CheckListComplanitId = item,
                                CreatedBy = _auditService.UserId,
                                CreatedOn = DateTime.Now,
                            });
                        }
                        RequestComplanitList.Add(RequestComplanit);

                    }


                    await _RequestComplanitRepository.AddRangeAsync(RequestComplanitList);
                    _RequestComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "RequestComplanitSavedSuccessfully";
                    _response.Result = null;
                    return _response;
                }
                catch (Exception ex)
                {
                    foreach (var requestObj in request.requests)
                    {
                        if (requestObj.AttachmentsComplanit.Length > 0)
                        {
                            var folderName = Path.Combine("wwwroot/Uploads/Complanits");

                            foreach (var fileRemove in requestObj.AttachmentsComplanit)
                            {
                                var file = System.IO.Path.Combine(folderName, fileRemove);
                                try
                                {
                                    System.IO.File.Delete(file);
                                }
                                catch { }
                            }


                        }

                    }
                       
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
