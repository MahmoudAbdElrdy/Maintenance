using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Auth.UpdateToken.Command;
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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
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
           
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
           
            private readonly ILogger<GetAllCategoryComplanitQuery> _logger;
            private readonly ResponseDTO _response;
            private readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            private readonly IStringLocalizer<ReportQueryByDate> _localizationProvider;
       
            public GetAllReportQuery(
             IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<GetAllCategoryComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                IAuditService auditService,
               IGRepository<ComplanitHistory> ComplanitHistoryRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository,
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListComplanit> CheckListComplanitRepository,
               
                IStringLocalizer<ReportQueryByDate> localizationProvider

            )
            {
                _mapper = mapper;
                _CategoryComplanitRepository = CategoryComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
              
                _auditService = auditService;
               
                _RequestComplanitRepository = RequestComplanitRepository;
               
              
                _localizationProvider = localizationProvider;

            }
            public async Task<ResponseDTO> Handle(ReportQueryByDate request, CancellationToken cancellationToken)
            {
                try
                {

                    var res2 = await _RequestComplanitRepository.
                       
                        GetAll(x => x.State == State.NotDeleted )
                      
                        .WhereIf(request.FormDate!=null&&request.FormDate!=DateTime.MinValue,c=>c.CreatedOn.Date >= request.FormDate.Value.Date)
                        
                        .WhereIf(request.ToDate!=null&&request.ToDate!=DateTime.MinValue,c=>c.CreatedOn.Date <= request.ToDate.Value.Date)
                      
                        .Include(c=>c.CheckListRequests).
                       
                        ThenInclude(c=>c.CheckListComplanit.CategoryComplanit)
                      
                        .ToListAsync();


                    var checklist = res2.SelectMany(c => c.CheckListRequests).ToList();
                  
                    var itemCat = await _CategoryComplanitRepository.GetAll(x => x.State == State.NotDeleted).ToListAsync();
                   
                    
                    
                    var listNumber = new List<ReportDto>();

                    listNumber.Add(new ReportDto()
                    {
                        NameProperty = _localizationProvider["AllComplanit"] ,
                        ValueProperty = res2.Count()
                    });

                    var distinct = Enum.GetNames(typeof(ComplanitStatus)).ToList();

                    foreach (var check in distinct)
                    {
                        var Property = new ReportDto();
                      
                        var number = res2.Count(x => x.ComplanitStatus.ToString() == check);
                       
                        Property.ValueProperty = number;
                      
                        Property.NameProperty = _localizationProvider[check.ToString()] ;

                        listNumber.Add(Property);

                    }

                    foreach (var check in itemCat)
                    {
                        var Property = new ReportDto();
                       
                        var number = checklist.DistinctBy(c => c.RequestComplanitId).Count(c => c.CheckListComplanit.CategoryComplanit.NameEn == check.NameEn);
                      
                        Property.ValueProperty = number;
                       
                        Property.NameProperty = _auditService.UserLanguage=="ar"? check.NameAr: check.NameEn;

                        listNumber.Add(Property);

                    }
                  
                   

       
                    //var item = new
                    //{
                    //    AllComplanit=res2.Count(),
                    //    TechnicianCanceled = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianCanceled),
                    //    Submitted = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.Submitted),
                    //    TechnicianSuspended = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianSuspended),
                    //    TechnicianClosed = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianClosed),
                    //    TechnicianDone = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianDone),
                    //    TechnicianAssigned = res2.Count(x=>x.ComplanitStatus==ComplanitStatus.TechnicianAssigned), 

                    //    Electricity = checklist.DistinctBy(c=>c.RequestComplanitId).Count(c => c.CheckListComplanit.CategoryComplanit.NameEn == "Electricity"),
                    //    Cleanliness = checklist.DistinctBy(c => c.RequestComplanitId).Count(c => c.CheckListComplanit.CategoryComplanit.NameEn == ("cleanliness")),
                    //    Plumbing = checklist.DistinctBy(c => c.RequestComplanitId).Count(c => c.CheckListComplanit.CategoryComplanit.NameEn == ("Plumbing")),
                    //    AirConditioner = checklist.DistinctBy(c => c.RequestComplanitId).Count(c => c.CheckListComplanit.CategoryComplanit.NameEn == ("Air conditioner")),
                    //    //Electricity = checklist.Where(c => itemCategories.Where(c => c.NameEn == "Electricity").FirstOrDefault() != null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c=>c.NameEn== "Electricity").FirstOrDefault().Id),
                    //    //Cleanliness = checklist.Where(c => itemCategories.Where(c => c.NameEn == "cleanliness").FirstOrDefault() != null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c => c.NameEn == "cleanliness").FirstOrDefault().Id),
                    //    //Plumbing = checklist.Where(c=>itemCategories.Where(c => c.NameEn == "Plumbing").FirstOrDefault() != null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c => c.NameEn == "Plumbing").FirstOrDefault().Id),
                    //    //AirConditioner = checklist.Where(c => itemCategories.Where(c => c.NameEn == "Air conditioner").FirstOrDefault() != null).Count(c=>c.CheckListComplanit.CategoryComplanitId== itemCategories.Where(c => c.NameEn == "Air conditioner").FirstOrDefault().Id)
                    //};

                  
                 
                    _response.Result = listNumber;
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
   public class ReportDto
    {
        public string NameProperty { get; set; }
        public long ValueProperty { get; set; } 
    }
}
