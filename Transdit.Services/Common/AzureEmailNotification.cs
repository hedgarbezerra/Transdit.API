using Azure;
using Azure.Communication.Email;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Notification;

namespace Transdit.Services.Common
{
    public class AzureEmailNotificator : INotificator<AzureEmailNotification>
    {
        private readonly EmailClient _client;
        private readonly ILogger<AzureEmailNotificator> _logger;

        public AzureEmailNotificator(string connectionString, ILogger<AzureEmailNotificator> logger)
        {
            _client = new EmailClient(connectionString);
            _logger = logger;
        }

        public async Task Notify(AzureEmailNotification notification)
        {
            try
            {
                var email = CreateMessage(notification);
                var emailSendOperation = await _client.SendAsync(WaitUntil.Started, email);

                _logger.LogInformation($"Email operation was sent.", emailSendOperation, email);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, $"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
            }
        }

        private EmailMessage CreateMessage(AzureEmailNotification notification)
        {
            var emailAdresses = notification.Recipients.Select(r => new EmailAddress(r.Address, r.Name));
            var recipients = new EmailRecipients(emailAdresses);
            var content = new EmailContent(notification.Title) { Html = notification.Message};
            var email = new EmailMessage("DoNotReply@transdit.com.br", recipients, content);
            notification.Attachments.ToList().ForEach(at =>
                email.Attachments.Add(new EmailAttachment(at.Name, at.Type, BinaryData.FromStream(at.Content))));

            return email;
        }
    }
}
