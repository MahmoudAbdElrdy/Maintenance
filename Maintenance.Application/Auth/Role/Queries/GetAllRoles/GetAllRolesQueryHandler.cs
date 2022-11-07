using AutoMapper;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Maintenance.Application.Auth.Role.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllRolesQueryHandler> _logger;
        private readonly ResponseDTO _responseDTO;
        private readonly IGRepository<Domain.Entities.Auth.Permission> _permissionRepository;
        private readonly IGRepository<Domain.Entities.Auth.PermissionRole> _permissionroleRepository;

        public GetAllRolesQueryHandler(IGRepository<AuthDomain.Entities.Auth.Role> roleRepository, IMapper mapper, ILogger<GetAllRolesQueryHandler> logger,
            IGRepository<Domain.Entities.Auth.Permission> permissionRepository, IGRepository<Domain.Entities.Auth.PermissionRole> permissionroleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseDTO = new ResponseDTO();
            _permissionRepository = permissionRepository;
            _permissionroleRepository = permissionroleRepository;
        }

        public async Task<ResponseDTO> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entityObjs = _roleRepository.GetAll(s => s.State != Domain.Enums.State.Deleted).ToList();

                var mappedEntities = _mapper.Map<List<GetRolesQueryDTO>>(entityObjs);

                foreach (var element in mappedEntities)
                {
                    element.Permissions = _permissionroleRepository.GetAll(s => s.RoleId == element.Id && s.State != Domain.Enums.State.Deleted).Select(s => s.PermissionId).ToList();
                }

                _responseDTO.Result = mappedEntities;
                _responseDTO.StatusEnum = StatusEnum.Success;
                _responseDTO.Message = "rolesRetrievedSuccessfully";
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
