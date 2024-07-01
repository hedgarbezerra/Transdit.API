using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Users;
using Transdit.Services.Validators;

namespace Transdit.Tests.Services.Validators
{
    [TestFixture]
    internal class EmailUpdateValidatorTests
    {
        [Test]
        [TestCase("email", "confirmation", 1, false)]
        [TestCase("email", "", 1, false)]
        [TestCase("email@hot.com", "", 3, false)]
        [TestCase("email@hot.com", "email@hot.com", 0, true)]
        public void Validate(string email, string confirmation, int errCount, bool isValid)
        {
            var validador = new EmailUpdateValidator();
            var update = new EmailUpdate { NewEmail = email, NewEmailConfirmation = confirmation };

            var result = validador.Validate(update);

            result.Should().NotBeNull();
            result.IsValid.Should().Be(isValid);
            result.Errors.Should().HaveCount(errCount);
        }
    }
}
