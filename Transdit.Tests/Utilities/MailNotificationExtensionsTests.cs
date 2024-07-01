using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Notification;
using Transdit.Utilities.Extensions;

namespace Transdit.Tests.Utilities
{
    [TestFixture]
    public class MailNotificationExtensionsTests
    {
        Fixture _fixture;
        MockRepository _mockRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fixture= new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Default);
        }

        [Test]
        public void MailNotificationExtensionsTest_ExpectedToWorkProperly()
        {
            BodyBuilder builder = new BodyBuilder();
            Attachment file = new Attachment()
            {
                Name = "file",
                Type = "content/json",
                Content = new MemoryStream()
            };

            builder.Attachments.Should().BeEmpty();
            builder.AddAttachment(file);
            builder.Attachments.Should().HaveCount(1);
        }

        [Test]
        public void MailNotificationExtensionsTest_BuilderIsNull()
        {
            BodyBuilder builder = null;
            Attachment file = new Attachment()
            {
                Name = "file",
                Type = "content/json",
                Content = new MemoryStream()
            };

            builder?.Attachments.Should().BeNull();
            builder.AddAttachment(file);
            builder?.Attachments.Should().BeNull();
        }

        [Test]
        public void MailNotificationExtensionsTest()
        {
            BodyBuilder builder = new BodyBuilder();
            Attachment file = null;

            builder.Attachments.Should().BeEmpty();
            builder.AddAttachment(file);
            builder.Attachments.Should().BeEmpty();
        }
    }
}
