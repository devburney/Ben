using AutoMapper;
using TBS.PrintTest.Business.Models;
using TBS.PrintTest.Web.Models;

namespace TBS.PrintTest.Web.MappingProfiles
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            // Business => ViewModel
            // ---------------------
            CreateMap<Employee, EmployeeViewModel>();
            CreateMap<Employee, EmployeeItemViewModel>();


            // ViewModel => Business
            // ---------------------
            CreateMap<EmployeeViewModel, Employee>();
        }
    }
}
