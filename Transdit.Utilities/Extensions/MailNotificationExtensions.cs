using MimeKit;
using Transdit.Core.Models.Notification;

namespace Transdit.Utilities.Extensions
{
    public static class MailNotificatorExtensions
    {
        public static void AddAttachment(this BodyBuilder builder, Attachment file)
        {
            if (builder is null || file is null)
                return;

            var multipart = new Multipart("mixed");
            var mimeAttachment = new MimePart(file.Type)
            {
                Content = new MimeContent(file.Content),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = file.Name
            };

            multipart.Add(mimeAttachment);
            builder.Attachments.Add(multipart);
        }
    }
}
