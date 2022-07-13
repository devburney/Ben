using cloudscribe.Web.Navigation;
using GoC.WebTemplate.Components.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBS.PrintTest.Web.Infrastructure.Services.Interfaces
{
    public interface ISitemapService
    {
        Task<List<Breadcrumb>> GetBreadcrumbs(bool includeCurrentNode = true, bool includeRoot = true);
        Task<MenuLink> GetCurrentMenuLink();
        Task<TreeNode<NavigationNode>> GetCurrentSitemapNode();
        Task<List<MenuLink>> GetMenu(bool includeRoot = true);
        Task<TreeNode<NavigationNode>> GetSitemapNode(string key);
    }
}
