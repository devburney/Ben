using TBS.PrintTest.Web.Resources;
using System.ComponentModel.DataAnnotations;

namespace TBS.PrintTest.Web.Models
{
    public class EmployeeItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "EmployeeFirstName", ResourceType = typeof(Localization))]
        public string FirstName { get; set; }

        [Display(Name = "EmployeeLastName", ResourceType = typeof(Localization))]
        public string LastName { get; set; }

        [Display(Name = "EmployeeJobDescription", ResourceType = typeof(Localization))]
        public string JobDescription { get; set; }
    }
}
