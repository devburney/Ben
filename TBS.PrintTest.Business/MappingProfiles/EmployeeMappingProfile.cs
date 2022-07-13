using AutoMapper;
using Entity = TBS.PrintTest.DataAccess.Models;
using TBS.PrintTest.DataAccess.DTOs;

namespace TBS.PrintTest.Business.MappingProfiles
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            // Data => Business
            // ---------------------
            CreateMap<Entity.Employee, Models.Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.EmployeeFirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.EmployeeLastName))
                .ForMember(dest => dest.TermTypeId, opt => opt.MapFrom(src => src.EmployeeTermTypeId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.EmployeeStartDate))
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.EmployeeProvinceId))
                .ForMember(dest => dest.JobDescription, opt => opt.MapFrom(src => src.EmployeeJobDescription))
                ;

            // Business => Data (DTOs)
            CreateMap<Models.Employee, EmployeeDTO>();

        }
    }
}
