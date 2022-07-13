using TBS.PrintTest.UITest.Pages;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using POM.Pages;
using POM.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBS.PrintTest.UITest.Tests
{
    [TestFixture]
    public class ContactsPageTest : BasePageTest
    {
        private HomePage HomePage;
        private ContactsPage ContactsPage;

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
            ContactsPage = PageFactory.GetPage<ContactsPage>();
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
            ContactsPage.ValidatePageUrlContains("contacts");
            Assert.IsTrue(ContactsPage.H1_ContactsPageTitle.Text.Equals("Contacts"));
            Assert.IsTrue(ContactsPage.H2_ContactsPageSubTitle.Text.Trim().Equals("General and technical enquiries"));
            Assert.IsTrue(ContactsPage.Email_ContactPageLink.Text.Equals("someone@tbs-sct.gc.ca"));
            Assert.IsTrue(ContactsPage.Phone_TollFreeNumber.Text.Equals("1-877-555-5556"));
            Assert.IsTrue(ContactsPage.Fax_Teletypewriter.Text.Equals("613-555-5555"));
        }
    }
}
