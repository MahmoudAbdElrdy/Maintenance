using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Notifications;
using Maintenance.Application.Helpers.QueryableExtensions;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Refit;

namespace Maintenance.Application.Features.RequestsComplanit.Commands
{
    public class PostRequestComplanitCommand : IRequest<ResponseDTO>
    {

        public string? Description { get; set; }
        public long[]? CheckListsRequest { get; set; }
        public string[]? AttachmentsComplanit { get; set; }
        public string? SerialNumber { get; set; }

        public long? CategoryComplanitId { set; get; }
        //    public UserType? User { get; set; }
        class PostRequestComplanit : IRequestHandler<PostRequestComplanitCommand, ResponseDTO>
        {
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;

            private readonly IGRepository<ComplanitHistory> _ComplanitHistoryRepository;
            private readonly IGRepository<ComplanitFilter> _ComplanitFilterRepository;

            private readonly ILogger<PostRequestComplanitCommand> _logger;
            private readonly IStringLocalizer<string> _Localizer; 
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            private readonly IRoom _room;
            private readonly UserManager<User> _userManager;
            private readonly IGRepository<Notification> _NotificationRepository;
            public PostRequestComplanit(

                IGRepository<RequestComplanit> RequestComplanitRepository,
                ILogger<PostRequestComplanitCommand> logger,
                IAuditService auditService,
                IMapper mapper,
                IGRepository<ComplanitHistory> ComplanitHistoryRepository,
                IStringLocalizer<string> Localizer,
                IRoom room,
                IGRepository<Notification> NotificationRepository,
                UserManager<User> userManager,
                IGRepository<ComplanitFilter> ComplanitFilterRepository
            )
            {
                _RequestComplanitRepository = RequestComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
                _ComplanitHistoryRepository = ComplanitHistoryRepository;
                _Localizer = Localizer;
                _room = room;
                _NotificationRepository = NotificationRepository;
                _userManager = userManager;
                _ComplanitFilterRepository = ComplanitFilterRepository;
            }
            public async Task<ResponseDTO> Handle(PostRequestComplanitCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    RoomsDTO room = new RoomsDTO();
                    try
                    {
                        room = await _room.GetRoomId(request.SerialNumber);

                        if (room == null)

                        {
                            _response.Result = request.SerialNumber;

                            _response.StatusEnum = StatusEnum.Failed;

                            _response.Message = _Localizer["RoomNotFound"].ToString();

                            return _response;
                        }

                    }
                    catch (ApiException ex)
                    {
                        _response.Result = null;

                        _response.StatusEnum = StatusEnum.Failed;

                        _response.Message = _Localizer["anErrorOccurredPleaseContactSystemAdministrator"];
                        return _response;
                    }
   
                        var RequestComplanit = new RequestComplanit()
                        {
                            CreatedBy = _auditService.UserId,
                            CreatedOn = DateTime.Now,
                            State = Domain.Enums.State.NotDeleted,
                            Description = request.Description,
                            SerialNumber = request.SerialNumber,
                            Code= GenerateCodeComplaint(),
                            ComplanitStatus=ComplanitStatus.Submitted,
                            OfficeId=room.OfficeId,
                            RegionId = room.RegionId
                        };

                       foreach (var item in request.AttachmentsComplanit)
                        {
                            RequestComplanit.AttachmentsComplanit.Add(new AttachmentComplanit()
                            {
                                Path = item,
                                CreatedBy = _auditService.UserId,
                                CreatedOn = DateTime.Now,
                            });
                        }
                       foreach (var item in request.CheckListsRequest)
                        {
                            RequestComplanit.CheckListRequests.Add(new CheckListRequest()
                            {
                                CheckListComplanitId = item,
                                CreatedBy = _auditService.UserId,
                                CreatedOn = DateTime.Now,
                            });
                        }
                     

                      var ComplanitHistory = new ComplanitHistory()
                       {
                        CreatedBy = _auditService.UserId,
                        CreatedOn = DateTime.Now,
                        State = Domain.Enums.State.NotDeleted,
                        ComplanitStatus = Domain.Enums.ComplanitStatus.Submitted,

                       };

                    //////////
                    ///

                    var ComplanitFilterList = await _ComplanitFilterRepository.GetAll(x=>x.State==State.NotDeleted).ToListAsync();
                     if(room.OfficeId != null && room.OfficeId > 0)
                    {
                        ComplanitFilterList = ComplanitFilterList.Where(c => c.OfficeId.Split(',', StringSplitOptions.None).Contains(room.OfficeId.ToString())).ToList();

                    }
                    if (room.RegionId != null && room.RegionId > 0)
                    {
                        ComplanitFilterList = ComplanitFilterList.Where(c => c.RegionId.Split(',', StringSplitOptions.None).Contains(room.RegionId.ToString())).ToList();

                    }
                    if (request.CategoryComplanitId != null && request.CategoryComplanitId > 0)
                    {
                        ComplanitFilterList = ComplanitFilterList.Where(c => c.CategoryComplanitId.Split(',', StringSplitOptions.None).Contains(request.CategoryComplanitId.ToString())).ToList();

                    }

                    var usersIds = ComplanitFilterList.Select(c => c.CreatedBy).ToList();

                    var users = await _userManager.Users.Where(x => x.State == State.NotDeleted)

                        .WhereIf(usersIds.Count>0, x=> (x.UserType == UserType.Owner || x.UserType == UserType.Consultant ||( x.UserType == UserType.Technician &&ComplanitFilterList.Select(f => f.CreatedBy).Contains(x.Id))))
                        .WhereIf(usersIds.Count==0, x=> (x.UserType == UserType.Owner || x.UserType == UserType.Consultant ))
                       
                        .ToListAsync();
                    foreach (var item in users)

                    {
                        var notfication = new Notification()
                        {
                            CreatedBy = _auditService.UserId,

                            CreatedOn = DateTime.Now,

                            State = Domain.Enums.State.NotDeleted,

                            From = _auditService.UserId,

                            NotificationState = NotificationState.New,

                            SubjectAr = RequestComplanit.Code,

                            SubjectEn = RequestComplanit.Code,

                            BodyAr = _Localizer["ResponsesToComplaint"],

                            BodyEn = _Localizer["ResponsesToComplaint","en"],

                            To = item.Id,

                            Read = false,

                            Type = NotificationType.Message
                        };


                        
                        var notificationDto = new NotificationDto()
                        {
                            Title = RequestComplanit.Code,
                            Body = _Localizer["ResponsesToComplaint"]
                        };

                        await NotificationHelper.FCMNotify(notificationDto, item.Token);
                         ComplanitHistory.Notifications.Add(notfication);
                    }
                 
                    RequestComplanit.ComplanitHistory.Add(ComplanitHistory);
                      
                    await _RequestComplanitRepository.AddAsync(RequestComplanit);

                    _RequestComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = _Localizer["AddedSuccessfully"] ;
                    _response.Result = null;
                    return _response;
                }
                catch (Exception ex)
                {
                   
               var folderName = Path.Combine("wwwroot/Uploads/Complanits");

               foreach (var fileRemove in request.AttachmentsComplanit)
                 {
                                var file = System.IO.Path.Combine(folderName, fileRemove);
                                try
                                {
                                    System.IO.File.Delete(file);
                                }
                                catch { }
                  }
                    _response.StatusEnum = StatusEnum.Exception;
                    _response.Result = null;
                    _response.Message = ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _logger.LogError(ex, ex.Message, ex != null && ex.InnerException != null ? ex.InnerException.Message : "");

                    return _response;
                }
            }
            public  string GenerateCodeComplaint()
            {
                var characters = "0123456789";
                var charsArr = new char[10];
                var random = new Random();
                for (int i = 0; i < charsArr.Length; i++)
                {
                    charsArr[i] = characters[random.Next(characters.Length)];
                }
                var segmentString = new String(charsArr);
                return segmentString;
            }
        }
    }
}
