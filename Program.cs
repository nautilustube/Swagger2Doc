using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using Swagger2Doc.Helpers;
using Swagger2Doc.Services;
using Swagger2Doc.Startup;

namespace Swagger2Doc
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            #region NLog Init 
            string configuringFileName = "NLog.config";
            // WebHost : ASPNETCORE_ENVIRONMENT
            // Console : DOTNET_ENVIRONMENT
            string? aspnetEnvironment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            string environmentSpecificLogFileName = $"NLog.{aspnetEnvironment}.config";
            if (File.Exists(environmentSpecificLogFileName))
            {
                configuringFileName = environmentSpecificLogFileName;
            }
            Logger logger = NLog.LogManager.Setup().LoadConfigurationFromFile(configuringFileName).GetCurrentClassLogger();
            #endregion
            try
            {
                IHostBuilder host = Host.CreateDefaultBuilder(args);

                // NLog Setup
                host.ConfigureLogging((hostContext, logging) =>
                {
                    // ensure just use NLog
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logging.AddNLog(configuringFileName);
                });

                // Config Setup
                host.ConfigureAppSetting();

                // HttpClient Setup
                host.ConfigureServices((hostContext, services) =>
                {
                    services.AddCusHttpClient(hostContext.Configuration);
                });

                // DI Cus Services
                host.ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<DapperHelper>();                     // Sql Dapper Helper
                    services.AddTransient<IEmailService, EmailService>();   // Add Email Service
                    services.AddFluentEmail(hostContext.Configuration);     // 註冊發信服務
                    services.AddTransient<CoreService>();  // 利害人關係比對發信 Service
                });
                host.UseConsoleLifetime();

                IHost app = host.Build();
                logger.Info("Host Instance Builed:" + aspnetEnvironment);

                // Run Cus Services
                using (var serviceScope = app.Services.CreateScope())
                {
                    IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                    CoreService service1 = serviceProvider.GetService<CoreService>() ?? throw new ArgumentNullException(nameof(CoreService));
                    await service1.Run();
                }
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, $"Stopped program because of exception: {JsonConvert.SerializeObject(ex)}");
                throw;
            }
            finally
            {
                logger.Info("program Shutdown");
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }

        }

    }
}