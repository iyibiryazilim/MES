using AutoMapper;
using LBS.Shared.Entity.Models;
using MES.Models;

namespace MES.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
			CreateMap<EndProduct, EndProductModel>().ReverseMap();
			CreateMap<RawProduct, RawProductModel>().ReverseMap();
			CreateMap<SemiProduct, SemiProductModel>().ReverseMap();
			CreateMap<ProductWarehouseParameter, ProductWarehouseParameterModel>().ReverseMap();
            CreateMap<ProductMeasure, ProductMeasureModel>().ReverseMap();

        }
    }
}
