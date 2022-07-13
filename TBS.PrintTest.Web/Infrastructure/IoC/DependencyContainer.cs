using AutoMapper;
using TBS.PrintTest.Business.Services;
using TBS.PrintTest.Business.Services.Interfaces;
using TBS.PrintTest.Web.Infrastructure.Services;
using TBS.PrintTest.Web.Infrastructure.Services.Interfaces;
using TBS.PrintTest.DataAccess.Repositories;
using TBS.PrintTest.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace TBS.PrintTest.Web.Infrastructure.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {

            // Infrastructure - Services
             services.TryAddScoped<ISitemapService, SitemapService>();

            // Business Layer - Services.
            services.AddScoped<ICodelookupService, CodelookupService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            // Data Access Layer - Repositories.
            services.AddScoped<ICodelookupRepository, CodelookupRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            // get the presentation layer (executing assembly) and business layer assemblies.
            // by adding the assemlies to the automapper injection, all classes that inherit from Profile will be injected.
            var presentationAssembly = Assembly.GetExecutingAssembly();
            var businessAssembly = typeof(Business.MappingProfiles.EmployeeMappingProfile).GetTypeInfo().Assembly; // just need to reference one class in order to get business layer assembly.

            // Add assemblies to automapper
            Assembly[] assemblies = new Assembly[2];
            assemblies[0] = presentationAssembly;
            assemblies[1] = businessAssembly;
            services.AddAutoMapper(assemblies);

            // Others.
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
        }
    }
}
