using AutoMapper;
using NotificationApplication.DTOs;
using NotificationDomain.Entities;

namespace NotificationApplication.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>().ReverseMap();
        CreateMap<CreateNotificationDto, Notification>();
        CreateMap<UpdateNotificationDto, Notification>();
    }
}