using AuthDomain.Entities.Auth;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Auth.Permission.Command
{
    class PostPersmissionCommandHandler : IRequestHandler<PostPersmissionCommand, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IGRepository<PermissionRole> _permissionRoleRepository;
        private readonly IGRepository<Domain.Entities.Auth.Permission> _permissionRepository;
        private readonly ResponseDTO _responseDTO;
        private readonly ILogger<PostPersmissionCommandHandler> _logger;

        public PostPersmissionCommandHandler(
            ILogger<PostPersmissionCommandHandler> logger, IGRepository<AuthDomain.Entities.Auth.Role> roleRepository,
            IGRepository<PermissionRole> permissionRoleRepository,
            IGRepository<Domain.Entities.Auth.Permission> permissionRepository)
        {
            _permissionRoleRepository = permissionRoleRepository;
            _permissionRepository = permissionRepository;
            _permissionRepository = permissionRepository;
            _responseDTO = new ResponseDTO();
            _logger = logger;

        }
        public async Task<ResponseDTO> Handle(PostPersmissionCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var allPermissions = _permissionRepository.GetAll(x => x.State != Domain.Enums.State.Deleted).ToList();

                var foundedPermissionRole = _permissionRoleRepository.GetAll(x => x.RoleId == request.RoleId).ToList();
                if (foundedPermissionRole.Count() > 0)
                {
                    foreach (var item in foundedPermissionRole)
                    {
                        var permission = request.Permissions.Where(x => x.Id == item.PermissionId).FirstOrDefault();
                        item.IsChecked = permission != null ? true : false;
                        item.PermissionAuthorize = permission != null ? permission.PermissionAuthorize : Domain.Enums.PermissionAuthorize.Others;
                        _permissionRoleRepository.Update(item);
                        _permissionRoleRepository.Save();
                    }

                    _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;
                    _responseDTO.Message = "permissionsAddedSuccessfully";
                    return _responseDTO;
                }
                List<PermissionRole> permissionRoles = new List<PermissionRole>();

                if (request.Permissions != null && request.Permissions.Count() > 0)
                {
                    foreach (var dbPermission in allPermissions)
                    {

                        var permission = request.Permissions.Where(x => x.Id == dbPermission.Id).FirstOrDefault();
                        permissionRoles.Add(new PermissionRole()
                        {
                            IsChecked = permission != null ? true : false,
                            RoleId = request.RoleId,
                            PermissionId = dbPermission.Id,
                            State = Domain.Enums.State.NotDeleted,
                            PermissionAuthorize = permission != null ? permission.PermissionAuthorize : Domain.Enums.PermissionAuthorize.Others,

                        });
                    }

                    _permissionRoleRepository.AddRange(permissionRoles);
                    _permissionRoleRepository.Save();
                }
                _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;
                _responseDTO.Message = "permissionsAddedSuccessfully";
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
    }
}