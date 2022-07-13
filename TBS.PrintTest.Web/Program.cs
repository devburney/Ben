using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Web;


namespace TBS.PrintTest.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit.
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((ctx, config) =>
                {
                    if (!ctx.HostingEnvironment.IsDevelopment())
                    {
                        // Build the config from sources we have
                        var builtConfig = config.Build();
                        // Create Managed Service Identity token provider
                        var tokenProvider = new AzureServiceTokenProvider();
                        // Create the Key Vault client
                        var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
                        // Add Key Vault to configuration pipeline
                        config.AddAzureKeyVault(builtConfig["KeyVault:BaseUrl"], kvClient, new DefaultKeyVaultSecretManager());
                        NLog.GlobalDiagnosticsContext.Set("logDirectory", "..\\LogFiles");
                    }
                    else
                    {
                        NLog.GlobalDiagnosticsContext.Set("logDirectory", "C:/Temp/");
                    }
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}
