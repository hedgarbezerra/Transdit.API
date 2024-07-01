using AutoFixture;
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
    public class UserSignUpValidatorTests
    {
        UserSignUpValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _validator = new UserSignUpValidator();
        }

        [Test]
        public void Validate_AllFieldsInvalid_SeveralErrors()
        {
            var signUp = new UserSignUp
            {
                Name = "",
                UserName = "",
                Email = "notvalidemail",
                BirthDate = DateTime.Now,
                PlanId = 0,
                Password = "",
                PasswordConfirm = ""
            };
            IEnumerable<string> failedProperties = new List<string>()
            {
                "Name", "UserName", "Email", "BirthDate", "PlanId", "Password"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(8)
                .And.Match(err => err.Select(e => e.PropertyName).All(field => failedProperties.Any(p => p.Equals(field))));
        }

        [Test]
        public void Validate_InvalidEmail_EmailProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotworkcom",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(err => err.All(e => e.PropertyName == "Email"));
        }

        [Test]
        public void Validate_ValidEmail_NoErrors()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => r.IsValid);
            result.Errors.Should().HaveCount(0);
        }


        [Test]
        public void Validate_FiveYearsOld_BirthDateProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-5),
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(err => err.All(e => e.PropertyName == "BirthDate"));
        }
        [Test]
        public void Validate_InvalidBirthdate_BirthDateProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now,
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(err => err.All(e => e.PropertyName == "BirthDate"));
        }

        [Test]
        public void Validate_ValidBirthdate_NoErrors()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => r.IsValid);
            result.Errors.Should().HaveCount(0);
        }


        [Test]
        public void Validate_InvalidPlan_PlanProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 0, 
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(err => err.All(e => e.PropertyName == "PlanId"));
        }

        [Test]
        public void Validate_InvalidPlanNegativeID_PlanProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = -1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(err => err.All(e => e.PropertyName == "PlanId"));
        }

        [Test]
        public void Validate_ValidServicePlan_NoErrors()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => r.IsValid);
            result.Errors.Should().HaveCount(0);
        }

        [Test]
        public void Validate_EmptyPassword_PasswordProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(3)
                .And.Match(err => err.All(e => e.PropertyName == "Password"));
        }
        [Test]
        public void Validate_InvalidPassword_PasswordProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "@Password",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(err => err.All(e => e.PropertyName == "Password"));
        }
        [Test]
        public void Validate_PasswordsDontMatch_ConfirmPasswordProperty()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => !r.IsValid);
            result.Errors.Should().HaveCount(1)
                .And.Match(err => err.All(e => e.PropertyName == "PasswordConfirm"));
        }

        [Test]
        public void Validate_ValidPassword_NoErrors()
        {
            var signUp = new UserSignUp
            {
                Name = "name",
                UserName = "username",
                Email = "invalidemail@hotwork.com",
                BirthDate = DateTime.Now.AddYears(-19),
                PlanId = 1,
                Password = "@Password123",
                PasswordConfirm = "@Password123"
            };
            var result = _validator.Validate(signUp);

            result.Should().NotBeNull()
                .And.Match<ValidationResult>(r => r.IsValid);
            result.Errors.Should().HaveCount(0);
        }
    }
}
