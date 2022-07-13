using NUnit.Framework;
using POM.Pages;
using POM.Utils.Entities;
using POM.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.UITest.Pages
{
    public class HomePage : TemplateBasePage
    {
        public Element H1_Title_Home => GetElement("h1", SelectorType.TagName);
        public Element Paragraph_H1_TitleDescription_Home => GetElement("pagetag", SelectorType.ClassName);
        public Element H2_Title_Home => GetElement("h2", SelectorType.TagName);

        public HomePage()
        {
            Url = TestContext.Parameters.Get("Url");

        }
    }
}