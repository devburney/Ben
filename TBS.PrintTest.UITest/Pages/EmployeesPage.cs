using POM.Utils.Entities;
using POM.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.UITest.Pages
{
    public class EmployeesPage : TemplateBasePage
    {
        public EmployeesPage() : base()
        {
            base.EmployeesPage.Click();
        }

        #region EmplyeeListing
        public Element Table_Employees => GetElement("employees", SelectorType.Id);
        public Element Row_Employee_1 => Table_Employees.FindElement("//tbody/tr[1]", SelectorType.Xpath);
        public Element Row_Employee_2 => Table_Employees.FindElement("//tbody/tr[2]", SelectorType.Xpath);
        public Element Button_ViewEmployee_1 => Row_Employee_1.FindElement("View", SelectorType.LinkText);
        public Element Button_ViewEmployee_2 => Row_Employee_2.FindElement("View", SelectorType.LinkText);
        public Element Button_AddEmployee => GetElement("Add employee", SelectorType.LinkText);
        public Element Button_EditEmployee_1 => Row_Employee_1.FindElement("Edit", SelectorType.LinkText);
        public Element Button_EditEmployee_2 => Row_Employee_2.FindElement("Edit", SelectorType.LinkText);
        public Element Button_DeleteEmployee_1 => Table_Employees.FindElement("//tbody/tr[1]/td[5]/button", SelectorType.Xpath);
        public Element Button_DeleteEmployee_2 => Table_Employees.FindElement("//tbody/tr[2]/td[5]/button", SelectorType.Xpath);
        public Element Button_BackToList => GetElement("Back", SelectorType.PartialLinkText);
        #endregion

        #region Add/Edit Employee
        public Element Form_EditEmployee => GetElement("//*[@id='wb-auto-2']/form", SelectorType.Xpath);
        public Element TxtBox_FirstName_AddEdit => Form_EditEmployee.FindElement("FirstName", SelectorType.Id);
        public Element TxtBox_LastName_AddEdit => Form_EditEmployee.FindElement("LastName", SelectorType.Id);
        public Element Date_StartDate_AddEdit => Form_EditEmployee.FindElement("StartDate", SelectorType.Id);
        public Element Select_Province_AddEdit => Form_EditEmployee.FindElement("ProvinceId", SelectorType.Id);
        public Element Radio_Employment_FullTime_AddEdit => Form_EditEmployee.FindElement("TermTypeId-1", SelectorType.Id); 
        public Element Radio_Employment_PartTime_AddEdit => Form_EditEmployee.FindElement("TermTypeId-2", SelectorType.Id);
        public Element TxtArea_JobDescription_AddEdit => Form_EditEmployee.FindElement("JobDescription", SelectorType.Id);
        public Element Button_Save_AddEdit => Form_EditEmployee.FindElement("save", SelectorType.Id);
        #endregion

        #region Delete Modal
        public Element Modal_ConfirmDelete => GetElement("Employee_Index_DeleteModal", SelectorType.Id);
        public Element Button_Yes_ConfirmDelete => GetElement("Employee_Index_DeleteModal-Confirm", SelectorType.Id);
        public Element Button_Cancel_ConfirmDelete => GetElement("Employee_Index_DeleteModal-Dismiss", SelectorType.Id);
        #endregion
    }
}