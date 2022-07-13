using TBS.PrintTest.UITest.Pages;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using POM.Pages;
using POM.Security.Authentication.Users;
using POM.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.UITest.Tests
{
    [TestFixture]
    public class EmployeesPageTest : BasePageTest
    {
        private HomePage HomePage;
        private EmployeesPage EmployeesPage;

        [OneTimeSetUp] // One time setup for this test fixture before any tests execute
        public void FixtureSetup()
        {
            base.FixtureSetup();

        }

        [OneTimeTearDown] // One time teardown for this test fixture after all tests have executed
        public new void FixtureTearDown()
        {
            base.FixtureTearDown();
        }

        [SetUp] // One time setup before each test execution
        public void TestSetup()
        {
            BaseTestSetup();
            HomePage = new HomePage();
            HomePage.GoTo();
            EmployeesPage = PageFactory.GetPage<EmployeesPage>();
        }


        [TearDown] // One time teardown after each test execution
        public void TestTearDown()
        {
            //if the test failed, take a screenshot before closing the driver
            //if the test fails, there will also be a recording of the test. This is specified on the .runsettings file.
            if (TestContext.CurrentContext.Result.Outcome != ResultState.Success)
            {
                BasePage.TakeScreenshot($"{TestContext.CurrentContext.Test.FullName}.png");
            }
            BaseTestTearDown();
        }

        [Retry(1)]
        [TestCase(TestName = "View an employee"), Order(1)]
        public void ValidateViewEmployeePage()
        {
            EmployeesPage.Button_ViewEmployee_1.Click();
            EmployeesPage.ValidatePageUrlContains("Details");
            EmployeesPage.Button_BackToList.Click();
        }

        [Retry(1)]
        [TestCase(TestName = "Add an employee with Error (First Name starts with B)"), Order(2)]
        public void ValidateAddEmployeePageWithError()
        {
            EmployeesPage.Button_AddEmployee.Click();
            EmployeesPage.ValidatePageUrlContains("Add");

            //Send Keys means to enter text in the element
            EmployeesPage.TxtBox_FirstName_AddEdit.SendKeys("Bob");
            EmployeesPage.TxtBox_LastName_AddEdit.SendKeys(BasePage.CurrentUser.LastName);
            EmployeesPage.Select_Province_AddEdit.SelectOptionByValue("1");
            EmployeesPage.Radio_Employment_FullTime_AddEdit.Click();
            EmployeesPage.TxtArea_JobDescription_AddEdit.SendKeys(".NET Developer");
            EmployeesPage.Button_Save_AddEdit.Click();
            EmployeesPage.ValidateElementIsDisplayed("foundation-validation-errors", SelectorType.Id);
            EmployeesPage.Button_BackToList.Click();
        }

        [Retry(1)]
        [TestCase(TestName = "Add an employee with no Error"), Order(3)]
        public void ValidateAddEmployeePage()
        {
            EmployeesPage.Button_AddEmployee.Click();
            EmployeesPage.ValidatePageUrlContains("Add");

            //Send Keys means to enter text in the element
            EmployeesPage.TxtBox_FirstName_AddEdit.SendKeys(BasePage.CurrentUser.FirstName);
            EmployeesPage.TxtBox_LastName_AddEdit.SendKeys(BasePage.CurrentUser.LastName);
            EmployeesPage.Select_Province_AddEdit.SelectOptionByValue("1");
            EmployeesPage.Radio_Employment_FullTime_AddEdit.Click();
            EmployeesPage.TxtArea_JobDescription_AddEdit.SendKeys(".NET Developer");
            EmployeesPage.Button_Save_AddEdit.Click();
            EmployeesPage.ValidateElementIsDisplayed("foundation-validation-success", SelectorType.Id);
            EmployeesPage.Button_BackToList.Click();
        }

        [Retry(1)]
        [TestCase(TestName = "edit an employee"), Order(4)]
        public void ValidateEditEmployeePage()
        {
            EmployeesPage.Button_EditEmployee_2.Click();
            EmployeesPage.ValidatePageUrlContains("Edit");
            EmployeesPage.Radio_Employment_PartTime_AddEdit.Click();
            EmployeesPage.TxtArea_JobDescription_AddEdit.Clear();
            EmployeesPage.TxtArea_JobDescription_AddEdit.SendKeys("Azure Devops Consultant");
            EmployeesPage.Button_Save_AddEdit.Click();
            EmployeesPage.ValidateElementIsDisplayed("foundation-validation-success", SelectorType.Id);
            EmployeesPage.Button_BackToList.Click();
        }

        [Retry(1)]
        [TestCase(TestName = "delete an employee"), Order(5)]
        public void ValidateDeleteEmployeePage()
        {
            EmployeesPage.Button_DeleteEmployee_2.Click();
            EmployeesPage.SwitchToNewModal();
            EmployeesPage.Button_Yes_ConfirmDelete.Click();
        }
    }
}
