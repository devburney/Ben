﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace TBS.PrintTest.DataAccess.Models
{
    public partial class Province
    {
        public Province()
        {
            Employee = new HashSet<Employee>();
        }

        public int ProvinceId { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceEnglishName { get; set; }
        public string ProvinceFrenchName { get; set; }
        public int ProvinceSortOrder { get; set; }
        public bool? ProvinceActiveBoolean { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}