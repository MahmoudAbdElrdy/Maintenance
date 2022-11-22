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
        }

        public async Task<ResponseDTO> Handle(VerificationCodeComplanit request, CancellationToken cancellationToken)

        {
            try
            {
               
                var userCode = await _RequestComplanitRepository.GetFirstAsync(x => x.Id == request.RequestComplanitId && x.Code == request.CodeSms);

                if (userCode == null)
                {
                    _response.StatusEnum = StatusEnum.Failed;
                  
                    _response.Message = _localizationProvider["codeNotCorrect"];

                    return _response;
                }
            

                //var authorizedUserDto = new AuthorizedUserDTO
                //{
                //    User = _mapper.Map<UserDto>(userLogin),
                //    Token = GenerateJSONWebToken(userLogin),
                //};
               // await _userManager.UpdateAsync(userLogin);
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
        private string GenerateJSONWebToken(User user)
        {
            var signingKey = Convert.FromBase64String(_configuration["JwtOption:Key"]);
            var audience = _configuration["JwtOption:Audience"];
            var expiryDuration = int.Parse(_configuration["JwtOption:ExpiryDuration"]);
            var issuer = _configuration["JwtOption:Issuer"];

            var claims = (new List<Claim>() {
                    new Claim("userLoginId", user.Id.ToString()),
                    new Claim("identityNumber", user.IdentityNumber),
                    new Claim("FullName", user.FullName),
                    new Claim("UserType", user.UserType.ToString())
                     });
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtOption.ExpireDays));

            var tokenDescriptor = new JwtSecurityToken(
            _jwtOption.Issuer,
              _jwtOption.Issuer,
              claims,
              expires: expires,
              signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
     
    }
}
