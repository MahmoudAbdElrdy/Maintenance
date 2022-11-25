using MediatR;
using Maintenance.Application.Helper;
using Maintenance.Domain.Enums;

namespace Maintenance.Application.Features.RequestsComplanit.Command 
{
    public class VerificationCodeComplanit : IRequest<ResponseDTO>
    {
        public long? RequestComplanitId { get; set; }
        public string? CodeSms { get; set; } 
     
    }
}
