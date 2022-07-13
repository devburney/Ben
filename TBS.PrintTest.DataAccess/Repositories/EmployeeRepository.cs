using TBS.PrintTest.DataAccess.DTOs;
using TBS.PrintTest.DataAccess.Models;
using TBS.PrintTest.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TBS.PrintTest.DataAccess.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMTDTemplateContext _context;

        public EmployeeRepository(IMTDTemplateContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Employee> GetEmployees()
        {
            return _context.Employee.AsNoTracking().Where(x => x.EmployeeActiveBoolean == true).ToList();
        }

        public Employee GetEmployee(int id)
        {
            return _context.Employee.AsNoTracking().FirstOrDefault(x => x.EmployeeId == id && x.EmployeeActiveBoolean == true);
        }

        public Employee AddOrEditEmployee(EmployeeDTO employeeDTO)
        {
            Models.Employee employeeEntity;

            // if we don't have an id, it's an add, or else it's an update.
            if (employeeDTO.Id == 0)
            {
                employeeEntity = new Models.Employee()
                {
                    EmployeeFirstName = employeeDTO.FirstName,
                    EmployeeLastName = employeeDTO.LastName,
                    EmployeeTermTypeId = employeeDTO.TermTypeId,
                    EmployeeStartDate = employeeDTO.StartDate,
                    EmployeeProvinceId = employeeDTO.ProvinceId,
                    EmployeeJobDescription = employeeDTO.JobDescription
                };

                _context.Employee.Add(employeeEntity);
            }
            else
            {
                // We should always get an employee since we wouldn't make it here if we didn't have an a valid id.
                // if there's a concern, business layer should handle what happens if there's no record for id being updated. This method
                // will simply return a null employee entity.
                employeeEntity = _context.Employee.FirstOrDefault(x => x.EmployeeId == employeeDTO.Id);

                if (employeeEntity != null)
                {
                    employeeEntity.EmployeeFirstName = employeeDTO.FirstName;
                    employeeEntity.EmployeeLastName = employeeDTO.LastName;
                    employeeEntity.EmployeeTermTypeId = employeeDTO.TermTypeId;
                    employeeEntity.EmployeeStartDate = employeeDTO.StartDate;
                    employeeEntity.EmployeeProvinceId = employeeDTO.ProvinceId;
                    employeeEntity.EmployeeJobDescription = employeeDTO.JobDescription;
                }

                // NOTE: if you chose not to track changes, you have to set the entity state.
                // currenty, this project is configured to track changes (defined in startup.cs).
                //_context.Entry(employeeEntity).State = EntityState.Modified;
            }

            _context.SaveChanges();

            return employeeEntity;
        }

        public void DeleteEmployee(int id)
        {

            // logically delete employee by setting active flag to false.
            var employeeEntity = _context.Employee.FirstOrDefault(x => x.EmployeeId == id);

            if (employeeEntity != null)
            {
                employeeEntity.EmployeeActiveBoolean = false;

                _context.SaveChanges();
            }
        }
    }
}
