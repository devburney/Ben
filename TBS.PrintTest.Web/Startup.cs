using GoC.WebTemplate.Components.Core.Services;
using TBS.PrintTest.Web.Infrastructure.BaseActions;
using TBS.PrintTest.Web.Infrastructure.IoC;
using TBS.PrintTest.Web.Infrastructure.Providers;
using TBS.PrintTest.DataAccess.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;



namespace TBS.PrintTest.Web
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        private IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest)
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = "Resources"; })
            .AddDataAnnotationsLocalization()
            .AddMvcOptions(options =>
            {
                options.CacheProfiles.Add("SitemapCacheProfile",
                                 new CacheProfile
                                 {
                                     Duration = 100
                                 });
                options.Filters.Add(new BaseActionFilter());
                options.EnableEndpointRouting = false;
            });

            // Configure the Antiforgery token.
            services.AddAntiforgery(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Add runtime compilation.
            services.AddRazorPages().AddRazorRuntimeCompilation(); 

            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            });

            // Configure Cloudscribe's navigation.
            services.AddCloudscribeNavigation(Configuration.GetSection("NavigationOptions"));

            // Configure Localization.
            services.Configure<GlobalResourceOptions>(Configuration.GetSection("GlobalResourceOptions"));
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<IStringLocalizerFactory, GlobalResourceManagerStringLocalizerFactory>();
            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-GB"), new CultureInfo("en-CA"), new CultureInfo("en-US"), new CultureInfo("en"),
                        new CultureInfo("fr-CA"), new CultureInfo("fr")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en-CA");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                }
            );

            // Configure GoC.WebTemplate (CDTS).
            var inactivity = this.Configuration.GetSection("GoC.WebTemplate")["inactivity"];
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMilliseconds(Convert.ToDouble(inactivity));
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Get database connection string.
            string connectionString = string.Empty;
            if (!Environment.IsDevelopment())
            {
                string keyName = Configuration.GetSection("KeyVault")["ConnectionStringKeyName"];
                connectionString = Configuration[keyName];
            }
            else
            {
                connectionString = Configuration.GetConnectionString("DefaultConnection");
            }

            // Configure context for EF Core.
            services.AddDbContext<IMTDTemplateContext>(
                options =>
                {
                    options.UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(30));
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                }
                , ServiceLifetime.Scoped
            );

            // Add accessor to HttpContext.
            services.AddHttpContextAccessor();

            // Add GoC Template model accessor
            services.AddModelAccessor();

            // configire GoC Template localization.
            services.ConfigureGoCTemplateRequestLocalization();

            // define other possible injections
            RegisterServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // for testing exception handler locally.
                //app.UseExceptionHandler("/" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + "/Home/Error");
            }
            else
            {
                app.UseExceptionHandler("/" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + "/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict,
                Secure = CookieSecurePolicy.Always
            });


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();


            // Localization.
            var localizationOptions = new RequestLocalizationOptions();
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-GB"), new CultureInfo("en-CA"), new CultureInfo("en-US"), new CultureInfo("en"),
                new CultureInfo("fr-CA"), new CultureInfo("fr")
            };

            localizationOptions.DefaultRequestCulture = new RequestCulture("en-CA");
            localizationOptions.SupportedCultures = supportedCultures;
            localizationOptions.SupportedUICultures = supportedCultures;
            localizationOptions.RequestCultureProviders.Clear();
            localizationOptions.RequestCultureProviders.Add(new RouteCultureProvider(localizationOptions.DefaultRequestCulture));
            app.UseRequestLocalization(localizationOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    //template: "{controller=Home}/{action=Index}/{id?}");
                    template: "{culture=en}/{controller=Home}/{action=Index}/{id?}");
            });

        }

        private static void RegisterServices(IServiceCollection services)
        {
            DependencyContainer.RegisterServices(services);
        }
    }
}
