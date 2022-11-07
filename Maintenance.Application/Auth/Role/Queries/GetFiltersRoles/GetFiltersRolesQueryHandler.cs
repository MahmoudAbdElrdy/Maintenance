using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Maintenance.Application.Auth.Role.Queries.GetFiltersRoles
{
    public class GetFiltersRolesQueryHandler : IRequestHandler<GetFiltersRolesQuery, ResponseDTO>
    {
        private readonly IGRepository<AuthDomain.Entities.Auth.Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFiltersRolesQuery> _logger;
        private readonly ResponseDTO _responseDTO;
        private readonly IGRepository<AuthDomain.Entities.Auth.User> _userRepository;
        private readonly IGRepository<AuthDomain.Entities.Auth.UserRole> _userRoleRepository;
        private readonly IGRepository<Domain.Entities.Auth.PermissionRole> _permissionroleRepository;

        public GetFiltersRolesQueryHandler(IGRepository<AuthDomain.Entities.Auth.Role> roleRepository, IMapper mapper, ILogger<GetFiltersRolesQuery> logger,
            IGRepository<AuthDomain.Entities.Auth.UserRole> userRoleRepository,
            IGRepository<AuthDomain.Entities.Auth.User> userRepository, IGRepository<Domain.Entities.Auth.PermissionRole> permissionroleRepository)
        {

            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseDTO = new ResponseDTO();
            _userRepository = userRepository;
            _permissionroleRepository = permissionroleRepository;
        }

        public async Task<ResponseDTO> Handle(GetFiltersRolesQuery request, CancellationToken cancellationToken)
            {
            try
            {
 /*               var userIds = _userRepository.GetAll(x => string.IsNullOrEmpty(request.CreatorName) 
                || x.Name.Contains(request.CreatorName)).Select(x => x.Id).ToList();
                var entityObjs = _roleRepository.GetAll(s =>
               ( string.IsNullOrEmpty(request.Name) || s.Name.Contains(request.Name))
                &&  (userIds.Contains(s.CreatedBy.Value))
                && (request.StartDate == null || s.CreatedOn >= request.StartDate)
                && (request.EndDate == null || s.CreatedOn <= request.EndDate)
                && (s.State != Domain.Enums.State.Deleted)).ToList();*/
/*
                var mappedEntities = _mapper.Map<List<GetRolesQueryDTO>>(entityObjs);

                var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, mappedEntities);
                _responseDTO.setPaginationData(paginatedObjs);
                _responseDTO.Result = paginatedObjs;
                _responseDTO.StatusEnum = StatusEnum.Success;
                _responseDTO.Message = "rolesRetrievedSuccessfully";*/
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
