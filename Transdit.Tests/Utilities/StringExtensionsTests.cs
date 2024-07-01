using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Utilities.Extensions;

namespace Transdit.Tests.Utilities
{
    [TestFixture]
    public class StringExtensionsTests
    {
        Fixture _fixture;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _fixture = new Fixture();
        }

        [Test]
        [TestCase(0, 2, '*', ExpectedResult = "**ssword123")]
        [TestCase(8, 11, '#', ExpectedResult = "password###")]
        [TestCase(8, 14, '#', ExpectedResult = "password123")]
        [TestCase(-5, 2, 'B', ExpectedResult = "password123")]
        public string Mask_(int de, int ate, char subst)
        {
            string password = "password123";
            
            return password.Mask(de, ate, subst);
        }
    }
}
