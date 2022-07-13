using POM.Utils.Entities;
using POM.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.UITest.Pages
{
    public class ContactsPage : TemplateBasePage
    {
        public ContactsPage()
        {
            base.Contacts.Click();
        }

        public Element H1_ContactsPageTitle => GetElement("h1", SelectorType.TagName);
        public Element H2_ContactsPageSubTitle => GetElement("/html/body/main/h2", SelectorType.Xpath);
        public Element Email_ContactPageLink => GetElement("email", SelectorType.Name);
        public Element Phone_TollFreeNumber => GetElement("tollfree", SelectorType.Name);
        public Element Fax_Teletypewriter => GetElement("teletypewriter", SelectorType.Name);
    }
}
