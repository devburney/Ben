using TBS.PrintTest.Business.ErrorHandling;
using TBS.PrintTest.Business.Models;
using System.Collections.Generic;

namespace TBS.PrintTest.Business.Services.Interfaces
{
    public interface IEmployeeService
    {
        List<Employee> GetEmployees();

        BusinessResult<Employee> GetEmployee(int id);

        BusinessResult<Employee> AddOrEditEmployee(Employee employee);

        void DeleteEmployee(int id);
    }
}
