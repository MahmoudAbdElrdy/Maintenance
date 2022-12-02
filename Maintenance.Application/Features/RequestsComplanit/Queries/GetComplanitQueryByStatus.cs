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
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Xml.Linq;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetComplanitQueryByStatus : IRequest<ResponseDTO>
    {
        public GetComplanitQueryByStatus() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }

        public List<long>? CategoryId { get; set; } 
        public List<long>? RegionId { get; set; }
        public List<long>? OfficeId { get; set; }  
        
        public ComplanitStatus? ComplanitStatus { get; set; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryComplanit : IRequestHandler<GetComplanitQueryByStatus, ResponseDTO>
        {
            private readonly IGRepository<CategoryComplanit> _CategoryComplanitRepository;
            private readonly IGRepository<ComplanitHistory> _ComplanitHistoryRepository;
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository;
            private readonly IGRepository<CheckListComplanit> _CheckListComplanitRepository;
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetAllCategoryComplanitQuery> _logger;
            private readonly ResponseDTO _response;
            private readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            private readonly IGRepository<ComplanitFilter> _ComplanitFilterRepository;
            private readonly IRoom _room;
            public GetAllCategoryComplanit(
             IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<GetAllCategoryComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                IAuditService auditService,
               IGRepository<ComplanitHistory> ComplanitHistoryRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository,
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListComplanit> CheckListComplanitRepository,
                IRoom room,
                IGRepository<ComplanitFilter> ComplanitFilterRepository

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
                _room = room;
                _CheckListComplanitRepository = CheckListComplanitRepository;
                _ComplanitFilterRepository = ComplanitFilterRepository;

            }
            public async Task<ResponseDTO> Handle(GetComplanitQueryByStatus request, CancellationToken cancellationToken)
            {
                try
                {
                    var offices =await _room.GetOffices();
                    List<long>? CheckListComplanitIds = new List<long>();
                    if (request.CategoryId != null && request.CategoryId.Count > 0)
                    {
                        CheckListComplanitIds = _CheckListComplanitRepository.GetAll(x =>x.State==State.NotDeleted&& request.CategoryId.Contains((long)x.CategoryComplanitId)).Select(s => (long)s.CategoryComplanitId).ToList();
                    //   var CheckListComplanit= _CheckListComplanitRepository.GetAll(x =>x.State==State.NotDeleted&& request.CategoryId.Contains((long)x.CategoryComplanitId)).Select(s => s.CategoryComplanit).ToList();

                    }

                    var res2 = await _RequestComplanitRepository.GetAll()
                       
                     .Include(x => x.Creator)
                     .Include(x => x.AttachmentsComplanit)
                     .Include(x => x.ComplanitHistory)
                     .Include(x => x.CheckListRequests).
                      ThenInclude(x => x.CheckListComplanit.CategoryComplanit)
                     .Protected(x => x.State == State.NotDeleted)
                     .WhereIf(request.RegionId != null && request.RegionId.Count > 0, x => request.RegionId.Contains((long)x.RegionId))
                     .WhereIf(request.OfficeId != null && request.OfficeId.Count > 0, x => request.OfficeId.Contains((long)x.OfficeId))
                     .WhereIf(request.CategoryId != null && request.CategoryId.Count > 0, x => x.CheckListRequests.Select(c => c.CheckListComplanit).Any(c => CheckListComplanitIds.Contains((long)c.CategoryComplanitId)))
                     .WhereIf(request.ComplanitStatus != null && request.ComplanitStatus > 0,c=>c.ComplanitStatus==request.ComplanitStatus)
                      .OrderByDescending(c => c.CreatedOn)
                      .Select(x => new ComplanitDto
                         {
                             Code = x.Code,
                             SerialNumber = x.SerialNumber,
                         
                             CategoryComplanitLogo = x.CheckListRequests.FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.Logo,


                             CategoryComplanitName = _auditService.UserLanguage == "ar" ?
                              x.CheckListRequests.Where(s => s.RequestComplanit.State == State.NotDeleted && s.RequestComplanitId == x.Id).FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.NameAr
                             : x.CheckListRequests.Where(s => s.RequestComplanit.State == State.NotDeleted && s.RequestComplanitId == x.Id).FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.NameEn,
                             Description = x.Description,
                             //CheckListComplanit =_mapper.Map<List<CheckListComplanitDto>>(x.CheckListRequests.Select(x=>x.CheckListComplanit).Where(x=>x.State==State.NotDeleted)),
                             RequestComplanitId = x.Id,
                             //CheckListsRequestIds = x.CheckListRequests.Select(x => x.CheckListComplanit.Id),
                             CategoryComplanitId = x.CheckListRequests.Where(s => s.RequestComplanit.State == State.NotDeleted && s.RequestComplanitId == x.Id).FirstOrDefault(y => y.State == State.NotDeleted && y.RequestComplanitId == x.Id).CheckListComplanit.CategoryComplanitId,
                             AttachmentsComplanit = x.AttachmentsComplanit.Where(s => s.State == State.NotDeleted).Select(x => x.Path).ToArray(),
                             // ComplanitStatus=(int) x.ComplanitHistory.OrderByDescending(x=>x.CreatedOn).Select(x => x.ComplanitStatus).FirstOrDefault(),
                             ComplanitStatus = (int)x.ComplanitStatus,
                             CreatedOn=x.CreatedOn,
                             CheckListComplanit = (List<CheckListComplanitDto>)x.CheckListRequests.
                             Where(s => s.State == State.NotDeleted).
                             Select(s => new CheckListComplanitDto
                             {
                                 CheckListComplanitId = s.CheckListComplanitId,
                                 Name = _auditService.UserLanguage == "ar" ? s.CheckListComplanit.NameAr : s.CheckListComplanit.NameEn,
                                 Description = _auditService.UserLanguage == "ar" ? s.CheckListComplanit.DescriptionAr : s.CheckListComplanit.DescriptionEn,

                             }
                                 ),
                             UserDto=new UserDto()
                             {
                                 FullName = x.Creator!=null? x.Creator.FullName:"",
                                 IdentityNumber = x.Creator!=null? x.Creator.IdentityNumber : "",
                                 PhoneNumber = x.Creator!=null? x.Creator.PhoneNumber : "",
                                 UserId =x.Creator!=null? x.Creator.Id : null,
                             }

                         }).ToListAsync();

                    foreach (var item in res2)
                    {
                        var officeName = offices.Where(y => y.Code == item.SerialNumber.Substring(0, 3)).FirstOrDefault();
                        if (officeName != null)
                        {
                            var name = _auditService.UserLanguage == "ar" ? officeName.NameAr : officeName.NameEn;
                          
                            if(item.SerialNumber.Length > 0)
                            {
                                item.OfficeName = name;
                                item.RegionName = item.SerialNumber.Substring(3, 2);
                                item.CarvanNumber = item.SerialNumber.Substring(5, 2);
                                item.RoomNumber = item.SerialNumber.Substring(7, 1);
                            }
                     
                        }
                        else
                        {
                            if (item.SerialNumber.Length > 0)
                            {
                                item.RegionName = item.SerialNumber.Substring(3, 2);
                                item.CarvanNumber = item.SerialNumber.Substring(5, 2);
                                item.RoomNumber = item.SerialNumber.Substring(7, 1);
                            }


                        }

                    }
                    if (_auditService.UserType == UserType.Technician.ToString()){
                        if (!(request.OfficeId.Count() == 0 && request.CategoryId.Count == 0 && request.RegionId.Count == 0))
                        {
                            var ComplanitFilterExit = await _ComplanitFilterRepository.GetFirstAsync(c => c.CreatedBy == _auditService.UserId);

                            if (ComplanitFilterExit == null)
                            {
                                ComplanitFilterExit = new ComplanitFilter()
                                {
                                    CreatedBy = _auditService.UserId,
                                    CreatedOn = DateTime.Now,
                                    State = Domain.Enums.State.NotDeleted,
                                    OfficeId = request.OfficeId.Count > 0 ? string.Join(",", request.OfficeId) : "",
                                    RegionId = request.RegionId.Count > 0 ? string.Join(",", request.RegionId) : "",
                                    CategoryComplanitId = request.CategoryId.Count > 0 ? string.Join(",", request.CategoryId) : "",

                                };
                                await _ComplanitFilterRepository.AddAsync(ComplanitFilterExit);
                            }
                            else
                            {
                                ComplanitFilterExit.UpdatedOn = DateTime.Now;
                                ComplanitFilterExit.OfficeId = request.OfficeId.Count>0? string.Join(",", request.OfficeId):"";
                                ComplanitFilterExit.RegionId = request.RegionId.Count > 0 ? string.Join(",", request.RegionId):"";
                                ComplanitFilterExit.CategoryComplanitId = request.CategoryId.Count>0? string.Join(",", request.CategoryId):"";

                                _ComplanitFilterRepository.Update(ComplanitFilterExit);
                            }




                            _ComplanitFilterRepository.Save();
                        }

                    }



                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, res2.ToList());

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
