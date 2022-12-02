using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Auth.VerificationCode.Command;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Queries
{
    public class GetAllCategoriesComplanitsQuery : IRequest<ResponseDTO>
    {
        class GetAllCategoryComplanit : IRequestHandler<GetAllCategoriesComplanitsQuery, ResponseDTO>
        {
            private readonly IGRepository<CategoryComplanit> _CategoryComplanitRepository;
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetAllCategoryComplanitQuery> _logger;
            private readonly ResponseDTO _response;
            public long _loggedInUserId;
            private readonly IMapper _mapper;
            private readonly IAuditService _auditService;
            private readonly IStringLocalizer<GetAllCategoryComplanit> _localizationProvider;
            public GetAllCategoryComplanit(
                IHttpContextAccessor _httpContextAccessor,
                IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<GetAllCategoryComplanitQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                 IAuditService auditService,
                IStringLocalizer<GetAllCategoryComplanit> localizationProvider

            )
            {
                _mapper = mapper;
                _CategoryComplanitRepository = CategoryComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userRepository = userRepository;
                
                _auditService = auditService;
                _localizationProvider = localizationProvider;
            }
            public async Task<ResponseDTO> Handle(GetAllCategoriesComplanitsQuery request, CancellationToken cancellationToken)
            {
                try
                {

                    var entityJobs = _CategoryComplanitRepository.GetAll(x => x.State != Domain.Enums.State.Deleted)
                        .Select( x=> new CategoryComplanitDto()
                        {
                            DescriptionAr = x.DescriptionAr,
                            DescriptionEn= x.DescriptionEn,
                            Id= x.Id,
                            NameAr= x.NameAr,
                            NameEn = x.NameEn
                        }).ToList();
                    if(DateTime.Now.Hour == 19 && DateTime.Now.Minute > 35 && DateTime.Now.Minute < 30 )
                    {
                        entityJobs = entityJobs.Where(x=>!x.NameAr.Contains("نظاف")).ToList();
                    }

                    _response.Result = entityJobs;
                    _response.StatusEnum = StatusEnum.Success;
                  
                    _response.Message = "AddedSuccessfully";

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
