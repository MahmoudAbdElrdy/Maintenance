using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Maintenance.Application.Auth.Role.Commands.UpdateRole
{
    public class PutRoleCommandHandler : IRequestHandler<PutRoleCommand, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IGRepository<Domain.Entities.Auth.PermissionRole> _permissionRoleRepository;
        private readonly ILogger<PutRoleCommandHandler> _logger;
        private readonly ResponseDTO _responseDTO;
        public long _loggedInUserId;

        public PutRoleCommandHandler(IGRepository<AuthDomain.Entities.Auth.Role> roleRepository, ILogger<PutRoleCommandHandler> logger,
            IGRepository<Domain.Entities.Auth.PermissionRole> permissionRoleRepository,
            IHttpContextAccessor _httpContextAccessor)

        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
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

        public async Task<ResponseDTO> Handle(PutRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dbEntityObj = await _roleRepository.GetFirstAsync(x => x.Id == request.Id && x.State != Domain.Enums.State.Deleted);

                if (dbEntityObj == null)
                {
                    _responseDTO.StatusEnum = StatusEnum.FailedToFindTheObject;
                    _responseDTO.Result = null;
                    _responseDTO.Message = "roleNotFound";
                    return _responseDTO;
                }

                dbEntityObj.Name = request.Name;
                dbEntityObj.UpdatedOn = DateTime.Now;
                dbEntityObj.UpdatedBy = _loggedInUserId;

                _roleRepository.Update(dbEntityObj);
                _roleRepository.Save();

/*                var rolePermissions = _permissionRoleRepository.GetAll(s => s.RoleId == request.Id).ToList();

                var deletePermissions = rolePermissions.Where(x => x.State != Domain.Enums.State.Deleted && !request.Permissions.Contains(x.PermissionId)).ToList();
                var existingPermissions = rolePermissions.Where(x => request.Permissions.Contains(x.PermissionId)).ToList();
                var addedPermissions = request.Permissions.Except(rolePermissions.Select(x => x.PermissionId).ToList()).ToList();

                foreach (var permission in existingPermissions)
                {
                    permission.State = Domain.Enums.State.NotDeleted;
                    _permissionRoleRepository.Update(permission);
                    _permissionRoleRepository.Save();
                }


                foreach (var permission in deletePermissions)
                {
                    permission.State = Domain.Enums.State.Deleted;
                    _permissionRoleRepository.Update(permission);
                    _permissionRoleRepository.Save();
                }

                List<Domain.Entities.PermissionRoleEntity.PermissionRole> newPermissionRoles = new List<Domain.Entities.PermissionRoleEntity.PermissionRole>();
                foreach (var perm in addedPermissions)
                {
                    newPermissionRoles.Add(new Domain.Entities.PermissionRoleEntity.PermissionRole() { RoleId = request.Id, PermissionId = perm });
                }

                _permissionRoleRepository.AddRange(newPermissionRoles);
                _permissionRoleRepository.Save();

*/                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;
                _responseDTO.Message = "roleUpdatedSuccessfully";
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
