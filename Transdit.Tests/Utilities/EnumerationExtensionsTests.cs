using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Utilities.Extensions;

namespace Transdit.Tests.Utilities
{
    [TestFixture]
    public class EnumerationExtensionsTests
    {
        private Fixture _fixture;

        [OneTimeSetUp] public void SetUpFixture()
        {
            _fixture = new Fixture();
        }

        [Test]
        [TestCase(3)]
        [TestCase(1)]
        public void AddIdentityErrors_ExistsErrors_TargetListShouldhaveAllErros(int errorCount) 
        {
            var listaAlvo = new List<string>();
            var errors = _fixture.Build<IdentityError>()
                .With(p => p.Description, "error message").CreateMany(errorCount);

            listaAlvo.AddIdentityErrors(errors);

            listaAlvo.Should().NotBeEmpty().And.HaveCount(errorCount).And.Match(p => p.All(msg => msg.Equals("error message")));
        }

        [Test]
        public void AddIdentityErrors_NoIdentityErrors_TargetListShouldBeEmpty()
        {
            var listaAlvo = new List<string>();
            var errors = _fixture.Build<IdentityError>()
                .With(p => p.Description, "error message").CreateMany(0);

            listaAlvo.AddIdentityErrors(errors);

            listaAlvo.Should().BeEmpty();
        }


        [Test]
        public void AddIdentityErrors_NullIdentityErrors_TargetListShouldBeEmpty()
        {
            var listaAlvo = new List<string>();
            var errors = _fixture.Build<IdentityError>()
                .With(p => p.Description, "error message").CreateMany(0);

            listaAlvo.AddIdentityErrors(errors);

            listaAlvo.Should().BeEmpty();
        }

        [Test]
        public void AddIdentityErrors_NullTargetList_TargetListShouldNull()
        {
            List<string> listaAlvo = null;
            var errors = _fixture.Build<IdentityError>()
                .With(p => p.Description, "error message").CreateMany(0);

            listaAlvo.AddIdentityErrors(errors);

            listaAlvo.Should().BeNull();
        }



        [Test]
        [TestCase(3)]
        [TestCase(1)]
        public void AddValidationErrors_ExistsErrors_TargetListShouldhaveAllErros(int errorCount)
        {
            var listaAlvo = new List<string>();
            var errors = _fixture.Build<ValidationFailure>()
                .With(p => p.ErrorMessage, "error message").CreateMany(errorCount);

            listaAlvo.AddValidationErrors(errors);

            listaAlvo.Should().NotBeEmpty().And.HaveCount(errorCount).And.Match(p => p.All(msg => msg.Equals("error message")));
        }

        [Test]
        public void AddValidationErrors_NoValidationFailures_TargetListShouldBeEmpty()
        {
            var listaAlvo = new List<string>();
            var errors = _fixture.Build<ValidationFailure>()
                .With(p => p.ErrorMessage, "error message").CreateMany(0);

            listaAlvo.AddValidationErrors(errors);

            listaAlvo.Should().BeEmpty();
        }


        [Test]
        public void AddValidationErrors_NullValidationFailures_TargetListShouldBeEmpty()
        {
            var listaAlvo = new List<string>();
            var errors = _fixture.Build<ValidationFailure>()
                .With(p => p.ErrorMessage, "error message").CreateMany(0);

            listaAlvo.AddValidationErrors(errors);

            listaAlvo.Should().BeEmpty();
        }

        [Test]
        public void AddValidationErrors_NullTargetList_TargetListShouldNull()
        {
            List<string> listaAlvo = null;
            var errors = _fixture.Build<ValidationFailure>()
                .With(p => p.ErrorMessage, "error message").CreateMany(0);

            listaAlvo.AddValidationErrors(errors);

            listaAlvo.Should().BeNull();
        }


        [Test]
        public void ForEachNext()
        {
            var list = new List<int>() { 1, 2, 3, 4 };
            var resultList = new List<int>();
            list.ForEachNext((q, b) => resultList.Add(q + b));

            resultList.Should().NotBeNullOrEmpty().And.HaveCount(list.Count - 1);
        }
    }
}
