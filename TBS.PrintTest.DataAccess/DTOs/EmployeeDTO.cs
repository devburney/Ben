using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.DataAccess.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TermTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public int ProvinceId { get; set; }
        public string JobDescription { get; set; }
    }
}
