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

namespace Maintenance.Application.Features.Categories.Queries
{
    public class ReportQueryByDate : IRequest<ResponseDTO>
    {
      public DateTime? FormDate { get; set; }
      public DateTime? ToDate { get; set; } 
        class GetAllReportQuery : IRequestHandler<ReportQueryByDate, ResponseDTO>
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
          
            private readonly IRoom _room;
            public GetAllReportQuery(
             IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<GetAllCategoryComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                IAuditService auditService,
               IGRepository<ComplanitHistory> ComplanitHistoryRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository,
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListComplanit> CheckListComplanitRepository,
                IRoom room

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


            }
            public async Task<ResponseDTO> Handle(ReportQueryByDate request, CancellationToken cancellationToken)
            {
                try
                {

                    var res2 = await _RequestComplanitRepository.GetAll(x => x.State == State.NotDeleted)
                       .Include(c=>c.CheckListRequests).
                        ThenInclude(c=>c.CheckListComplanit.CategoryComplanit)
                       .ToListAsync();
                    var checklist = res2.SelectMany(c => c.CheckListRequests).ToList();
                    var checklist2 = await _CheckListRequestRepository.GetAll(x => x.State == State.NotDeleted).ToListAsync();

                    var itemCategories2 = await _CategoryComplanitRepository.GetAll(c => c.State == State.NotDeleted).ToListAsync();
                    var itemCategories = res2.SelectMany(c => c.CheckListRequests).Select(c=>c.CheckListComplanit.CategoryComplanit).DistinctBy(c=>c.Id).ToList();
                    var item = new
                    {
                        AllComplanit=res2.Count(),
                        SubmittedComplanit = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.Submitted),
                        SuspendedComplanit = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianSuspended),
                        ClosedComplanit = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianClosed),
                        DoneComplanit = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianDone),
                        Electricity = checklist.Where(c=>c.CheckListComplanit.CategoryComplanit!=null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c=>c.NameEn== "Electricity").FirstOrDefault().Id),
                        Cleanliness = checklist.Where(c => c.CheckListComplanit.CategoryComplanit != null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c => c.NameEn == "cleanliness").FirstOrDefault().Id),
                        Plumbing = checklist.Where(c => c.CheckListComplanit.CategoryComplanit != null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c => c.NameEn == "Plumbing").FirstOrDefault().Id),
                        AirConditioner = checklist.Where(c => c.CheckListComplanit.CategoryComplanit != null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c => c.NameEn == "Air conditioner").FirstOrDefault().Id)
                    };

                  
                 
                    _response.Result = item;
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
