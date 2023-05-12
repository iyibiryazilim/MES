using AutoMapper;
using LBS.Shared.Entity.Models;
using MES.Models;

namespace MES.Profiles
{
    public class RawProductProfile : Profile
    {
        public RawProductProfile()
        {
            CreateMap<RawProduct, RawProductModel>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Code, o => o.MapFrom(s => s.Code))
                .ForMember(d => d.LockTrackingType, o => o.MapFrom(s => s.LockTrackingType))
                .ForMember(d => d.ProducerCode, o => o.MapFrom(s => s.ProducerCode))
                .ForMember(d => d.ReferenceId, o => o.MapFrom(s => s.ReferenceId))
                .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
                .ForMember(d => d.CardType, o => o.MapFrom(s => s.CardType))
                .ForMember(d => d.Vat, o => o.MapFrom(s => s.Vat))
                .ForMember(d => d.Unitset, o => o.MapFrom(s => s.Unitset))
                .ForMember(d => d.SpeCode, o => o.MapFrom(s => s.SpeCode))
                .ForMember(d => d.TrackingType, o => o.MapFrom(s => s.TrackingType));


        }
    }
}
