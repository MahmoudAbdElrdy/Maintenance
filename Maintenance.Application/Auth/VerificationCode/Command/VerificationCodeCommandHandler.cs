using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Auth.Login;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.Helper;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maintenance.Application.Auth.VerificationCode.Command
{
    class VerificationCodeCommandHandler : IRequestHandler<VerificationCodeCommand, ResponseDTO>
    {

        private readonly ILogger<VerificationCodeCommand> _logger;
        private readonly ResponseDTO _response;
        private readonly UserManager<User> _userManager;
        public ResponseDTO Response => _response;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationProvider _localizationProvider;
        private readonly IAuditService _auditService;
        public VerificationCodeCommandHandler(IMapper mapper,
            UserManager<User> userManager,
            IConfiguration configuration,
             IAuditService auditService,
             ILocalizationProvider localizationProvider,
            ILogger<VerificationCodeCommand> logger)
        
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _response = new ResponseDTO();
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper;
            _configuration = configuration;
            _auditService = auditService;
            _localizationProvider = localizationProvider;
        }

        public async Task<ResponseDTO> Handle(VerificationCodeCommand request, CancellationToken cancellationToken)

        {
            try
            {
                var userLogin = await _userManager.Users.Where(x => x.Id == request.UserId).FirstOrDefaultAsync();
                
                if (userLogin == null)
                {
                    _response.StatusEnum = StatusEnum.Failed;
                   // _response.Message = "UserNotFound";
                    _response.Message = _localizationProvider.Localize("UserNotFound", _auditService.UserLanguage);

                    return _response;
                }
                var userCode = await _userManager.Users.Where(x => x.Id == request.UserId && x.Code == request.Code).FirstOrDefaultAsync();

                if (userCode == null)
                {
                    _response.StatusEnum = StatusEnum.Failed;
                  
                    _response.Message = _localizationProvider.Localize("codeNotCorrect", _auditService.UserLanguage);

                    return _response;
                }
            

                var authorizedUserDto = new AuthorizedUserDTO
                {
                    User = _mapper.Map<UserDto>(userLogin),
                    Token = GenerateJSONWebToken(userLogin),
                };
               // await _userManager.UpdateAsync(userLogin);
                _response.StatusEnum = StatusEnum.Success;
            
                _response.Message = _localizationProvider.Localize("mobileVerficiationSuccess", _auditService.UserLanguage);

                _response.Result = authorizedUserDto;
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

            var claims = new ClaimsIdentity(new List<Claim>() {
                    new Claim("userLoginId", user.Id.ToString()),
                    new Claim("identityNumber", user.IdentityNumber),
                    new Claim("FullName", user.FullName),
                     });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,              // Not required as no third-party is involved
                Audience = audience,            // Not required as no third-party is involved
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(expiryDuration),
                Subject = claims,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(jwtToken);
            return token;
        }
    }
}
