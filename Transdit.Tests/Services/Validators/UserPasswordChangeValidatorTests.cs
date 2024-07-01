using FluentValidation.Results;
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
    internal class UserPasswordChangeValidatorTests
    {
        [Test]
        public void Validate_OldPasswordEmpty_OldPasswordError()
        {
            var validator = new UserPasswordChangeValidator();
            var userpass = new UserPasswordChange()
            {
                Password = "",
                NewPassword = "",
                NewPasswordConfirm = ""
            };

            var result = validator.Validate(userpass);
            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid)
                .And.Match<ValidationResult>(r=> r.Errors.Count == 1);
        }

        [Test]
        public void Validate_NewPasswordEmpty_MultipleErrors()
        {
            var validator = new UserPasswordChangeValidator();
            var userpass = new UserPasswordChange()
            {
                Password = "password",
                NewPassword = "",
                NewPasswordConfirm = ""
            };

            var result = validator.Validate(userpass);
            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid)
                .And.Match<ValidationResult>(r => r.Errors.Count == 3);
        }

        [Test]
        public void Validate_NewPasswordSameAsOldAndNotInExpectedFormat()
        {
            var validator = new UserPasswordChangeValidator();
            var userpass = new UserPasswordChange()
            {
                Password = "password",
                NewPassword = "password",
                NewPasswordConfirm = ""
            };

            var result = validator.Validate(userpass);
            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(2)
                .And.Match(r => r.All(err => err.PropertyName == "NewPassword"));
        }

        [Test]
        public void Validate_NewPasswordCorrect_NewPasswordConfirmationNotEqual()
        {
            var validator = new UserPasswordChangeValidator();
            var userpass = new UserPasswordChange()
            {
                Password = "password",
                NewPassword = "@Password123",
                NewPasswordConfirm = ""
            };

            var result = validator.Validate(userpass);
            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(r => r.All(err => err.PropertyName == "NewPasswordConfirm"))
                .And.Match(r => r.All(err => err.ErrorCode == "EqualValidator"));
        }

        [Test]
        public void Validate_AllFieldCorrect_NoValidationIssues()
        {
            var validator = new UserPasswordChangeValidator();
            var userpass = new UserPasswordChange()
            {
                Password = "password",
                NewPassword = "@Password123",
                NewPasswordConfirm = "@Password123"
            };

            var result = validator.Validate(userpass);
            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => r.IsValid);
            result.Errors.Should().HaveCount(0);
        }


    }
}
