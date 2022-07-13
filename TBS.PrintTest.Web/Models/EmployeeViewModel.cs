using TBS.PrintTest.Web.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TBS.PrintTest.Web.Models
{
    public class EmployeeViewModel : BaseViewModel
    {
        // lookups
        public List<SelectListItem> Provinces { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TermTypes { get; set; } = new List<SelectListItem>();

        // -------
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "EmployeeFirstName", ResourceType = typeof(Localization))]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "EmployeeLastName", ResourceType = typeof(Localization))]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "EmployeeTermType", ResourceType = typeof(Localization))]
        public int TermTypeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "EmployeeStartDate", ResourceType = typeof(Localization))]
        public DateTime StartDate { get; set; } = DateTime.Now.Date;

        [Required]
        [Display(Name = "EmployeeProvince", ResourceType = typeof(Localization))]
        public int ProvinceId { get; set; }

        [MaxLength(256)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "EmployeeJobDescription", ResourceType = typeof(Localization))]
        public string JobDescription { get; set; }

    }
}
