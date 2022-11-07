using AutoMapper;
using Maintenance.Application.Auth.UserRole.Commands.AddRole;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Maintenance.Application.Auth.Role.Commands.AddRole
{
    public class PostRoleCommandHandler : IRequestHandler<PostRoleCommand, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IGRepository<Domain.Entities.Auth.PermissionRole> _permissionRoleRepository;

        private readonly IMapper _mapper;
        private readonly ILogger<PostRoleCommandHandler> _logger;
        private readonly ResponseDTO _responseDTO;
        public long _loggedInUserId;

        public PostRoleCommandHandler(IGRepository<AuthDomain.Entities.Auth.Role> roleRepository,
            IMapper mapper, ILogger<PostRoleCommandHandler> logger,
            IGRepository<Domain.Entities.Auth.PermissionRole> permissionRoleRepository,
            IHttpContextAccessor _httpContextAccessor)

        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseDTO = new ResponseDTO();
            _permissionRoleRepository = permissionRoleRepository;

            try
            {
                _loggedInUserId = long.Parse(_httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == "userLoginId").SingleOrDefault().Value);
            }
            catch (Exception ex)
            {
              //   throw new UnauthorizedAccessException();
            }
        }


        public async Task<ResponseDTO> Handle(PostRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entityObj = new AuthDomain.Entities.Auth.Role();

                entityObj.CreatedOn = DateTime.Now;
                entityObj.UpdatedOn = DateTime.Now;
                entityObj.CreatedBy = _loggedInUserId;
                entityObj.UpdatedBy = _loggedInUserId;
                entityObj.Code = GenerateSegment();
                entityObj.State = Domain.Enums.State.NotDeleted;
                entityObj.Name = request.RoleName;
                await _roleRepository.AddAsync(entityObj);
                _roleRepository.Save();

                //if (request.Permissions != null && request.Permissions.Count > 0)
                //{
                //    List<Domain.Entities.PermissionRoleEntity.PermissionRole> permissionRoles = new List<Domain.Entities.PermissionRoleEntity.PermissionRole>();

                //    foreach (var perm in request.Permissions)
                //    {
                //        permissionRoles.Add(new Domain.Entities.PermissionRoleEntity.PermissionRole()
                //        {
                //            RoleId = entityObj.Id,
                //            PermissionId = perm
                //        });
                //    }

                //    _permissionRoleRepository.AddRange(permissionRoles);
                //    _permissionRoleRepository.Save();
                //}

                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;
                _responseDTO.Message = "roleAddedSuccessfully";
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
