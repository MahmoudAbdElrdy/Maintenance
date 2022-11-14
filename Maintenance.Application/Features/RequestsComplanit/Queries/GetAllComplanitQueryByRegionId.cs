using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Application.Helpers.QueryableExtensions;
using Maintenance.Domain.Entities.Complanits;
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
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryComplanit : IRequestHandler<GetAllComplanitQueryByRegionId, ResponseDTO>
        {
            private readonly IGRepository<CategoryComplanit> _CategoryComplanitRepository;
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository; 
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository; 
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
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository
            )
            {
                _mapper = mapper;
                _CategoryComplanitRepository = CategoryComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userRepository = userRepository;
                _auditService = auditService;
                _RequestComplanitRepository = RequestComplanitRepository;
                _CheckListRequestRepository = CheckListRequestRepository;

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
                    var _RequestComplanit = await _CheckListRequestRepository.GetAll().
                     
                        Include(c=>c.RequestComplanit).ThenInclude(c=>c.CheckListRequests)
                     
                        .WhereIf(request.RegionId != null && request.RegionId>0, x => x.RequestComplanit.RegionId == request.RegionId)
                        
                        .WhereIf(request.CategoryId !=null && request.CategoryId > 0, x=>x.CheckListComplanit.CategoryComplanitId==request.CategoryId)
                    
                        .Select(x => new ComplanitDto() 
                       { 
                           CategoryComplanitName=_auditService.UserLanguage== "ar" ? 
                           x.CheckListComplanit.CategoryComplanit.NameAr
                           : x.CheckListComplanit.CategoryComplanit.NameEn,
                           Description=x.RequestComplanit.Description,
                           CheckListsRequest= _auditService.UserLanguage == "ar" && x.CheckListComplanit.CheckListRequests.Count()>0 ? 
                           x.CheckListComplanit.CheckListRequests.Select(x => x.CheckListComplanit.NameAr).ToArray() :
                           x.CheckListComplanit.CheckListRequests.Select(x => x.CheckListComplanit.NameEn).ToArray()


                       })
                       .ToListAsync();

                    var entityJobs = _CategoryComplanitRepository.GetAll(x => x.State != Domain.Enums.State.Deleted).ToList();

                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, _RequestComplanit);

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
