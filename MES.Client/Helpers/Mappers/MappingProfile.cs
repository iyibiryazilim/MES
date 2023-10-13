using AutoMapper;
using MES.Client.ListModels;

namespace MES.Client.Helpers.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WorkOrderList, dynamic>();
    }
}

