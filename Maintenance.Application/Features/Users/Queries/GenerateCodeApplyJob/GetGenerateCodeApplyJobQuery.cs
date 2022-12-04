using Maintenance.Application.Helper;
using MediatR;

namespace Maintenance.Application.Features.Users.Queries.GenerateCodeApplyJob
{
    public class GetGenerateCodeApplyJobQuery:IRequest<ResponseDTO>
    {
        public string? MobileNumber { get; set; }
    }
}
