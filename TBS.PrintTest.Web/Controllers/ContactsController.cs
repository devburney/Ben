using GoC.WebTemplate.Components.Core.Services;
using TBS.PrintTest.Web.Infrastructure.Services.Interfaces;
using TBS.PrintTest.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace TBS.PrintTest.Web.Controllers
{
    /// <summary>
    /// Contacts controller
    /// </summary>
    public class ContactsController : BaseController
    {
        /// <summary>
        /// Constructor for contacts controller
        /// </summary>
        /// <param name="modelAccessor">GoC Template model accessor</param>
        /// <param name="sitemapService">Sitemap service</param>
        /// <param name="logger">logger</param>
        public ContactsController(ModelAccessor modelAccessor, ISitemapService sitemapService, ILogger<ContactsController> logger) : base(modelAccessor, sitemapService, logger)
        {

        }

        public IActionResult Index()
        {
            // set the page title.
            PageTitle = Localization.Contacts_Title;

            return View();
        }
    }
}
