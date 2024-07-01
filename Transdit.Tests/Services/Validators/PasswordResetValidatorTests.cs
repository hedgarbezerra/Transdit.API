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
    internal class PasswordResetValidatorTests
    {
        [Test]
        [TestCase("password", "password", "token", 1, false)]
        [TestCase("password", "", "", 2, false)]
        [TestCase("@Password123", "@Password123", "", 1, false)]
        [TestCase("@Password123", "", "", 2, false)]
        [TestCase("@Password123", "@Password123", "token", 0, true)]
        public void Validate(string password, string passConfirmation, string token, int errCount, bool isValid)
        {
            var validador = new PasswordResetValidator();
            var reset = new UserPasswordReset { Password = password, PasswordConfirm = passConfirmation, Token = token };

            var result = validador.Validate(reset);

            result.Should().NotBeNull();
            result.IsValid.Should().Be(isValid);
            result.Errors.Should().HaveCount(errCount);
        }
    }
}
