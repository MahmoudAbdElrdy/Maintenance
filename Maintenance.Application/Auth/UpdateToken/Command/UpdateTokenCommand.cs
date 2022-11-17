using AuthDomain.Entities.Auth;
using AutoMapper;
using Common.Options;
using Maintenance.Application.Auth.Login;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.Helper;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Auth.UpdateToken.Command
{
    public class UpdateTokenCommand : IRequest<ResponseDTO>
    {
        public long? UserId { get; set; }
        public string? Token { get; set; }
        class TokenCommandHandler : IRequestHandler<UpdateTokenCommand, ResponseDTO>
        {

            private readonly ILogger<UpdateTokenCommand> _logger;
            private readonly ResponseDTO _response;
            private readonly UserManager<User> _userManager;
            public ResponseDTO Response => _response;
            private readonly IMapper _mapper;
        
            private readonly IStringLocalizer<UpdateTokenCommand> _localizationProvider;
         
            public TokenCommandHandler(IMapper mapper,
                UserManager<User> userManager,
                IConfiguration configuration,
                 IAuditService auditService,
                  IStringLocalizer<UpdateTokenCommand> localizationProvider,
                 JwtOption jwtOption,
                ILogger<UpdateTokenCommand> logger)

            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
                _mapper = mapper;
             
                _localizationProvider = localizationProvider;
               
            }

            public async Task<ResponseDTO> Handle(UpdateTokenCommand request, CancellationToken cancellationToken)

            {
                try
                {
                    var userLogin = await _userManager.Users.Where(x => x.Id == request.UserId).FirstOrDefaultAsync();

                    if (userLogin == null)
                    {
                        _response.StatusEnum = StatusEnum.Failed;
                    
                        _response.Message = _localizationProvider["UserNotFound"];

                        return _response;
                    }

                    userLogin.Token = request.Token;

                    var authorizedUserDto = new AuthorizedUserDTO
                    {
                        User = _mapper.Map<UserDto>(userLogin),
                        Token = null,
                    };
                    await _userManager.UpdateAsync(userLogin);
                    _response.StatusEnum = StatusEnum.Success;

                    _response.Message = _localizationProvider["Success"];

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
           

          
        }

    }
}
