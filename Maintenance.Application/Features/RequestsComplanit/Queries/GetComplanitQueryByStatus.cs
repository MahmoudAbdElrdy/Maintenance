﻿using AuthDomain.Entities.Auth;
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
using Maintenance.Infrastructure.Migrations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetComplanitQueryByStatus : IRequest<ResponseDTO>
    {
        public GetComplanitQueryByStatus() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public List<long>? CategoryId { get; set; } 
        public List<string>? RegionId { get; set; }
        public List<string>? OfficeId { get; set; }  
        public ComplanitStatus? ComplanitStatus { get; set; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryComplanit : IRequestHandler<GetComplanitQueryByStatus, ResponseDTO>
        {
          
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository; 
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository; 
            private readonly IGRepository<CheckListComplanit> _CheckListComplanitRepository; 
           
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
                IGRepository<CheckListComplanit> CheckListComplanitRepository,
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
                _CheckListComplanitRepository = CheckListComplanitRepository;


            }
            public async Task<ResponseDTO> Handle(GetComplanitQueryByStatus request, CancellationToken cancellationToken)
            {
                try
                {
                    var offices =await _room.GetOffices();
                    List<long> CheckListComplanitIds = new List<long>();
                    if (request.CategoryId != null && request.CategoryId.Count > 0)
                    {
                        CheckListComplanitIds = _CheckListComplanitRepository.GetAll(x => request.CategoryId.Contains((long)x.CategoryComplanitId)).Select(s => s.Id).ToList();

                    }

                    var res2 = await _RequestComplanitRepository.GetAll()

                     .Include(x => x.AttachmentsComplanit)
                     .Include(x => x.ComplanitHistory)
                     .Include(x => x.CheckListRequests).
                     ThenInclude(x => x.CheckListComplanit.CategoryComplanit)
                     .Protected(x => x.State == State.NotDeleted)
                     .WhereIf(request.RegionId != null && request.RegionId.Count>0, x => request.RegionId.Contains(x.SerialNumber.Substring(3,2)))
                     .WhereIf(request.OfficeId != null && request.OfficeId.Count > 0, x => request.OfficeId.Contains(x.SerialNumber.Substring(0, 3)))
                     .WhereIf(request.CategoryId != null && request.CategoryId.Count > 0, x => x.CheckListRequests.Any(c=> CheckListComplanitIds.Contains((long)c.CheckListComplanitId)))
                     .WhereIf(request.ComplanitStatus != null && request.ComplanitStatus > 0, x => x.ComplanitHistory.Select(x => x.ComplanitStatus)
                     .Contains(request.ComplanitStatus))


                         .Select(x => new ComplanitDto
                         {
                             SerialNumber=x.SerialNumber,
                             location = x.SerialNumber.Length > 0 ? "مركز : " //+ offices.Where(y => y.Code== x.SerialNumber.Substring(0, 3)).FirstOrDefault().Name
                             + " منطقة : " + x.SerialNumber.Substring(3, 2)
                             + " بركس : " + x.SerialNumber.Substring(5, 2)
                             + " غرفة : " + x.SerialNumber.Substring(7, 0) : "",
                             CategoryComplanitLogo = x.CheckListRequests.FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.Logo,


                             CategoryComplanitName = _auditService.UserLanguage == "ar" ?
                              x.CheckListRequests.FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.NameAr
                             : x.CheckListRequests.FirstOrDefault(x => x.State == State.NotDeleted).CheckListComplanit.CategoryComplanit.NameEn,
                             Description = x.Description,
                             //CheckListComplanit =_mapper.Map<List<CheckListComplanitDto>>(x.CheckListRequests.Select(x=>x.CheckListComplanit).Where(x=>x.State==State.NotDeleted)),
                             RequestComplanitId = x.Id,
                             //CheckListsRequestIds = x.CheckListRequests.Select(x => x.CheckListComplanit.Id),
                             CategoryComplanitId = x.CheckListRequests.FirstOrDefault(x=>x.State==State.NotDeleted).CheckListComplanit.Id,
                             AttachmentsComplanit = x.AttachmentsComplanit.Where(s => s.State == State.NotDeleted).Select(x => x.Path).ToArray(),
                             ComplanitStatus=(int) x.ComplanitHistory.OrderByDescending(x=>x.CreatedOn).Select(x => x.ComplanitStatus).FirstOrDefault(),
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
