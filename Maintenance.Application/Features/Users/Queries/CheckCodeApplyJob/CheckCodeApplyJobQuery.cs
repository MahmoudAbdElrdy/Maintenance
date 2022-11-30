using MediatR;

namespace Maintenance.Application.Features.Users.Queries.CheckCodeApplyJob
{
    public class CheckCodeApplyJobQuery:IRequest<Helper.ResponseDTO>
    {
        public string? NationalId { get; set; }
        public string? Code { get; set; }

    }
}
