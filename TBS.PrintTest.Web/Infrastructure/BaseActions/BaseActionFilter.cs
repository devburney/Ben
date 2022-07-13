using AppResources = TBS.PrintTest.Web.Resources;
using CommonResources = Foundation.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using TBS.PrintTest.Web.Controllers;

namespace TBS.PrintTest.Web.Infrastructure.BaseActions
{

    public class BaseActionFilter : IActionFilter
    {
        public async void OnActionExecuting(ActionExecutingContext context)
        {
            var baseController = (BaseController)context.Controller;

            var controller = (Controller)context.Controller;
            var urlHelper = (IUrlHelper)controller.Url;
            var webTemplateModel = baseController.WebTemplateCore.Model;
            controller.ViewData.Add("WebTemplateModel", webTemplateModel);

            // Build breadcrumbs.
            var breadcrumbs = await baseController.SitemapService.GetBreadcrumbs();
            if (breadcrumbs != null)
                webTemplateModel.Breadcrumbs = breadcrumbs;

            // Build menu.
            var menu = await baseController.SitemapService.GetMenu();
            if (menu != null)
                webTemplateModel.MenuLinks = menu;

            var currentMenuLink = await baseController.SitemapService.GetCurrentMenuLink();
            if (currentMenuLink != null)
            {
                if (string.IsNullOrWhiteSpace(baseController.PageTitle) && !string.IsNullOrWhiteSpace(currentMenuLink.Text))
                {
                    baseController.PageTitle = currentMenuLink.Text;
                    webTemplateModel.HeaderTitle = currentMenuLink.Text;
                }
                {
                    webTemplateModel.HeaderTitle = baseController.PageTitle;
                }

                webTemplateModel.LanguageLink.Href = LocalizeLanguageLinkHref(context, urlHelper);
            }

            webTemplateModel.ShowSignOutLink = false;

            // Application specifics.
            var fileVersionDetails = typeof(Startup).Assembly.GetName().Version;
            webTemplateModel.ApplicationTitle.Href = urlHelper.Action("Index", "Home");
            webTemplateModel.ApplicationTitle.Text = AppResources.Localization.ApplicationTitle;
            webTemplateModel.VersionIdentifier = $"{fileVersionDetails.Major}.{fileVersionDetails.Minor}.{fileVersionDetails.Build}";

            // css files
            webTemplateModel.HTMLHeaderElements.Add($"<link rel=\"stylesheet\" href=\"/css/foundation.css\">");
            //webTemplateModel.HTMLHeaderElements.Add($"<link rel=\"stylesheet\" href=\"/css/template.css\">"); <-- example on how to add custom css.

            // javascript files
            webTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/config.js\"></script>");
            webTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/foundation.core.js\"></script>");
            webTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/print.js\"></script>");

            // app version
            webTemplateModel.HTMLBodyElements.Add($"<!-- TBS.PrintTest App Build: {fileVersionDetails.ToString()} -->");

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action has executed.
        }

        private string LocalizeLanguageLinkHref(ActionExecutingContext context, IUrlHelper urlHelper)
        {
            var controller = (Controller)context.Controller;

            // Language link.
            var languageRouteData = new RouteValueDictionary();
            // - Clone RouteData.Values.
            foreach (var data in controller.RouteData.Values)
                languageRouteData.Add(data.Key, data.Value);
            // - Toggle the culture.
            if (languageRouteData.ContainsKey("culture"))
                languageRouteData["culture"] = CommonResources.Localization.OtherLanguageTwoLetterLanguage;
            else
                languageRouteData.Add("culture", CommonResources.Localization.OtherLanguageTwoLetterLanguage);

            return urlHelper.Action((languageRouteData["action"].ToString() ?? "index"), languageRouteData);
        }
    }
}
