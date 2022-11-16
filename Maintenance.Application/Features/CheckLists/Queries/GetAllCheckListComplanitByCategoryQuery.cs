using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.CheckLists.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Domain.Entities.Complanits;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.CheckLists.Queries
{
    public class GetAllCheckListComplanitByCategoryQuery : IRequest<ResponseDTO>
    {
        public GetAllCheckListComplanitByCategoryQuery() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public long? CategoryComplanitId { set; get; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCheckListComplanit : IRequestHandler<GetAllCheckListComplanitByCategoryQuery, ResponseDTO>
        {
            private readonly IGRepository<CheckListComplanit> _CheckListComplanitRepository;
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetAllCheckListComplanitQuery> _logger;
            private readonly ResponseDTO _response;
            public long _loggedInUserId;
            private readonly IMapper _mapper;
            public GetAllCheckListComplanit(
                IHttpContextAccessor _httpContextAccessor,
                IGRepository<CheckListComplanit> CheckListComplanitRepository,
                ILogger<GetAllCheckListComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository
            )
            {
                _mapper = mapper;
                _CheckListComplanitRepository = CheckListComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userRepository = userRepository;
                try
                {
                    _loggedInUserId = long.Parse(_httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == "userLoginId").SingleOrDefault().Value);
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException();
                }

            }
            public async Task<ResponseDTO> Handle(GetAllCheckListComplanitByCategoryQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _userRepository.GetFirst(x => x.Id == _loggedInUserId);
                    if (user == null)
                    {

                        _response.StatusEnum = StatusEnum.Failed;
                        _response.Message = "userNotFound";
                    }

                    var entityJobs = _CheckListComplanitRepository.GetAll(x => x.State != Domain.Enums.State.Deleted&&x.CategoryComplanitId==request.CategoryComplanitId).ToList();

                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, _mapper.Map<List<CheckListComplanitDto>>(entityJobs));

                    _response.setPaginationData(paginatedObjs);
                    _response.Result = paginatedObjs;
                    _response.StatusEnum = StatusEnum.Success;
                    _response.Message = "CheckListComplanitRetrievedSuccessfully";

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
