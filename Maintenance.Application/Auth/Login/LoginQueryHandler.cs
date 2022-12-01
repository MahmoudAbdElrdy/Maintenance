using AuthDomain.Entities.Auth;
using AutoMapper;
using Common.Options;
using Maintenance.Application.Auth.Client.Command;
using Maintenance.Application.Auth.Login;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly JwtOption _jwtOption;
        private readonly IAuditService _auditService;
        private readonly IStringLocalizer<LoginQueryHandler> _localizationProvider;
        public LoginQueryHandler(
            IMapper mapper, ILogger<LoginQueryHandler> logger,
         
            UserManager<User> userManager,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration,
            JwtOption jwtOption,
             IAuditService auditService,
              IStringLocalizer<LoginQueryHandler> localizationProvider

        )
        {
          
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _response = new ResponseDTO();
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtOption = jwtOption;
            _auditService = auditService;
            _localizationProvider = localizationProvider;

        }
        public async Task<ResponseDTO> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                var personalUser =  await _userManager.Users.Where(x => x.IdentityNumber == request.IdentityNumber).FirstOrDefaultAsync();
                if (personalUser == null)
                {

                  
                    _response.Message = _localizationProvider["NationalNumberNotFound"];

                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }
               
             
                else if (personalUser.State == Domain.Enums.State.Deleted)
                {
                   
                    _response.Message = _localizationProvider["userAreDeleted"];

                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }
                var personalVefiy = await _userManager.Users.Where(x => x.IdentityNumber == request.IdentityNumber && x.IsVerifyCode == true).FirstOrDefaultAsync();
               
                if (personalVefiy == null)
                {


                    _response.Message = _localizationProvider["NotVerficiation"];

                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }

                var userHasValidPassword = await _userManager.CheckPasswordAsync(personalUser, request.Password);

                if (!userHasValidPassword)
                {
                   
                    _response.Message = _localizationProvider["PassWordNotCorrect"];

                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;

                }
                personalUser.Code = SendSMS.GenerateCode();
             
                var meass = _localizationProvider["Mobileverificationcode"];

                var res = SendSMS.SendMessageUnifonic(meass + " : " + personalUser.Code, personalUser.PhoneNumber);
                if (res == -1)
                {

                    //if (await _userManager.FindByNameAsync(personalUser.UserName) != null)
                    //{
                    //    await _userManager.DeleteAsync(personalUser);
                    //}

                    _response.Message = _localizationProvider["ProplemSendCode"];

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
               
                _response.Message = _localizationProvider["userLoggedInSuccessfully"];

                _response.Result = authorizedUserDto;

                return _response;
            }
            catch (Exception ex)
            {
                _response.StatusEnum = StatusEnum.Exception;
                _response.Result = null;
                _response.Message = _localizationProvider["anErrorOccurredPleaseContactSystemAdministrator"];

            //    _response.Message = (ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                _logger.LogError(ex, ex.Message, (ex != null && ex.InnerException != null ? ex.InnerException.Message : ""));

                return _response;
            }
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