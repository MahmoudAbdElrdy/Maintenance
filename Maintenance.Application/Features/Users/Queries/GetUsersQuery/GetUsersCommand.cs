using AuthDomain.Entities.Auth;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Maintenance.Application.Features.Users.Queries.GetUsersQuery
{
    public class GetUsersQuery : IRequest<ResponseDTO>
    {
        public UserType UserType { get; set; }
    }
    class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ResponseDTO>
    {
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _responseDTO;
        private readonly IAuditService _auditService;
        private readonly IStringLocalizer<GetUsersQuery> _stringLocalizer;
        public GetUsersQueryHandler(UserManager<User> userManager, IStringLocalizer<GetUsersQuery> stringLocalizer, IAuditService auditService)
        {
            _userManager = userManager;
            _responseDTO = new ResponseDTO();
            _auditService = auditService;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ResponseDTO> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var user = new User();
            try
            {

                var models = await _userManager.Users.Where(x => x.UserType == request.UserType && x.State != State.Deleted).ToListAsync();

                var users = models.Select(x => new UsersDTO()
                {
                    FullName = x.FullName,
                    IdentityNumber = x.IdentityNumber,
                    OfficeId = x.OfficeId,
                    PhoneNumber = x.PhoneNumber,
                    RegionId = x.RegionId,
                }).ToList();

                _responseDTO.Result = users;
            }
            catch (Exception ex)
            {
              
                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Exception;
                _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];

            }

            _responseDTO.Message = "usersRetrievedSuccessfully";

            _responseDTO.StatusEnum = StatusEnum.Success;

            return _responseDTO;

        }

    }
}

