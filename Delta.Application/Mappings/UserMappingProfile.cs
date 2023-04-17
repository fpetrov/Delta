using AutoMapper;
using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;

namespace Delta.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserRequest, User>();
    }
}