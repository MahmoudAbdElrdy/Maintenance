using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Auth.Login;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Unifonic.NetCore.Exceptions;

namespace Maintenance.Application.Features.Account.Commands.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, ResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<LoginQueryHandler> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _response;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        

        public LoginQueryHandler(
            IMapper mapper, ILogger<LoginQueryHandler> logger,
         
            UserManager<User> userManager,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration
          
        )
        {
          
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _response = new ResponseDTO();
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
          
            
        }
        public async Task<ResponseDTO> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                var personalUser =  await _userManager.Users.Where(x => x.IdentityNumber == request.IdentityNumber).FirstOrDefaultAsync();
                if (personalUser == null)
                {

                    _response.Message = "nationalIdNotFound";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }
               
             
                else if (personalUser.State == Domain.Enums.State.Deleted)
                {
                    _response.Message = "userAreDeleted";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }
                var userHasValidPassword = await _userManager.CheckPasswordAsync(personalUser, request.Password);

                if (!userHasValidPassword)
                {
                    _response.Message = "PassWordNotCorrect";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;

                }
                personalUser.Code = SendSMS.GenerateCode();
                var res = await SendSMS.SendMessageUnifonic("رمز التحقق من الجوال : " + personalUser.Code, personalUser.PhoneNumber);
                if (res == -1)
                {
                    _response.Message = "حدث خطا فى ارسال الكود";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }
                await _userManager.UpdateAsync(personalUser);
                var authorizedUserDto = new AuthorizedUserDTO
                {
                    User = _mapper.Map<UserDto>(personalUser),
                    Token = GenerateJSONWebToken(personalUser),
                };

                _response.StatusEnum = StatusEnum.Success;
                _response.Message = "userLoggedInSuccessfully";
                _response.Result = authorizedUserDto;

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

        private string GenerateJSONWebToken(User user)
        {
            var signingKey = Convert.FromBase64String(_configuration["Jwt:Key"]);
            var audience = _configuration["Jwt:Audience"];
            var expiryDuration = int.Parse(_configuration["Jwt:ExpiryDuration"]);
            var issuer = _configuration["Jwt:Issuer"];

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