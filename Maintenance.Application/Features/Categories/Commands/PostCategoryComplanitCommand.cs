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
    public class PostCategoryComplanitCommand : IRequest<ResponseDTO>
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        class PostCategoryComplanit : IRequestHandler<PostCategoryComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<CategoryComplanit> _CategoryComplanitRepository;
            private readonly ILogger<PostCategoryComplanitCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public PostCategoryComplanit(

                IGRepository<CategoryComplanit> CategoryComplanitRepository,
                ILogger<PostCategoryComplanitCommand> logger,
                IAuditService auditService,
                IMapper mapper
            )
            {
                _CategoryComplanitRepository = CategoryComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;

            }
            public async Task<ResponseDTO> Handle(PostCategoryComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {

                    var CategoryComplanit = new CategoryComplanit()
                    {
                        CreatedBy = _auditService.UserId,
                        CreatedOn = DateTime.Now,
                        State = Domain.Enums.State.NotDeleted,
                        DescriptionAr = request.DescriptionAr,
                        DescriptionEn= request.DescriptionEn,
                        NameAr = request.NameAr,
                        NameEn = request.NameEn
                    };

                    await _CategoryComplanitRepository.AddAsync(CategoryComplanit);
                    _CategoryComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "CategoryComplanitSavedSuccessfully";
                    _response.Result = _mapper.Map<CategoryComplanitDto>(CategoryComplanit);
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
