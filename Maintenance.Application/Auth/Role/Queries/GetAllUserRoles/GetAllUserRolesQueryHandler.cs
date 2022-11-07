using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Maintenance.Application.Auth.Role.Queries.GetAllRolesWithoutPagination
{
    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUserRolesQueryHandler> _logger;
        private readonly ResponseDTO _responseDTO;
        private readonly IGRepository<AuthDomain.Entities.Auth.User> _userRepository;
        private readonly IGRepository<Domain.Entities.Auth.PermissionRole> _permissionroleRepository;

        public GetAllUserRolesQueryHandler(IGRepository<AuthDomain.Entities.Auth.Role> roleRepository, IMapper mapper, ILogger<GetAllUserRolesQueryHandler> logger,
            IGRepository<AuthDomain.Entities.Auth.User> userRepository, IGRepository<Domain.Entities.Auth.PermissionRole> permissionroleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseDTO = new ResponseDTO();
            _userRepository = userRepository;
            _permissionroleRepository = permissionroleRepository;
        }

        public async Task<ResponseDTO> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entityObjs = _userRepository.GetAll(s => s.State != Domain.Enums.State.Deleted && s.Id != null)
                    .Include(x => x.UserRoles).ToList();

                var mappedEntities = _mapper.Map<List<GetAllUserRolesDTO>>(entityObjs);

                foreach (var element in mappedEntities)
                {
                    var user = _userRepository.GetFirst(x => x.Id == element.CreatedBy);
                    element.Permissions = _permissionroleRepository.GetAll(s => s.RoleId == element.Id && s.State != Domain.Enums.State.Deleted)
                                          .Select(s => s.PermissionId).ToList();
                }

                _responseDTO.Result = mappedEntities;
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
