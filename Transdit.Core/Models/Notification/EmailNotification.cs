using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Notification
{
    public class EmailNotification : BaseNotification
    {
        public IList<Recipient> Recipients { get; set; } = Enumerable.Empty<Recipient>().ToList();
        public IList<Attachment> Attachments { get; set; } = Enumerable.Empty<Attachment>().ToList();
    }

    public class AzureEmailNotification : EmailNotification
    {

    }
}
