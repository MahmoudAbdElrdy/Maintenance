using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.Categories.Commands
{
    public class PutCategoryComplanitCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        class PutCategoryComplanit : IRequestHandler<PutCategoryComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<CategoryComplanit> _CategoryComplanitRepository;
            private readonly ILogger<PutCategoryComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PutCategoryComplanit(

                IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<PutCategoryComplanitCommand> logger,
                IAuditService auditService,
                IMapper  mapper
            )
            {
                _CategoryComplanitRepository = CategoryComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
            }
            public async Task<ResponseDTO> Handle(PutCategoryComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var CategoryComplanit = new CategoryComplanit()
                    {
                        UpdatedBy = _auditService.UserId,
                        UpdatedOn = DateTime.Now,
                        DescriptionAr = request.DescriptionAr,
                        DescriptionEn = request.DescriptionEn,
                        NameAr = request.NameAr,
                        NameEn = request.NameEn,
                        Id=request.Id
                    };

                     _CategoryComplanitRepository.Update(CategoryComplanit);
                    _CategoryComplanitRepository.Save();
                    _response.Result = _mapper.Map<CategoryComplanitDto>(CategoryComplanit);
                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CategoryComplanitUpdatedSuccessfully";

                    return _response;
                }
                catch (Exception ex)
                {
                    _response.StatusEnum = StatusEnum.Exception;
                    _response.Result = null;
                    _response.Message = ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _logger.LogError(ex, ex.Message, ex != null && ex.InnerException != null ? ex.InnerException.Message : "");

                    return _response;
                }
            }

        }
    }
}
