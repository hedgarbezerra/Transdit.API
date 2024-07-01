using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Transdit.Services.Common;

namespace Transdit.Tests.Services.Common
{
    [TestFixture]
    public class UriServiceTests
    {
        [Test]
        [TestCase("/users", "http://localhost.com/users")]
        [TestCase("/users/1", "http://localhost.com/users/1")]
        [TestCase("/users/roles/1", "http://localhost.com/users/roles/1")]
        public void GetUri_DoThis_ExpectThat(string route, string expectedUrl)
        {
            var service = new UriService("http://localhost.com");
            var uri = service.GetUri(route);

            Assert.IsNotNull(uri);
            Assert.That(uri.ToString(), Is.EqualTo(expectedUrl));
        }

        [Test]
        [TestCase(1, 2, "/users")]
        [TestCase(5, 1, "/roles")]
        public void GetPageUri(int pageIndex, int pageSize, string route)
        {
            var service = new UriService("http://localhost.com");
            var uri = service.GetPageUri(pageIndex, pageSize, route);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            Assert.That(uri.PathAndQuery, Is.EqualTo($"{route}?pageIndex={pageIndex}&pageSize={pageSize}"));
            Assert.IsNotNull(queryParams);
            Assert.IsTrue(queryParams.Count == 2);

            var pageIndexParam = queryParams.Get(nameof(pageIndex));
            Assert.IsNotNull(pageIndexParam);
            Assert.That(pageIndex, Is.EqualTo(int.Parse(pageIndexParam)));

            var pageSizeParam = queryParams.Get(nameof(pageSize));
            Assert.IsNotNull(pageSizeParam);
            Assert.That(pageSize, Is.EqualTo(int.Parse(pageSizeParam)));
        }
    }
}
