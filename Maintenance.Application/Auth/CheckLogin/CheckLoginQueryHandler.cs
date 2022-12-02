using AuthDomain.Entities.Auth;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Account.Queries.CheckLogin
{
    class CheckLoginQueryHandler : IRequestHandler<CheckLoginQuery, ResponseDTO>
    {
        private readonly ILogger<CheckLoginQuery> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _response;
        private readonly IPasswordHasher<User> _passwordHasher;
            public CheckLoginQueryHandler(
            ILogger<CheckLoginQuery> logger,
            UserManager<User> userManager,
            IPasswordHasher<User> passwordHasher
        )
        {
           
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _response = new ResponseDTO();
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }
        public async Task<ResponseDTO> Handle(CheckLoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var personalUser = await _userManager.Users.Where(x => x.IdentityNumber == request.IdentityNumber
                && (x.UserType==UserType.Owner || x.UserType == UserType.Consultant)).FirstOrDefaultAsync();
                if (personalUser == null)
                {

                    _response.Message = "nationalIdNotFound";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }
                var userLogin = await _userManager.Users.Where(x => (x.PhoneNumber == personalUser.PhoneNumber))
                    .FirstOrDefaultAsync();

                if (userLogin == null)
                {
                    _response.Message = "phoneNumberNotFound";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }

               
                    if (_passwordHasher.VerifyHashedPassword(userLogin, userLogin.PasswordHash, request.Password) != PasswordVerificationResult.Success)
                    {
                        userLogin.AccessFailedCount += 1;
                        await _userManager.UpdateAsync(userLogin);

                        _response.Message = "invalidPassword";
                        _response.StatusEnum = StatusEnum.Failed;
                        return _response;
                    }

                userLogin.Code = GenerateCode();

                var smsService = new SMSService();
                var result = await smsService.SendMessageUnifonic("رمز الدخول الخاص للنظام : " + userLogin.Code, userLogin.PhoneNumber);
                if (result == -1)
                {
                    _response.Message = "حدث خطا فى ارسال الكود";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }

                await _userManager.UpdateAsync(userLogin);

                if (userLogin.AccessFailedCount > 0)
                {
                    userLogin.AccessFailedCount = 0;
                    await _userManager.UpdateAsync(userLogin);
                }

             
                _response.StatusEnum = StatusEnum.Success;
                _response.Message = "userLoggedInSuccessfully";
                _response.Result = null;

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



        public string GenerateCode()
        {
            var characters = "0123456789";
            var charsArr = new char[4];
            var random = new Random();
            for (int i = 0; i < charsArr.Length; i++)
            {
                charsArr[i] = characters[random.Next(characters.Length)];
            }
            var segmentString = new String(charsArr);
            return segmentString;
        }
    }
}
