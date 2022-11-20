using AuthDomain.Entities.Auth;
using AutoMapper;
using MailKit.Search;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Application.Helpers.QueryableExtensions;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using Maintenance.Infrastructure.Migrations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using System.Linq;
using System.Linq.Dynamic.Core;
using AttachmentComplanit = Maintenance.Domain.Entities.Complanits.AttachmentComplanit;
using ComplanitHistory = Maintenance.Domain.Entities.Complanits.ComplanitHistory;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetComplanitHistoryQueryByStatus : IRequest<ResponseDTO>
    {
        public GetComplanitHistoryQueryByStatus() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public long? CategoryId { get; set; } 
        public long? RegionId { get; set; }  
        public long? OfficeId { get; set; }  
        public ComplanitStatus? ComplanitStatus { get; set; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryComplanit : IRequestHandler<GetComplanitHistoryQueryByStatus, ResponseDTO>
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
            public async Task<ResponseDTO> Handle(GetComplanitHistoryQueryByStatus request, CancellationToken cancellationToken)
            {
                try
                {


                    //var res = await _RequestComplanitRepository.GetAll() 

                    // .Include(x => x.AttachmentsComplanit)
                    // .Include(x => x.ComplanitHistory)
                    // .Include(x => x.CheckListRequests).
                    // ThenInclude(x => x.CheckListComplanit.CategoryComplanit)
                    // .Protected(x=>x.State==State.NotDeleted)
                    // .WhereIf(request.RegionId != null && request.RegionId > 0, x => x.RegionId == request.RegionId)
                    // .WhereIf(request.OfficeId != null && request.OfficeId > 0, x => x.OfficeId == request.OfficeId)
                    // .WhereIf(request.CategoryId != null && request.CategoryId > 0, x => x.CheckListRequests.Select(x => x.CheckListComplanit.CategoryComplanitId).Contains(request.CategoryId))
                    // .WhereIf(request.ComplanitStatus != null && request.ComplanitStatus > 0, x => x.ComplanitHistory.Select(x => x.ComplanitStatus)
                    // .Contains(request.ComplanitStatus))


                    //     .Select(x => new ComplanitDto
                    //     {

                    //         CategoryComplanitName = _auditService.UserLanguage == "ar" ?
                    //          x.CheckListRequests.FirstOrDefault().CheckListComplanit.NameAr
                    //         : x.CheckListRequests.FirstOrDefault().CheckListComplanit.NameEn,
                    //         Description = x.Description,
                    //         //CheckListComplanit =_mapper.Map<List<CheckListComplanitDto>>(x.CheckListRequests.Select(x=>x.CheckListComplanit).Where(x=>x.State==State.NotDeleted)),
                    //         RequestComplanitId = x.Id,
                    //         //CheckListsRequestIds = x.CheckListRequests.Select(x => x.CheckListComplanit.Id),
                    //         CategoryComplanitId = x.CheckListRequests.FirstOrDefault().CheckListComplanit.Id,
                    //         AttachmentsComplanit = x.AttachmentsComplanit.Where(s=>s.State==State.NotDeleted).Select(x => x.Path).ToArray(),
                    //         CheckListComplanit= (List<CheckListComplanitDto>)x.CheckListRequests.
                    //         Where(x => x.State == State.NotDeleted).
                    //         Select(s=>new CheckListComplanitDto
                    //         {
                    //             CheckListComplanitId=s.CheckListComplanitId,
                    //             Name= _auditService.UserLanguage == "ar"?s.CheckListComplanit.NameAr:s.CheckListComplanit.NameEn,
                    //             Description= _auditService.UserLanguage == "ar"?s.CheckListComplanit.DescriptionAr:s.CheckListComplanit.DescriptionEn,

                    //         }
                    //             )

                    //     }).ToListAsync()
                    // ;
                    var res = await _CheckListRequestRepository.GetAll()

                     .Include(x => x.RequestComplanit.AttachmentsComplanit)
                     .Include(x => x.RequestComplanit.ComplanitHistory)
                     .Include(x => x.CheckListComplanit.CategoryComplanit)
                     .WhereIf(request.RegionId != null && request.RegionId > 0, x => x.RequestComplanit.RegionId == request.RegionId)
                     .WhereIf(request.OfficeId != null && request.OfficeId > 0, x => x.RequestComplanit.OfficeId == request.OfficeId)
                     .WhereIf(request.CategoryId != null && request.CategoryId > 0, x => x.CheckListComplanit.CategoryComplanitId == request.CategoryId)
                     .WhereIf(request.ComplanitStatus != null && request.ComplanitStatus > 0, x => x.RequestComplanit.ComplanitHistory.Select(x => x.ComplanitStatus)
                     .Contains(request.ComplanitStatus))


                         .Select(x => new
                         {
                             CategoryComplanitName = _auditService.UserLanguage == "ar" ?
                              x.CheckListComplanit.CategoryComplanit.NameAr
                             : x.CheckListComplanit.CategoryComplanit.NameEn,
                             Description = x.RequestComplanit.Description,
                             CheckListsRequest = _auditService.UserLanguage == "ar" ?
                             x.CheckListComplanit.NameAr :
                             x.CheckListComplanit.NameEn,
                             ComplanitId = x.RequestComplanitId,
                             CheckListsRequestIds = x.CheckListComplanit.Id,
                             CategoryComplanitId = x.CheckListComplanit.CategoryComplanitId,
                             AttachmentsComplanit = x.RequestComplanit.AttachmentsComplanit.Select(x => x.Path).ToArray()
                         }).ToListAsync()
                     ;
                    var res1 = res.DistinctBy(x => x.ComplanitId).ToList();

                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, res1);

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
