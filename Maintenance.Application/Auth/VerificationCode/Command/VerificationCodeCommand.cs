using MediatR;
using Maintenance.Application.Helper;
using Maintenance.Domain.Enums;

namespace Maintenance.Application.Auth.VerificationCode.Command 
{
    public class VerificationCodeCommand : IRequest<ResponseDTO>
    {
        public long? UserId { get; set; }
        public string? Code { get; set; }
    }
}
