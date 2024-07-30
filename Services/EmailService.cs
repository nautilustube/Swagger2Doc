using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swagger2Doc.Models.DTO;

namespace Swagger2Doc.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailMetaData emailMetadata, bool isHtml = false);

    }

    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        private readonly IConfiguration _config;

        private readonly IFluentEmailFactory _fluentEmail;

        public EmailService(ILogger<EmailService> logger, IConfiguration config, IFluentEmailFactory fluentEmail)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _fluentEmail = fluentEmail ?? throw new ArgumentNullException(nameof(fluentEmail));
        }

        public async Task SendAsync(EmailMetaData emailMetadata, bool isHtml = false)
        {
            await _fluentEmail.Create().To(emailMetadata.ToAddress)
            .CC(emailMetadata.CCToAddress)
            .Subject(emailMetadata.Subject)
            .Body(emailMetadata.Body, isHtml)
            .SendAsync();

            _logger.LogInformation($"{emailMetadata.ToAddress}: ${emailMetadata.Subject} : 已寄送");
        }
    }
}
