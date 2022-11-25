using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Application.Helpers.QueryableExtensions;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetComplanitDetailsQuery : IRequest<ResponseDTO>
    {
        public GetComplanitDetailsQuery() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
       
        public long? RequestComplanitId { set; get; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryComplanit : IRequestHandler<GetComplanitDetailsQuery, ResponseDTO>
        {
          
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository; 
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository; 
           
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetAllCategoryComplanitQuery> _logger;
            private readonly ResponseDTO _response;
            private readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public GetAllCategoryComplanit(
            
                ILogger<GetAllCategoryComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                IAuditService auditService,
              
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository

            )
            {
                _mapper = mapper;
               
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userRepository = userRepository;
                _auditService = auditService;
              
                _RequestComplanitRepository = RequestComplanitRepository;
                _CheckListRequestRepository = CheckListRequestRepository;


            }
            public async Task<ResponseDTO> Handle(GetComplanitDetailsQuery request, CancellationToken cancellationToken)
            {
                try
                {

                    //fileMovementRepository.GetAll()
                    //.Where(fm => repository.GetAll().Select(f => f.Id).Contains(fm.FileId) && fm.TransferredById == userId)
                    //.Include(f => f.User).Include(f => f.File).ThenInclude(f => f.Category)
                    //.OrderByDescending(f => f.MovedOn)
                    //.ToList()
                    //.GroupBy(f => f.FileId)
                    //.Select(f => f.First())
                    //.ToList();

                    var resx = _CheckListRequestRepository.
                        GetAllIncluding(
                        c => c.CheckListComplanit.CategoryComplanit,
                        c=>c.RequestComplanit.AttachmentsComplanit).
                        Include(c => c.RequestComplanit.ComplanitHistory)
                        .ThenInclude(c=>c.AttachmentComplanitHistory)
                        .Where(c => c.RequestComplanitId == request.RequestComplanitId)
                        .Select(c => new {
                            CheckListComplaniNameAr= c.CheckListComplanit.NameAr,
                            CheckListComplanitNameEn = c.CheckListComplanit.NameEn,
                            CategoryComplanitNameAr = c.CheckListComplanit.CategoryComplanit.NameAr,
                            CategoryComplanitNameEn = c.CheckListComplanit.CategoryComplanit.NameEn,
                            CheckListComplanitId = c.CheckListComplanit.Id,
                            CategoryComplanitId = c.CheckListComplanit.CategoryComplanitId,
                            RequestComplanitId=c.RequestComplanitId,
                            Description = c.RequestComplanit.Description,
                            AttachmentsComplanit = c.RequestComplanit.AttachmentsComplanit.Select(x => x.Path).ToArray(),
                            AttachmentComplanitHistory = c.RequestComplanit.ComplanitHistory.SelectMany(x => x.AttachmentComplanitHistory).Select(x => x.Path).ToArray(),
                            ComplanitHistory = c.RequestComplanit.ComplanitHistory.Select(x => x.ComplanitStatus)
                        }).ToList();

                    //var res1 = res.DistinctBy(x => x.RequestComplanitId).ToList();
                  //  var resx1= resx.DistinctBy(x => x.RequestComplanitId).ToList();

                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, resx);

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
