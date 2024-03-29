using System;
using AutoMapper;
using MES.Administration.Models.ProductModels;
using MES.Administration.Models.WorkstationModels;
using Shared.Entity.Models;

namespace MES.Administration.Helpers.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<EndProductModel, dynamic>();
        CreateMap<WorkstationModel, dynamic>();

    }
}

