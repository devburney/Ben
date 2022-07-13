using cloudscribe.Web.Navigation;
using cloudscribe.Web.Navigation.Caching;
using GoC.WebTemplate.Components.Entities;
using TBS.PrintTest.Web.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBS.PrintTest.Web.Infrastructure.Services
{
    public class SitemapService : ISitemapService
    {
        private readonly IHttpContextAccessor ContextAccessor;
        private readonly IActionContextAccessor ActionContextAccessor;
        private readonly NavigationTreeBuilderService NavigationTreeBuilderService;
        private readonly IStringLocalizer<MenuResources> StringLocalizer;
        private readonly ITreeCache TreeCache;
        private readonly IUrlHelper UrlHelper;

        private List<Breadcrumb> breadcrumbs;
        //private MenuLink currentMenuLink;
        private List<MenuLink> menu;
        private TreeNode<NavigationNode> sitemapTree;

        public SitemapService(IHttpContextAccessor contextAccessor, IActionContextAccessor actionContextAccessor, IStringLocalizer<MenuResources> stringLocalizer, ITreeCache treeCache, IUrlHelper urlHelper, NavigationTreeBuilderService navigationTreeBuilderService)
        {
            this.ContextAccessor = contextAccessor;
            this.ActionContextAccessor = actionContextAccessor;
            this.NavigationTreeBuilderService = navigationTreeBuilderService;
            this.StringLocalizer = stringLocalizer;
            this.TreeCache = treeCache;
            this.UrlHelper = urlHelper;
        }

        public async Task<List<Breadcrumb>> GetBreadcrumbs(bool includeCurrentNode = true, bool includeRoot = true)
        {
            if (this.breadcrumbs == null)
            {
                var currentSiteMapNode = await GetCurrentSitemapNode();
                if (currentSiteMapNode != null)
                {
                    var list = new List<Breadcrumb>();
                    var parentNodeChain = currentSiteMapNode.GetParentNodeChain(true, true);
                    foreach (var node in parentNodeChain)
                    {
                        var breadcrumb = BuildBreadcrumb(node, node == currentSiteMapNode);
                        list.Add(breadcrumb);
                    }
                    this.breadcrumbs = list;
                }
            }
            return this.breadcrumbs;
        }
        public async Task<MenuLink> GetCurrentMenuLink()
        {
            var currentSiteMapNode = await GetCurrentSitemapNode();
            if (currentSiteMapNode != null)
            {
                return BuildMenuLink(currentSiteMapNode);
            }
            return null;
        }
        public async Task<TreeNode<NavigationNode>> GetCurrentSitemapNode()
        {
            if (this.sitemapTree == null)
                this.sitemapTree = await BuildTree();

            var controllerName = this.ActionContextAccessor.ActionContext.RouteData.Values["controller"].ToString().ToLowerInvariant();
            var actionName = this.ActionContextAccessor.ActionContext.RouteData.Values["action"].ToString().ToLowerInvariant();
            var currentSitemapNode = this.sitemapTree.FindByControllerAndAction(controllerName, actionName);
            return currentSitemapNode;
        }
        public async Task<List<MenuLink>> GetMenu(bool includeRoot = true)
        {
            if (this.menu == null)
            {
                if (this.sitemapTree == null)
                    this.sitemapTree = await BuildTree();
                if (this.sitemapTree != null)// && this.sitemapTree.IsRoot()) //  && this.sitemapTree.IsRoot()  <-- This has been deprecated.  Don't have an equivalent but not sure if needed.
                    this.menu = BuildMenu(this.sitemapTree, includeRoot);
            }

            return this.menu;
        }
        public async Task<TreeNode<NavigationNode>> GetSitemapNode(string key)
        {
            if (this.sitemapTree == null)
                this.sitemapTree = await BuildTree();

            var node = this.sitemapTree.FindByKey(key);
            return node;
        }

        #region Building methods
        private Breadcrumb BuildBreadcrumb(TreeNode<NavigationNode> node, bool isCurrentNode)
        {
            Breadcrumb breadcrumb = null;
            if (node != null)
            {
                breadcrumb = new Breadcrumb();
                if (!isCurrentNode)
                    breadcrumb.Href = node.Value.ResolveUrl(this.UrlHelper).TrimStart('~');

                var localizedText = StringLocalizer[node.Value.Text];
                breadcrumb.Title = localizedText.Value;
            }
            return breadcrumb;
        }
        private List<MenuLink> BuildMenu(TreeNode<NavigationNode> rootNode, bool includeRoot)
        {
            List<MenuLink> menu = null;
            if (rootNode != null && rootNode.Children != null && rootNode.Children.Count > 0)
            {
                menu = new List<MenuLink>();

                if (includeRoot)
                {
                    MenuLink rootMenuLink = BuildMenuLink(rootNode);
                    if (rootMenuLink != null)
                        menu.Add(rootMenuLink);
                }

                foreach (var child in rootNode.Children)
                {
                    if (ShowInMenu(child))
                    {
                        MenuLink menuLink = BuildMenuLink(child);

                        // only build submenu if specified in parent node.
                        if (IncludeChildrenInSubMenu(child))
                        {
                            if (menuLink != null && child.Children != null && child.Children.Count > 0)
                            {
                                menuLink.SubLinks = BuildSubMenu(child);
                            }
                        }

                        menu.Add(menuLink);
                    }
                }
            }
            return menu;
        }

        private bool IncludeChildrenInSubMenu(TreeNode<NavigationNode> node)
        {
            if (node.Value.ComponentVisibility == "addChildrenToSubmenu")
                return true;

            return false;
        }

        private bool ShowInMenu(TreeNode<NavigationNode> node)
        {
            if (node.Value.ComponentVisibility == "")
                return true;

            if (node.Value.ComponentVisibility.Contains("none"))
                return false;

            return true;
        }

        private MenuLink BuildMenuLink(TreeNode<NavigationNode> node)
        {
            MenuLink menuLink = null;
            if (node != null)
            {
                menuLink = new MenuLink();
                menuLink.Href = node.Value.ResolveUrl(this.UrlHelper).TrimStart('~');

                var localizedText = StringLocalizer[node.Value.Text];
                menuLink.Text = localizedText.Value;

                if (!string.IsNullOrEmpty(node.Value.Url))
                {
                    var localizedUrl = StringLocalizer[node.Value.Url];
                    menuLink.Href = localizedUrl.Value;
                }

                // using the page property to add a params to url
                if (!string.IsNullOrEmpty(node.Value.Page))
                {
                    var localizedPage = node.Value.Page;
                    menuLink.Href = $"{menuLink.Href}/{localizedPage}";
                }

                if (node.Value.Target == "_blank") menuLink.NewWindow = true;
            }
            return menuLink;
        }
        private SubLink BuildSubLink(TreeNode<NavigationNode> node)
        {
            SubLink subLink = null;
            if (node != null)
            {
                subLink = new SubLink();
                subLink.Href = node.Value.ResolveUrl(this.UrlHelper).TrimStart('~');

                var localizedText = StringLocalizer[node.Value.Text];
                subLink.Text = localizedText.Value;

                if (!string.IsNullOrEmpty(node.Value.Url))
                {
                    var localizedUrl = StringLocalizer[node.Value.Url];
                    subLink.Href = localizedUrl.Value;
                }

                // using the page property to add a params to url
                if (!string.IsNullOrEmpty(node.Value.Page))
                {
                    var localizedPage = node.Value.Page;
                    if (!subLink.Href.ToUpper().Contains($"/{node.Value.Action.ToUpper()}"))
                    {
                        subLink.Href = $"{subLink.Href}/{node.Value.Action}/{localizedPage}";
                    }
                    else
                    {
                        // if links have been built before, remove old param and add new one from site nav xml.
                        if (subLink.Href.ToUpper().Contains($"/{node.Value.Action.ToUpper()}"))
                        {
                            subLink.Href = $"{subLink.Href.Substring(0, subLink.Href.ToUpper().IndexOf($"/{node.Value.Action.ToUpper()}"))}/{node.Value.Action}/{localizedPage}";
                        }
                        else
                        {
                            subLink.Href = $"{subLink.Href}/{localizedPage}";
                        }
                    }
                }

                if (node.Value.Target == "_blank") subLink.NewWindow = true;
            }
            return subLink;
        }
        private List<SubLink> BuildSubMenu(TreeNode<NavigationNode> node)
        {
            List<SubLink> subMenu = null;
            if (node != null && node.Children != null && node.Children.Count > 0)
            {
                subMenu = new List<SubLink>();
                foreach (var child in node.Children)
                {
                    if (ShowInMenu(child))
                    {
                        SubLink subLink = BuildSubLink(child);
                        subMenu.Add(subLink);
                    }
                }
            }
            return subMenu;
        }
        private async Task<TreeNode<NavigationNode>> BuildTree()
        {
            TreeNode<NavigationNode> tree;
            tree = await this.TreeCache.GetTree("RootNode");
            if (tree == null)
            {
                var builder = this.NavigationTreeBuilderService.GetRootTreeBuilder();
                tree = await builder.BuildTree(this.NavigationTreeBuilderService);
                await this.TreeCache.AddToCache(tree, "RootNode");
            }
            return tree;
        }
        #endregion
    }
}
