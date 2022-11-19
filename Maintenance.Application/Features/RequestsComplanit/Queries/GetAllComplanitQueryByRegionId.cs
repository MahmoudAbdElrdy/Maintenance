using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Application.Helpers.QueryableExtensions;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetAllComplanitQueryByRegionId : IRequest<ResponseDTO>
    {
        public GetAllComplanitQueryByRegionId() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public long? CategoryId { get; set; } 
        public long? RegionId { get; set; }  
        public long? OfficeId { get; set; }  

        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryComplanit : IRequestHandler<GetAllComplanitQueryByRegionId, ResponseDTO>
        {
            private readonly IGRepository<CategoryComplanit> _CategoryComplanitRepository;
            private readonly IGRepository<ComplanitHistory> _ComplanitHistoryRepository;
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository;
            private readonly IGRepository<CheckListComplanit> _CheckListComplanitRepository;
            private readonly IGRepository<AttachmentComplanit> _AttachmentComplanitRepository;
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetAllCategoryComplanitQuery> _logger;
            private readonly ResponseDTO _response;
            private readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public GetAllCategoryComplanit(
                IHttpContextAccessor _httpContextAccessor,
                IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<GetAllCategoryComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                IAuditService auditService,
                IGRepository<ComplanitHistory> ComplanitHistoryRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository,
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListComplanit> CheckListComplanitRepository,
                IGRepository<AttachmentComplanit> AttachmentComplanitRepository
            )
            {
                _mapper = mapper;
                _CategoryComplanitRepository = CategoryComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userRepository = userRepository;
                _auditService = auditService;
                _ComplanitHistoryRepository = ComplanitHistoryRepository;
                _CheckListRequestRepository = CheckListRequestRepository;
                _RequestComplanitRepository = RequestComplanitRepository;
                _CheckListComplanitRepository = CheckListComplanitRepository;
                _AttachmentComplanitRepository = AttachmentComplanitRepository;

            }
            public async Task<ResponseDTO> Handle(GetAllComplanitQueryByRegionId request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _userRepository.GetFirst(x => x.Id == _auditService.UserId);
                    if (user == null)
                    {

                        _response.StatusEnum = StatusEnum.Failed;
                        _response.Message = "userNotFound";
                    }
                    var res = await (
                             from req in _RequestComplanitRepository.GetAll(x => x.State == State.NotDeleted)

                             .WhereIf(request.RegionId != null && request.RegionId > 0, x => x.RegionId == request.RegionId)

                             .WhereIf(request.OfficeId != null && request.OfficeId > 0, x => x.OfficeId == request.OfficeId)


                             join check in _CheckListRequestRepository.GetAll(x => x.State == State.NotDeleted)

                               on req.Id equals check.RequestComplanitId

                             join CheckListComp in _CheckListComplanitRepository.GetAll(x => x.State == State.NotDeleted)

                             on check.CheckListComplanitId equals CheckListComp.Id

                             join cat in _CategoryComplanitRepository.GetAll(x => x.State == State.NotDeleted)
                              .WhereIf(request.CategoryId != null && request.CategoryId > 0, x => x.Id == request.CategoryId)

                             on CheckListComp.CategoryComplanitId equals cat.Id

                            

                             join attachment in _AttachmentComplanitRepository.GetAll()

                              on req.Id equals attachment.RequestComplanitId

                             select (new
                             {
                                 CategoryComplanitName = _auditService.UserLanguage == "ar" ? cat.NameAr : cat.NameEn,
                                 Description = req.Description,
                                 CheckListsRequest = _auditService.UserLanguage == "ar" ?
                                 CheckListComp.CheckListRequests.Select(x => x.CheckListComplanit.NameAr).ToArray() :
                                 CheckListComp.CheckListRequests.Select(x => x.CheckListComplanit.NameEn).ToArray(),
                                 ComplanitId = req.Id,
                                 CheckListsRequestIds = CheckListComp.CheckListRequests.Select(x => x.Id).ToArray(),
                                 CategoryComplanitId = CheckListComp.CategoryComplanitId,
                                 AttachmentsComplanit = req.AttachmentsComplanit.Select(x => x.Path).ToArray()
                             })).ToListAsync();


                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, res);

                    _response.setPaginationData(paginatedObjs);
                    _response.Result = paginatedObjs;
                    _response.StatusEnum = StatusEnum.Success;
                    _response.Message = "CategoryComplanitRetrievedSuccessfully";

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
