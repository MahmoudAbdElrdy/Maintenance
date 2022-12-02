using AuthDomain.Entities.Auth;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Maintenance.Application.Features.Users.Queries.GetUserByIdQuery
{
    public class GetUserByIdQuery : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
    }
    class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ResponseDTO>
    {
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _responseDTO;
        private readonly IStringLocalizer<GetUserByIdQuery> _stringLocalizer;
        public GetUserByIdQueryHandler(UserManager<User> userManager, IStringLocalizer<GetUserByIdQuery> stringLocalizer)
        {
            _userManager = userManager;
            _responseDTO = new ResponseDTO();
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ResponseDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = new User();
            try
            {

                var models = await _userManager.Users.Where(x => x.Id == request.Id && x.State != State.Deleted).ToListAsync();

                var users = models.Select(x => new UsersDTO()
                {
                    FullName = x.FullName,
                    IdentityNumber = x.IdentityNumber,
                    OfficeId = x.OfficeId,
                    PhoneNumber = x.PhoneNumber,
                    RegionId = x.RegionId,
                }).FirstOrDefault();

                _responseDTO.Result = users;
            }
            catch (Exception ex)
            {
              
                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Exception;
                _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];

            }

            _responseDTO.Message = "userRetrievedSuccessfully";

            _responseDTO.StatusEnum = StatusEnum.Success;

            return _responseDTO;

        }

    }
}

