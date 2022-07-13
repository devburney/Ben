using TBS.PrintTest.DataAccess.DTOs;
using TBS.PrintTest.DataAccess.Models;
using System.Collections.Generic;


namespace TBS.PrintTest.DataAccess.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        List<Employee> GetEmployees();

        Employee GetEmployee(int id);

        Employee AddOrEditEmployee(EmployeeDTO employeeDTO);

        void DeleteEmployee(int id);
    }
}
