using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Swagger2Doc.Startup
{
    public static class DependencyInjectSetup
    {

        public static IHostBuilder ConfigureAppSetting(this IHostBuilder host)
        {
            host.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(System.AppContext.BaseDirectory);
                config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                                   optional: false,
                                   reloadOnChange: true);

            });
            return host;
        }
    }
}
