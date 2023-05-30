using AutoMapper;
using LBS.Shared.Entity.Models;
using MES.Models;
using MES.Models.ProductModels.EndProductModels;
using MES.Models.ProductModels.RawProductModels;
using MES.Models.ProductModels.SemiProductModels;
using MES.Models.ProductPopupModels;

namespace MES.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
			CreateMap<EndProduct, EndProductModel>().ReverseMap();
			CreateMap<RawProduct, RawProductModel>().ReverseMap();
			CreateMap<SemiProduct, SemiProductModel>().ReverseMap();
			CreateMap<ProductWarehouseParameter, WarehouseParameterModel>().ReverseMap();
            CreateMap<ProductMeasure, ProductMeasureModel>().ReverseMap();

        }
    }
}
