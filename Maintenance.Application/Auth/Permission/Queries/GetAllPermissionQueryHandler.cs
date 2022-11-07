using AutoMapper;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Auth.Permission.Queries
{
    public class PermissionGetAllQueryHandler : IRequestHandler<GetAllPermissionQuery, ResponseDTO>
    {
        private readonly IGRepository<Domain.Entities.Auth.Permission> _permissionRepository;
        private readonly IGRepository<Domain.Entities.Auth.PermissionRole> _permissionRoleRepository;
        private readonly IMapper _mapper;
        private readonly ResponseDTO _responseDTO;
        private readonly ILogger<PermissionGetAllQueryHandler> _logger;

        public PermissionGetAllQueryHandler(IGRepository<Domain.Entities.Auth.Permission> permissionRepository,
            IGRepository<Domain.Entities.Auth.PermissionRole> permissionRoleRepository, IMapper mapper
            , ILogger<PermissionGetAllQueryHandler> logger)
        {
            _permissionRoleRepository = permissionRoleRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _responseDTO = new ResponseDTO();
            _logger = logger;

        }
        public async Task<ResponseDTO> Handle(GetAllPermissionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var permissions = _permissionRoleRepository.GetAll(x => x.RoleId == request.RoleId).Include(x=>x.Permission).ToList();
                if (permissions.Count() != 0)
                {
                    List <GetAllPermissionDTO> mappingObjs = new List<GetAllPermissionDTO>();
                    foreach (var item in permissions)
                    {
                        var obj = new GetAllPermissionDTO()
                        {
                            ControllerName = item.Permission.ControllerName,
                            Id = item.Permission.Id,
                            ActionName = item.Permission.ActionName,
                            IsChecked = item.IsChecked,
                            Name = item.Permission.Name,
                            State = Domain.Enums.State.NotDeleted,
                            PermissionAuthorize = item.PermissionAuthorize
                        };
                        mappingObjs.Add(obj);
                    }
                    _responseDTO.Result = mappingObjs;
                    _responseDTO.StatusEnum = StatusEnum.Success;
                    _responseDTO.Message = "permissionsRetrievedSuccessfully";
                    return _responseDTO;
                }
                else
                {
                    var originalPermissions = _permissionRepository.GetAll(x => x.State != Domain.Enums.State.Deleted).ToList();

                    var mappingObjs = _mapper.Map<List<GetAllPermissionDTO>>(originalPermissions);

                    _responseDTO.Result = mappingObjs;
                    _responseDTO.StatusEnum = StatusEnum.Success;
                    _responseDTO.Message = "permissionsRetrievedSuccessfully";
                }
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