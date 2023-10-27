using AutoMapper;

using YTT.Gateway.Model.Models.WorkOrderModels;

namespace MES.Client.Helpers.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductionWorkOrderList, dynamic>();
    }
}

