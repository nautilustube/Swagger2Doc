

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Swagger2Doc.Libs;
using Swagger2Doc.Models.DTO;

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


        public static IServiceCollection AddCusHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            #region HttpClientFactory Retry polly
            IConfiguration webSitting = configuration.GetSection("WebSetting");
            int individualTimeoutSec = webSitting.GetValue("ApiRequestTimeoutSeconds", 30);
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(individualTimeoutSec);

            services.AddHttpClient<IRequest, Request>("HttpClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(individualTimeoutSec);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                UseCookies = false
            })
            .AddPolicyHandler((services, request) => HttpPolicyExtensions
                .HandleTransientHttpError() // 5XX 408
                .OrResult(msg => msg.IsSuccessStatusCode == false)
                .WaitAndRetryAsync(new[]
                    {  TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) },
                    onRetry: (outcome, timespan, retryAttempt, cpntext) =>
                    {
                        services.GetService<ILogger<IRequest>>()?.LogWarning($"Is Not Success StatusCode:{outcome.Result?.StatusCode}, Delaying for {timespan.TotalMilliseconds} ms");
                    }
                )
            ).AddPolicyHandler(timeoutPolicy);
            #endregion
            return services;
        }


        /// <summary>
        /// 註冊發信服務
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection emailSettings = configuration.GetSection("EmailSetting");
            string defaultFromEmail = emailSettings.GetValue("DefaultFromEmail", String.Empty);
            string host = emailSettings["SmtpHost"] ?? throw new ArgumentException("SmtpHost is missing in appsettting");
            int port = emailSettings.GetValue("SmtpPort", 25);
            string? userName = emailSettings["SmtpUserName"];
            string? password = emailSettings["smtpPassword"];

            if (!String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(password))
            {
                services
                .AddFluentEmail(defaultFromEmail)
                .AddRazorRenderer()
                .AddSmtpSender(host, port, userName, password);
            }
            else
            {
                services
                    .AddFluentEmail(defaultFromEmail)
                    .AddRazorRenderer()
                    .AddSmtpSender(host, port);
            }

            return services;
        }

    }
}
