using AuthDomain.Entities.Auth;
using AutoMapper;
using Common.Options;
using Maintenance.Application.Auth.Login;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Maintenance.Application.Features.RequestsComplanit.Command
{
    class VerificationCodeCommandHandler : IRequestHandler<VerificationCodeComplanit, ResponseDTO>
    {

        private readonly ILogger<VerificationCodeComplanit> _logger;
        private readonly ResponseDTO _response;
        private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
        private readonly IGRepository<ComplanitHistory> _ComplanitHistoryRepository;
        public ResponseDTO Response => _response;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<VerificationCodeCommandHandler> _localizationProvider;
        private readonly IAuditService _auditService;
        private readonly JwtOption _jwtOption;
        public VerificationCodeCommandHandler(IMapper mapper,
           IGRepository<RequestComplanit> RequestComplanitRepository,
            IConfiguration configuration,
             IAuditService auditService,
              IStringLocalizer<VerificationCodeCommandHandler> localizationProvider,
              IGRepository<ComplanitHistory> ComplanitHistoryRep,
             JwtOption jwtOption,
            ILogger<VerificationCodeComplanit> logger)
        
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _response = new ResponseDTO();
            _RequestComplanitRepository = RequestComplanitRepository;
            _mapper = mapper;
            _configuration = configuration;
            _auditService = auditService;
            _localizationProvider = localizationProvider;
            _jwtOption = jwtOption;
            _ComplanitHistoryRepository = ComplanitHistoryRep;
        }

        public async Task<ResponseDTO> Handle(VerificationCodeComplanit request, CancellationToken cancellationToken)

        {
            try
            {
               
                var userCode = await _RequestComplanitRepository.GetFirstAsync(x => x.Id == request.RequestComplanitId && x.CodeSms == request.CodeSms);

                if (userCode == null)
                {
                    _response.StatusEnum = StatusEnum.Failed;
                  
                    _response.Message = _localizationProvider["codeNotCorrect"];

                    return _response;
                }
                userCode.UpdatedOn = DateTime.Now;
                userCode.ComplanitStatus = Domain.Enums.ComplanitStatus.TechnicianDone;
                var complanitHistory = new ComplanitHistory()
                {
                    CreatedBy = _auditService.UserId,
                    CreatedOn = DateTime.Now,
                    State = Domain.Enums.State.NotDeleted,
                    Description = Domain.Enums.ComplanitStatus.TechnicianDone.ToString(),
                    ComplanitStatus = Domain.Enums.ComplanitStatus.TechnicianDone,//TechnicianClosed no message
                    RequestComplanitId = request.RequestComplanitId
                };

               


                _RequestComplanitRepository.Update(userCode);
          
                await _ComplanitHistoryRepository.AddAsync(complanitHistory);
              
                _RequestComplanitRepository.Save();
               
                _response.StatusEnum = StatusEnum.Success;
            
                _response.Message = _localizationProvider["mobileVerficiationSuccess"];

                _response.Result = null;
            }
            catch (Exception ex)
            {
                _response.StatusEnum = StatusEnum.Exception;
                _response.Result = null;
                _response.Message = (ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                _logger.LogError(ex, ex.Message, (ex != null && ex.InnerException != null ? ex.InnerException.Message : ""));
                return _response;
            }

            return _response;
        }
        
     
    }
}
