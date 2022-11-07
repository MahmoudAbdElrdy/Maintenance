using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Maintenance.Application.Auth.Role.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IGRepository<Domain.Entities.Auth.PermissionRole> _permissionRoleRepository;
        private readonly IGRepository<AuthDomain.Entities.Auth.UserRole> _userRoleRepository;

        private readonly ILogger<DeleteRoleCommandHandler> _logger;
        private readonly ResponseDTO _responseDTO;
        public long _loggedInUserId;

        public DeleteRoleCommandHandler(IGRepository<AuthDomain.Entities.Auth.Role> roleRepository, ILogger<DeleteRoleCommandHandler> logger,
               IGRepository<Domain.Entities.Auth.PermissionRole> permissionRoleRepository, IGRepository<AuthDomain.Entities.Auth.UserRole> userRoleRepository, IHttpContextAccessor _httpContextAccessor)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseDTO = new ResponseDTO();
            _permissionRoleRepository = permissionRoleRepository;
            _userRoleRepository = userRoleRepository;

            try
            {
                _loggedInUserId = long.Parse(_httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == "userLoginId").SingleOrDefault().Value);
            }
            catch (Exception ex)
            {
              //   throw new UnauthorizedAccessException();
            }
        }

        public async Task<ResponseDTO> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entityObj = await _roleRepository.GetFirstAsync(x => x.Id == request.id && x.State != Domain.Enums.State.Deleted);

                if (entityObj == null)
                {
                    _responseDTO.StatusEnum = StatusEnum.FailedToFindTheObject;
                    _responseDTO.Result = null;
                    _responseDTO.Message = "roleNotFound";
                    return _responseDTO;
                }

                var userLogin = await _userRoleRepository.GetAll(x => x.RoleId == request.id && x.State != Domain.Enums.State.Deleted).FirstOrDefaultAsync();

                if (userLogin != null)
                {
                    _responseDTO.StatusEnum = StatusEnum.Failed;
                    _responseDTO.Result = null;
                    _responseDTO.Message = "roleIsConnectedToUserAndCanNotDeletedRole";
                    return _responseDTO;
                }


                var permissionsrole = _permissionRoleRepository.GetAll(s => s.RoleId == request.id).ToList();

                foreach (var permission in permissionsrole)
                {
                    permission.State = Domain.Enums.State.Deleted;

                    _permissionRoleRepository.Update(permission);
                    _permissionRoleRepository.Save();
                }

                entityObj.State = Domain.Enums.State.Deleted;
                entityObj.UpdatedOn = DateTime.Now;
                entityObj.UpdatedBy = _loggedInUserId;

                _roleRepository.Update(entityObj);
                _roleRepository.Save();

                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Success;
                _responseDTO.Message = "roleRemovedSuccessfully";

                return _responseDTO;
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
