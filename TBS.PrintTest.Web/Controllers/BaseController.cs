
using Foundation.Core.Enums;
using Foundation.Core.Helpers;
using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.CoreMVC.Controllers;
using TBS.PrintTest.Web.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;


namespace TBS.PrintTest.Web.Controllers
{
    public class BaseController : WebTemplateBaseController
    {

        private readonly ILogger _logger;
        public readonly ISitemapService SitemapService;
        public readonly ModelAccessor WebTemplateCore;

        public Language Language { get { return CultureHelper.GetCurrentLanguage(); } }
        public string PageDescription { get; set; }
        public string PageTitle { get; set; }


        public BaseController(ModelAccessor modelAccessor, ISitemapService sitemapService, ILogger logger)
            : base(modelAccessor)
        {
            _logger = logger;
            SitemapService = sitemapService;
            WebTemplateCore = modelAccessor;
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Title, description and version identifier.
            ViewBag.PageDescription = PageDescription;
            ViewBag.PageTitle = PageTitle;

            base.OnActionExecuted(filterContext);
        }

        public void LogError(Exception ex)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>
                {
                    {"status_code" , HttpContext.Response.StatusCode },
                    //{ "userId", "42" } <-- or whatever else we want to configure.
                };

            _logger.LogError(ex, ex.Message, additionalInfo);
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(1, message);
        }

        public void LogInformation(string info)
        {
            _logger.LogInformation(info);
        }
    }
}
