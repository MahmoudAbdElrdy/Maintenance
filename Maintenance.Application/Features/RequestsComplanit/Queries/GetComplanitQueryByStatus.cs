using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Application.Helpers.QueryableExtensions;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetComplanitQueryByStatus : IRequest<ResponseDTO>
    {
        public GetComplanitQueryByStatus() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public long? CategoryId { get; set; } 
        public long? RegionId { get; set; }  
        public long? OfficeId { get; set; }  
        public ComplanitStatus? ComplanitStatus { get; set; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryComplanit : IRequestHandler<GetComplanitQueryByStatus, ResponseDTO>
        {
          
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository; 
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository; 
           
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetAllCategoryComplanitQuery> _logger;
            private readonly ResponseDTO _response;
            private readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            private readonly IRoom _room;
            public GetAllCategoryComplanit(
            
                ILogger<GetAllCategoryComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                IAuditService auditService,
              
                IGRepository<CheckListRequest> CheckListRequestRepository,
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IRoom room

            )
            {
                _mapper = mapper;
               
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userRepository = userRepository;
                _auditService = auditService;
              
                _CheckListRequestRepository = CheckListRequestRepository;
                _RequestComplanitRepository = RequestComplanitRepository;
                _room = room;
              
            }
            public async Task<ResponseDTO> Handle(GetComplanitQueryByStatus request, CancellationToken cancellationToken)
            {
                try
                {
                    var offices =await _room.GetOffices();

                    var res2 = await _RequestComplanitRepository.GetAll()

                     .Include(x => x.AttachmentsComplanit)
                     .Include(x => x.ComplanitHistory)
                     .Include(x => x.CheckListRequests).
                     ThenInclude(x => x.CheckListComplanit.CategoryComplanit)
                     .Protected(x => x.State == State.NotDeleted)
                     .WhereIf(request.RegionId != null && request.RegionId > 0, x => x.SerialNumber.Substring(3,2).Contains(request.RegionId.ToString()))
                     .WhereIf(request.OfficeId != null && request.OfficeId > 0, x => x.SerialNumber.Substring(0,3).Contains(request.OfficeId.ToString()))
                     .WhereIf(request.CategoryId != null && request.CategoryId > 0, x => x.CheckListRequests.Select(x => x.CheckListComplanit.CategoryComplanitId).Contains(request.CategoryId))
                     .WhereIf(request.ComplanitStatus != null && request.ComplanitStatus > 0, x => x.ComplanitHistory.Select(x => x.ComplanitStatus)
                     .Contains(request.ComplanitStatus))


                         .Select(x => new ComplanitDto
                         {
                             location =x.SerialNumber.Length>0? "مركز : " + offices.Where(y=>y.Code==x.SerialNumber.Substring(0,3)).FirstOrDefault().Name
                             + " منطقة : "+ x.SerialNumber.Substring(3,2)
                             + " بركس : " + x.SerialNumber.Substring(5, 2)
                             + " غرفة : " + x.SerialNumber.Substring(7, 0):"",
                             CategoryComplanitName = _auditService.UserLanguage == "ar" ?
                              x.CheckListRequests.FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.NameAr
                             : x.CheckListRequests.FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.NameEn,
                             Description = x.Description,
                             //CheckListComplanit =_mapper.Map<List<CheckListComplanitDto>>(x.CheckListRequests.Select(x=>x.CheckListComplanit).Where(x=>x.State==State.NotDeleted)),
                             RequestComplanitId = x.Id,
                             //CheckListsRequestIds = x.CheckListRequests.Select(x => x.CheckListComplanit.Id),
                             CategoryComplanitId = x.CheckListRequests.FirstOrDefault(x=>x.State==State.NotDeleted).CheckListComplanit.Id,
                             AttachmentsComplanit = x.AttachmentsComplanit.Where(s => s.State == State.NotDeleted).Select(x => x.Path).ToArray(),
                             ComplanitStatus= x.ComplanitHistory.OrderByDescending(x=>x.CreatedOn).Select(x => x.ComplanitStatus).FirstOrDefault().Value,
                             CheckListComplanit = (List<CheckListComplanitDto>)x.CheckListRequests.
                             Where(x => x.State == State.NotDeleted).
                             Select(s => new CheckListComplanitDto
                             {
                                 CheckListComplanitId = s.CheckListComplanitId,
                                 Name = _auditService.UserLanguage == "ar" ? s.CheckListComplanit.NameAr : s.CheckListComplanit.NameEn,
                                 Description = _auditService.UserLanguage == "ar" ? s.CheckListComplanit.DescriptionAr : s.CheckListComplanit.DescriptionEn,

                             }
                                 )

                         }).ToListAsync()
                     ;

                    //var res = await _CheckListRequestRepository.GetAll()

                    // .Include(x => x.RequestComplanit.AttachmentsComplanit)
                    // .Include(x => x.RequestComplanit.ComplanitHistory)
                    // .Include(x => x.CheckListComplanit.CategoryComplanit)
                    // .WhereIf(request.RegionId != null && request.RegionId > 0, x => x.RequestComplanit.RegionId == request.RegionId)
                    // .WhereIf(request.OfficeId != null && request.OfficeId > 0, x => x.RequestComplanit.OfficeId == request.OfficeId)
                    // .WhereIf(request.CategoryId != null && request.CategoryId > 0, x => x.CheckListComplanit.CategoryComplanitId == request.CategoryId)
                    // .WhereIf(request.ComplanitStatus != null && request.ComplanitStatus > 0, x => x.RequestComplanit.ComplanitHistory.Select(x => x.ComplanitStatus)
                    // .Contains(request.ComplanitStatus))
                    //  .Select(x => new
                    //     {
                    //         CategoryComplanitName = _auditService.UserLanguage == "ar" ? x.CheckListComplanit.CategoryComplanit.NameAr : x.CheckListComplanit.CategoryComplanit.NameEn,
                    //         Description = x.RequestComplanit.Description,
                    //         CheckComplanitName = _auditService.UserLanguage == "ar" ? x.CheckListComplanit.NameAr : x.CheckListComplanit.NameEn,
                    //         RequestComplanitId = x.RequestComplanitId,
                    //         CheckComplanitId = x.CheckListComplanit.Id, 
                    //         CategoryComplanitId = x.CheckListComplanit.CategoryComplanitId,
                    //         AttachmentsComplanit = x.RequestComplanit.AttachmentsComplanit.Select(x => x.Path).ToArray()
                    //     }).ToListAsync();
                     
                    //var res1 = res.DistinctBy(x => x.RequestComplanitId).ToList();

                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, res2);

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
