using POM.Pages;
using POM.Utils.Entities;
using POM.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.UITest.Pages
{
    public class TemplateBasePage : BasePage
    {
        public Element NavBar => GetElement("nvbar", SelectorType.ClassName);
        public Element HomePage => NavBar.FindElement("Home", SelectorType.LinkText);
        public Element EmployeesPage => NavBar.FindElement("Employees", SelectorType.LinkText);
        public Element Contacts => NavBar.FindElement("Contacts", SelectorType.LinkText);
        public Element Google => NavBar.FindElement("Google", SelectorType.LinkText);

        public TemplateBasePage() : base()
        {

        }
    }
}
