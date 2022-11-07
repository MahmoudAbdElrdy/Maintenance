using AutoMapper;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Auth.Role.Queries.GetUsersRole
{
    public class GetUsersRoleQueryHandler : IRequestHandler<GetUsersRoleQuery, ResponseDTO>
    {
        private readonly IGRepository<Domain.Entities.RoleEntity.Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersRoleQueryHandler> _logger;
        private readonly ResponseDTO _responseDTO;
        private readonly IGRepository<Domain.Entities.UserEntity.User> _userRepository;
        private readonly IGRepository<Domain.Entities.PermissionRoleEntity.PermissionRole> _permissionroleRepository;

        public GetUsersRoleQueryHandler(IGRepository<Domain.Entities.RoleEntity.Role> roleRepository, IMapper mapper, ILogger<GetUsersRoleQueryHandler> logger,
            IGRepository<Domain.Entities.UserEntity.User> userRepository, IGRepository<Domain.Entities.PermissionRoleEntity.PermissionRole> permissionroleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseDTO = new ResponseDTO();
            _userRepository = userRepository;
            _permissionroleRepository = permissionroleRepository;
        }

        public async Task<ResponseDTO> Handle(GetUsersRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entityObjs = _userRepository.GetAll(s => s.State != Domain.Enums.State.Deleted && s.RoleId == request.RoleId).Include(x=>x.Skeleton)
                    .Include(x=>x.PersonalData).ThenInclude(x => x.JobsManagement).Include(x => x.Role).Select(x=>new GetAllUserRolesDTO()
                    {
                        Id = x.Id,
                        ProfileImage = x.PersonalData.Image,
                        SkeletonName = x.Skeleton.NameAr,
                        JobName = x.PersonalData.JobsManagement.JobName,
                        UserName = x.PersonalData.FirstName + " " + x.PersonalData.FourthName
                    }).ToList();
               

                _responseDTO.Result = entityObjs;
                _responseDTO.StatusEnum = StatusEnum.Success;
                _responseDTO.Message = "userRolesRetrievedSuccessfully";
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
