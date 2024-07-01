using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Builders;
using Transdit.Core.Models.Notification;

namespace Transdit.Tests.Core.Builders
{
    [TestFixture]
    public class MailNotificationBuilderTests
    {
        [Test]
        public void BuildNotification_SingleRecipientWithNoAttachment()
        {
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .WithRecipient("hedgar", "email@email.com")
                .Build();

            notification.Should().NotBeNull();
            notification.Attachments.Should().BeEmpty();
            notification.Recipients.Should().NotBeEmpty().And.HaveCount(1);
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }

        [Test]
        public void BuildNotification_NoReceipientAndNoAttachment()
        {
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .Build();

            notification.Should().NotBeNull();
            notification.Attachments.Should().BeEmpty();
            notification.Recipients.Should().BeEmpty();
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }

        [Test]
        public void BuildNotification_MultipleRecipients()
        {
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .WithRecipient("hedgar", "email@email.com")
                .WithRecipient("junior", "junior@email.com")
                .Build();

            notification.Should().NotBeNull();
            notification.Attachments.Should().BeEmpty();
            notification.Recipients.Should().NotBeEmpty().And.HaveCount(2);
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }

        [Test]
        public void BuildNotification_SingleRecipientWithAttachment()
        {
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .WithRecipient("hedgar", "email@email.com")
                .HasAttachment(new Attachment())
                .Build();

            notification.Should().NotBeNull();
            notification.Attachments.Should().NotBeEmpty().And.HaveCount(1);
            notification.Recipients.Should().NotBeEmpty().And.HaveCount(1);
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }

        [Test]
        public void BuildNotification_ListOfRecipients_RecipientsEmpty()
        {
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .WithRecipients(new List<Recipient>())
                .Build();

            notification.Should().NotBeNull();
            notification.Attachments.Should().BeEmpty();
            notification.Recipients.Should().BeEmpty();
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }

        [Test]
        public void BuildNotification_withRecipientObject_AddsNewRecipient()
        {
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .WithRecipient(new Recipient())
                .Build();

            notification.Should().NotBeNull();
            notification.Attachments.Should().BeEmpty();
            notification.Recipients.Should().NotBeEmpty().And.HaveCount(1);
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }

        [Test]
        public void BuildNotification_ListOfRecipients_ListOverridesPreviousRecipients()
        {
            var receipients = new List<Recipient>()
            {
                new Recipient(),
                new Recipient()
            };
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .WithRecipient("hedgar", "email@email.com")
                .WithRecipients(receipients)
                .Build();

            notification.Should().NotBeNull();
            notification.Attachments.Should().BeEmpty();
            notification.Recipients.Should().NotBeEmpty().And.HaveCount(2);
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }

        [Test]
        public void BuildNotification_ListOfAttachments_ListOverrides()
        {
            var attachments = new List<Attachment>()
            {
                new Attachment(),
                new Attachment()
            };
            var notification = new MailNotificationBuilder().New()
                .HasTitle("Titulo")
                .WithMessage("Mensagem")
                .WithRecipient("hedgar", "email@email.com")
                .HasAttachments(attachments)
                .Build();

            notification.Should().NotBeNull();
            notification.Recipients.Should().NotBeEmpty().And.HaveCount(1);
            notification.Attachments.Should().NotBeEmpty().And.HaveCount(2);
            notification.Message.Should().NotBeNullOrEmpty().And.Be("Mensagem");
            notification.Title.Should().NotBeNullOrEmpty().And.Be("Titulo");
        }
    }
}
