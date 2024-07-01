using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics.CodeAnalysis;
using Transdit.Core.Constants;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Notification;
using Transdit.Utilities.Extensions;

namespace Transdit.Services.Common
{
    [ExcludeFromCodeCoverage]
    public class MailNotificator : INotificator<EmailNotification>
    {
        private readonly BodyBuilder _messageBodyBuilder;
        private readonly SmtpSettings _smtpSettings;
        private readonly ICryptography _cryptography;
        public MailNotificator(SmtpSettings smtpSettings, ICryptography cryptography)
        {
            _messageBodyBuilder = new BodyBuilder();
            _smtpSettings = smtpSettings;
            _cryptography = cryptography;
        }

        public async Task Notify(EmailNotification notification)
        {
            var message = NewMessage(notification);

            await SendMessage(message);
        }

        private async Task SendMessage(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(_smtpSettings.Hostname, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                var decryptedEmail = _cryptography.Decrypt(_smtpSettings.Email);
                var decryptedPassword = _cryptography.Decrypt(_smtpSettings.Password);
                await client.AuthenticateAsync(decryptedEmail, decryptedPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        private MimeMessage NewMessage(EmailNotification notification)
        {
            var decryptedEmail = _cryptography.Decrypt(_smtpSettings.Email);
            var message = new MimeMessage();
            message.Subject = notification.Title;
            message.From.Add(new MailboxAddress(notification.Title, decryptedEmail));
            message.ReplyTo.Add(new MailboxAddress("Transdit", decryptedEmail));

            foreach (var recipient in notification.Recipients)
                message.To.Add(new MailboxAddress(recipient.Name, recipient.Address));

            foreach (var item in notification.Attachments)
                _messageBodyBuilder.AddAttachment(item);

            _messageBodyBuilder.HtmlBody = notification.Message;
            message.Body = _messageBodyBuilder.ToMessageBody();

            return message;
        }
    }
}