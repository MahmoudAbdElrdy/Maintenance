using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.CheckLists.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Reports;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Features.CheckLists.Queries
{
    public class GetCheckListReportQueryById : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        class GetCheckListReportByIdQueryHandler : IRequestHandler<GetCheckListReportQueryById, ResponseDTO>
        {
            private readonly IGRepository<CheckListReport> _CheckListReportRepository;
            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetCheckListReportByIdQueryHandler> _logger;
            private readonly ResponseDTO _response;
            public long _loggedInUserId;
            private readonly IMapper _mapper;
            public GetCheckListReportByIdQueryHandler(
                IHttpContextAccessor _httpContextAccessor,
                IGRepository<CheckListReport> CheckListReportRepository,
                ILogger<GetCheckListReportByIdQueryHandler> logger, IMapper mapper,
                IGRepository<User> userRepository
            )
            {
                _mapper = mapper;
                _CheckListReportRepository = CheckListReportRepository;
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
            public async Task<ResponseDTO> Handle(GetCheckListReportQueryById request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _userRepository.GetFirst(x => x.Id == _loggedInUserId);
                    if (user == null)
                    {

                        _response.StatusEnum = StatusEnum.Failed;
                        _response.Message = "userNotFound";
                    }

                    var entityCheckListReport = _CheckListReportRepository.GetAll(x => x.State != Domain.Enums.State.Deleted).FirstOrDefault();
                    if (entityCheckListReport == null)
                    {

                        _response.StatusEnum = StatusEnum.Failed;
                        _response.Message = "CheckListReportNotFound";
                    }
                    var mappedObj = _mapper.Map<CheckListReportDto>(entityCheckListReport);
                    _response.Result = mappedObj;
                    _response.StatusEnum = StatusEnum.Success;
                    _response.Message = "CheckListReportRetrievedSuccessfully";

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
