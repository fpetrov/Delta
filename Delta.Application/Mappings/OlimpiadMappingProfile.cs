using AutoMapper;
using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;

namespace Delta.Application.Mappings;

public class OlimpiadMappingProfile : Profile
{
    public OlimpiadMappingProfile()
    {
        CreateMap<CreateOlimpiadRequest, Olimpiad>();
    }
}