using AutoMapper;
using TBS.PrintTest.Business.ErrorHandling;
using TBS.PrintTest.Business.Models;
using TBS.PrintTest.Business.Resources;
using TBS.PrintTest.Business.Services.Interfaces;
using TBS.PrintTest.DataAccess.DTOs;
using TBS.PrintTest.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public List<Employee> GetEmployees()
        {
            // call repo method to get all employees and return mapped list of employee business modelss.
            return _mapper.Map<List<Employee>>(_employeeRepository.GetEmployees());
        }


        public BusinessResult<Employee> GetEmployee(int id)
        {
            var result = new BusinessResult<Employee>();
            // get employee based on id
            var employeeEntity = _employeeRepository.GetEmployee(id);

            // if employee is null, return specific error message stating that no record was found.
            if (employeeEntity == null)
            {
                result.AddError(Localization.BusinessRule_NoRecordFound, ErrorType.RecordNotFound);
            }

            // return mapped employee business model.
            result.Value = _mapper.Map<Employee>(_employeeRepository.GetEmployee(id));

            return result;
        }

        public BusinessResult<Employee> AddOrEditEmployee(Employee employee)
        {
            // validate employee
            var result = ValidateEmployee(employee);

            // if result has one or more errors, return the result.
            if (!result.Success)
            {
                result.Value = employee;
                return result;
            }

            // map business object to DTO
            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);

            // call repo method to add or update employee. Method will return new employee entity.            
            var employeeEntity = _employeeRepository.AddOrEditEmployee(employeeDTO);

            // map entity to business model and send it back.

            result.Value = _mapper.Map<Employee>(employeeEntity);

            return result;

        }

        public void DeleteEmployee(int id)
        {
            // call repo method to delete employee
            _employeeRepository.DeleteEmployee(id);
        }

        #region Business Rules

        private BusinessResult<Employee> ValidateEmployee(Employee employee)
        {
            // NOTE: It is recommended that you also add server side business rules for the same client side rules
            // implemented on the presentation layer.  For example, if First Name is required, catch it on the client 
            // and here.

            var result = new BusinessResult<Employee>();

            // client side validation rules replicated server side.
            // --------------------

            // Note:  You can validate all errors at once if you want to display multiple errors but sometimes it doesn't make sense/
            // So you can simply return the result in any failed rule where applicabe.  i.e. In this case, returning result
            // if first name is missing since we know we won't be able to validate the business rule that checks for First names starts with B.

            // first name required
            if (string.IsNullOrEmpty(employee.FirstName.Trim()))
            {
                result.AddError(Localization.BusinessRule_FirstNameRequired, ErrorType.BusinessLogic);
                return result;
            }

            // last name required
            if (string.IsNullOrEmpty(employee.LastName.Trim()))
            {
                result.AddError(Localization.BusinessRule_LastNameRequired, ErrorType.BusinessLogic);
            }

            // province required
            if (employee.ProvinceId == 0)
            {
                result.AddError(Localization.BusinessRule_ProvinceRequired, ErrorType.BusinessLogic);
            }

            // employee term type required
            if (employee.TermTypeId == 0)
            {
                result.AddError(Localization.BusinessRule_TermTypeRequired, ErrorType.BusinessLogic);
            }

            // do some business logic validation.
            // ----------------------

            // example of a business rule.  in this case, we don't allow first names that start with 'B'.  Couldn't think of a better example.
            if (employee.FirstName.Trim().Substring(0,1).ToUpper() == "B")
            {
                result.AddError(Localization.BusinessRule_NoFirstNameStartingWithB, ErrorType.BusinessLogic);
            }

            return result;
        }

        #endregion
    }
}
