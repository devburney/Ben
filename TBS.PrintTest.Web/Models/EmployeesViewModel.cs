using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBS.PrintTest.Web.Models
{
    public class EmployeesViewModel
    {

        public bool IsAdmin { get; set; }

        public List<EmployeeItemViewModel> Employees { get; set; } = new List<EmployeeItemViewModel>();
    }
}
