using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Notification;

namespace Transdit.Core.Contracts
{
    public interface IMailNotificationBuilder
    {
        IHasTitle New();
        EmailNotification Build();
    }
    public interface IHasTitle : IMailNotificationBuilder
    {
        IHasMessage HasTitle(string title);
    }
    public interface IHasMessage : IMailNotificationBuilder
    {
        IHasAttachments WithMessage(string message);
    }
    public interface IHasAttachments : IWithRecipient
    {
        IHasAttachments HasAttachment(Attachment attachment);
        IHasAttachments HasAttachments(IList<Attachment> attachmenst);
    }
    public interface IWithRecipient : IMailNotificationBuilder
    {
        IHasAttachments WithRecipient(string name, string address);
        IHasAttachments WithRecipient(Recipient recipient);
        IHasAttachments WithRecipients(IList<Recipient> recipients);

    }
}
