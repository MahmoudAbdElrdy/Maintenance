using AutoMapper;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Maintenance.Application.Auth.Role.Commands.PostUserRole
{
    public class PostUserRoleCommandHandler : IRequestHandler<PostUserRoleCommand, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IGRepository<AuthDomain.Entities.Auth.User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PostUserRoleCommandHandler> _logger;
        private readonly ResponseDTO _responseDTO;
        public long _loggedInUserId;

        public PostUserRoleCommandHandler(IGRepository<AuthDomain.Entities.Auth.Role> roleRepository,
            IGRepository<AuthDomain.Entities.Auth.User> userRepository,
            IMapper mapper, ILogger<PostUserRoleCommandHandler> logger,
            IHttpContextAccessor _httpContextAccessor)

        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseDTO = new ResponseDTO();
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


        public async Task<ResponseDTO> Handle(PostUserRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entityObj = _userRepository.GetAll(x=>x.Id == request.UserId).FirstOrDefault();

                if (entityObj == null)
                {
                    _responseDTO.StatusEnum = StatusEnum.FailedToFindTheObject;
                    _responseDTO.Message = "الموظف غير موجود";
                    return _responseDTO;
                }
                if(entityObj.Id == request.RoleId)
                {
                    _responseDTO.StatusEnum = StatusEnum.FailedToFindTheObject;
                    _responseDTO.Message = "مضاف مسبقا على هذا الدور";
                    return _responseDTO;
                }
                entityObj.Id = request.RoleId;

                _userRepository.Update(entityObj);
                _userRepository.Save();

              
                _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;
                _responseDTO.Message = "userRoleAddedSuccessfully";
            }
            catch (Exception ex)
            {
                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Exception;
                _responseDTO.Message = (ex != null && ex.InnerException != null ? ex.InnerException.Message : "");
                _logger.LogError(ex, ex.Message, (ex != null && ex.InnerException != null ? ex.InnerException.Message : ""));
            }

            return _responseDTO;
        }


        private string GenerateSegment()
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var Charsarr = new char[10];
            var random = new Random();

            for (int i = 0; i < Charsarr.Length; i++)
            {
                Charsarr[i] = characters[random.Next(characters.Length)];
            }

            var segmentstring = new String(Charsarr);
            var checkSegmant = _roleRepository.GetAll(s => s.Code == segmentstring).FirstOrDefault();
            if (checkSegmant != null)
                return GenerateSegment();
            else
                return segmentstring;
        }
    }
}
