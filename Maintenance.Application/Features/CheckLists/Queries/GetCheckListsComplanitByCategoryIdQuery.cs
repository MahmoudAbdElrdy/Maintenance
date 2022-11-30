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
    public class GetCheckListsComplanitByCategoryIdQuery : IRequest<ResponseDTO>
    {
        public long? CategoryComplanitId { set; get; }
        class GetAllCheckListComplanit : IRequestHandler<GetCheckListsComplanitByCategoryIdQuery, ResponseDTO>
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
                //try
                //{
                //    _loggedInUserId = long.Parse(_httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == "userLoginId").SingleOrDefault().Value);
                //}
                //catch (Exception ex)
                //{
                //    throw new UnauthorizedAccessException();
                //}

            }
            public async Task<ResponseDTO> Handle(GetCheckListsComplanitByCategoryIdQuery request, CancellationToken cancellationToken)
            {
                try
                {

                    var entityJobs = _CheckListComplanitRepository.GetAll(x => x.State != Domain.Enums.State.Deleted
                    &&x.CategoryComplanitId==request.CategoryComplanitId).Select(x=>new CheckListComplanitDto()
                    {
                        CategoryComplanitId = x.CategoryComplanitId,
                        NameEn = x.NameEn,
                        NameAr = x.NameAr,
                        Id = x.Id ,

                    }).ToList();

                    _response.Result = entityJobs;
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
