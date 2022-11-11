using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Domain.Entities.Reports;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetAllCategoryReportQuery : IRequest<ResponseDTO>
    {
        public GetAllCategoryReportQuery() 
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllCategoryReport : IRequestHandler<GetAllCategoryReportQuery, ResponseDTO>
        {
            private readonly IGRepository<CategoryReport> _CategoryReportRepository;
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetAllCategoryReportQuery> _logger;
            private readonly ResponseDTO _response;
            public long _loggedInUserId;
            private readonly IMapper _mapper;
            public GetAllCategoryReport(
                IHttpContextAccessor _httpContextAccessor,
                IGRepository<CategoryReport> CategoryReportRepository,
                ILogger<GetAllCategoryReportQuery> logger, IMapper mapper,
                IGRepository<User> userRepository
            )
            {
                _mapper = mapper;
                _CategoryReportRepository = CategoryReportRepository;
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
            public async Task<ResponseDTO> Handle(GetAllCategoryReportQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _userRepository.GetFirst(x => x.Id == _loggedInUserId);
                    if (user == null)
                    {

                        _response.StatusEnum = StatusEnum.Failed;
                        _response.Message = "userNotFound";
                    }

                    var entityJobs = _CategoryReportRepository.GetAll(x => x.State != Domain.Enums.State.Deleted).ToList();

                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, _mapper.Map<List<CategoryReportDto>>(entityJobs));

                    _response.setPaginationData(paginatedObjs);
                    _response.Result = paginatedObjs;
                    _response.StatusEnum = StatusEnum.Success;
                    _response.Message = "CategoryReportRetrievedSuccessfully";

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
