using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Notification;

namespace Transdit.Core.Builders
{

    public class MailNotificationBuilder : IHasTitle,
        IHasMessage, IHasAttachments
    {
        private EmailNotification _email;
        public IHasTitle New()
        {
            _email = new EmailNotification();

            return this;
        }

        public EmailNotification Build() => _email;

        public IHasAttachments HasAttachment(Attachment attachment)
        {
            _email.Attachments.Add(attachment);

            return this;
        }

        public IHasAttachments HasAttachments(IList<Attachment> attachments)
        {
            _email.Attachments = attachments;
            return this;
        }

        public IHasMessage HasTitle(string title)
        {
            _email.Title = title;
            return this;
        }


        public IHasAttachments WithMessage(string message)
        {
            _email.Message = message;
            return this;
        }

        public IHasAttachments WithRecipient(string name, string address)
        {
            var recipient = new Recipient() { Name = name, Address = address };
            _email.Recipients.Add(recipient);
            return this;
        }

        public IHasAttachments WithRecipient(Recipient recipient)
        {
            _email.Recipients.Add(recipient);
            return this;
        }

        public IHasAttachments WithRecipients(IList<Recipient> recipients)
        {
            _email.Recipients = recipients;
            return this;
        }
    }
}
