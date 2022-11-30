using AuthDomain.Entities.Auth;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Users.Queries.GenerateCodeApplyJob
{
    class GetGenerateCodeApplyJobQueryHandler : IRequestHandler<GetGenerateCodeApplyJobQuery, ResponseDTO>
    {
        private readonly ILogger<GetGenerateCodeApplyJobQueryHandler> _logger;
        private readonly ResponseDTO _response;
        private readonly IGRepository<TemporaryUser> _temporaryUserRepository;

        public GetGenerateCodeApplyJobQueryHandler(
             ILogger<GetGenerateCodeApplyJobQueryHandler> logger,
             IGRepository<TemporaryUser> temporaryUserRepository)
        {
            _temporaryUserRepository = temporaryUserRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _response = new ResponseDTO();
        }
        public async Task<ResponseDTO> Handle(GetGenerateCodeApplyJobQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var temporaryUser =  _temporaryUserRepository.GetAll(x => x.NationalId.ToString() == request.NationalId).FirstOrDefault();
                if (temporaryUser == null)
                {
                    var temp = new TemporaryUser();
                    temp.NationalId = long.Parse(request.NationalId);
                    temp.Code = GenerateCode();
                    temp.PhoneNumber = request.MobileNumber;

                    var serviceSMs = new SMSService();
                    var resultSms =await serviceSMs.SendMessageUnifonic("رمز التحقق من الجوال : " + temp.Code, temp.PhoneNumber);
                    if (resultSms == -1)
                    {
                        _response.Message = "حدث خطا فى ارسال الكود";
                        _response.StatusEnum = StatusEnum.Failed;
                        return _response;
                    }

                    _temporaryUserRepository.Add(temp);
                    _temporaryUserRepository.Save();
                }
                else
                {
                    temporaryUser.Code =GenerateCode();
                    temporaryUser.PhoneNumber = request.MobileNumber;

                    var serviceSMs = new SMSService();
                    var resultSms =await serviceSMs.SendMessageUnifonic("رمز التحقق من الجوال : " + temporaryUser.Code, temporaryUser.PhoneNumber);
                    if (resultSms == -1)
                    {
                        _response.Message = "حدث خطا فى ارسال الكود";
                        _response.StatusEnum = StatusEnum.Failed;
                        return _response;
                    }


                    _temporaryUserRepository.Update(temporaryUser);
                    _temporaryUserRepository.Save();
                }
                _response.StatusEnum = StatusEnum.Success;
                _response.Message = "تم ارسال الكود";

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
