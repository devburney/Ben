using GoC.WebTemplate.Components.Core.Services;
using TBS.PrintTest.Web.Infrastructure.Services.Interfaces;
using TBS.PrintTest.Web.Models;
using TBS.PrintTest.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;



namespace TBS.PrintTest.Web.Controllers
{
    /// <summary>
    /// Home controller
    /// </summary>
    public class HomeController : BaseController
    {

        /// <summary>
        /// Constructor for home controller
        /// </summary>
        /// <param name="modelAccessor">GoC Template model accessor</param>
        /// <param name="sitemapService">Sitemap service</param>
        /// <param name="logger">logger</param>
        public HomeController(ModelAccessor modelAccessor, ISitemapService sitemapService, ILogger<HomeController> logger) : base(modelAccessor, sitemapService, logger)
        {

        }

        public IActionResult Index()
        {
            // set the page title and page description.  page title is used in header and also displayed in the browswer tab.
            // descrtioption is displayed right below the header and is not required.
            PageTitle = Localization.Home_Title;
            PageDescription = Localization.Home_PageDescription;

            return View(new HomeViewModel());
        }

        /// <summary>
        /// Error page for unhandled errors.  This is configured in the startup class and it's where users are directected
        /// when an untrapped error occurs. (app.UseExceptionHandler)
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // set error page title
            PageTitle = Localization.GenericUntrappedErrorTitle;
            PageDescription = string.Empty;

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
