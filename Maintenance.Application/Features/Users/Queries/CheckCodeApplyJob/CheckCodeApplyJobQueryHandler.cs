using AuthDomain.Entities.Auth;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Users.Queries.CheckCodeApplyJob
{
    class CheckCodeApplyJobQueryHandler : IRequestHandler<CheckCodeApplyJobQuery, ResponseDTO>
    {
        private readonly ILogger<CheckCodeApplyJobQueryHandler> _logger;
        private readonly ResponseDTO _response;
        private readonly IConfiguration _configuration;
        private readonly IGRepository<TemporaryUser> _temporaryUserRepository;
        public CheckCodeApplyJobQueryHandler(ILogger<CheckCodeApplyJobQueryHandler> logger,
            IConfiguration configuration, IGRepository<TemporaryUser> temporaryUserRepository)
        {
            _temporaryUserRepository = temporaryUserRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _response = new ResponseDTO();
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<ResponseDTO> Handle(CheckCodeApplyJobQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var temporaryUser =  _temporaryUserRepository.GetAll(x => x.NationalId == request.NationalId).FirstOrDefault();
                
                if (temporaryUser == null)
                {

                    _response.Message = "الرقم الهوية غير موجود";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }
                else if (temporaryUser.Code != request.Code)
                {

                    _response.Message = "الكود غير متشابهه";
                    _response.StatusEnum = StatusEnum.Failed;
                    return _response;
                }

                _response.StatusEnum = StatusEnum.Success;
                _response.Message = "personalUserCodeIsMatchSuccessfully";
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
    }
}