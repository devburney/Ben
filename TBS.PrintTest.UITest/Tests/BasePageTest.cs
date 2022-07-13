using NUnit.Framework;
using POM.Drivers.Enums;
using POM.Pages;
using POM.Utils.Helpers;
using POM.Security.Authentication.Users;
using static POM.Utils.Helpers.DriverHelper;
using static TBS.PrintTest.UITest.Utils.FileUtils;
using POM.Security.Authentication.Login.TAP;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TBS.PrintTest.UITest.Tests
{
    [SetUpFixture]
    public class BasePageTest
    {
        protected string TestAssertString { get; set; }

        //MAKE SURE YOU START THE WEB APP LOCALLY AT LEAST ONCE BEFORE TESTING TO SETUP IIS EXPRESS
        // One time setup for ALL test fixtures before a fixture's tests execute
        // Don't use the [OneTimeSetUp] attribute.
        // The stable web driver is chrome. All the other ones might have specific out of scope issues
        public void FixtureSetup(DriverType driverType = DriverType.EDGE, bool headlessMode = false)
        {
            Debug(string.Empty);
            Debug($" #### TEST FIXTURE: {TestContext.CurrentContext.Test.Name} ####");
            //you can also create your own user that inherits DefaultUser and use it throughout the code.
            //But you must setup the Current user if you want to use login features
            BasePage.CurrentUser = new DefaultUser
            {
                Username = TestContext.Parameters.Get("webAppUserName"), //retrieves parameters from the .runsettings file
                FirstName = TestContext.Parameters.Get("webAppFirstName"),
                LastName = TestContext.Parameters.Get("webAppLastName"),
                Password = TestContext.Parameters.Get("webAppPassword")
            };
            StartWebDriver(driverType, headlessMode);
        }

        // One time teardown for ALL test fixtures after a fixture's tests have executed
        public void FixtureTearDown()
        {
            StopWebDriver();
        }

        // One time setup for all testcases. Must be called by each fixture's [OneTimeSetUp] method.
        public void BaseTestSetup()
        {
            // TestAssertString is used in Assert strings
            TestAssertString =
                "\nTestFixture: " + TestContext.CurrentContext.Test.ClassName + "\n" +
                "Testcase: " + TestContext.CurrentContext.Test.Name;
        }  

        // One time teardown for all testcases.
        public void BaseTestTearDown()
        {
            Debug($"\t\tResult: {TestContext.CurrentContext.Result.Outcome}");
        }


        // Generic method to validate the web elements when a page loads
        protected string ValidatePage(BasePage page, string url)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(string.Empty);
            var s = string.Empty;

            if (!BasePage.GetUrl.Equals(url))
                return $"Expected URL ({url}) Actual URL ({BasePage.GetUrl})";

            s = PageDataHelper.ValidateElementsClickableOnPageLoad(url);
            sb.Append(s);

            s = PageDataHelper.ValidateElementsDisplayedOnPageLoad(url);
            sb.Append(s);

            return s;
        }
    }
}
