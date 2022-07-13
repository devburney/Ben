using AutoMapper;
using Entity = TBS.PrintTest.DataAccess.Models;
using TBS.PrintTest.DataAccess.DTOs;

namespace TBS.PrintTest.Business.MappingProfiles
{
    public class CodelookupMappingProfile : Profile
    {
        public CodelookupMappingProfile()
        {
            // Data => Business
            // ---------------------

            // Province
            CreateMap<Entity.Province, Models.Codelookup>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProvinceId))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ProvinceCode))
                .ForMember(dest => dest.EnglishText, opt => opt.MapFrom(src => src.ProvinceEnglishName))
                .ForMember(dest => dest.FrenchText, opt => opt.MapFrom(src => src.ProvinceFrenchName))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.ProvinceActiveBoolean))
                ;

            // Term Type
            CreateMap<Entity.EmployeeTermType, Models.Codelookup>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EmployeeTermTypeId))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.EmployeeTermTypeCode))
                .ForMember(dest => dest.EnglishText, opt => opt.MapFrom(src => src.EmployeeTermTypeEnglishName))
                .ForMember(dest => dest.FrenchText, opt => opt.MapFrom(src => src.EmployeeTermTypeFrenchName))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.EmployeeTermTypeActiveBoolean))
                ;
        }
    }
}
