using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Pagination;

namespace Transdit.Tests.Core.Models.Pagination
{
    [TestFixture]
    public class PaginatedListTests
    {
        private MockRepository _mockRepo;
        private Fixture _fixture;

        [OneTimeSetUp] 
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Default);
            _fixture = new Fixture();
        }

        [Test]
        [TestCase(5, 2, 1, true, true)]
        [TestCase(2, 2, 1, false, true)]
        [TestCase(5, 1, 2, true, false)]
        [TestCase(0, 1, 1, false, false)]
        public void PaginatedList_PaginatedItens_MatchConditions(int itemNumber, int pageIndex, int pageSize, bool expectedToHaveNextPage, bool expectedToHavePreviousPage)
        {
            var mqUriService = MockUriService();
            var query = GetMockedQueryable(itemNumber);
            var paginatedList = new PaginatedList<ObjetoTestes>(query, mqUriService.Object, string.Empty, pageIndex, pageSize);

            Assert.That(paginatedList.PageSize, Is.EqualTo(pageSize));
            Assert.That(paginatedList.TotalCount, Is.EqualTo(itemNumber));
            Assert.That(paginatedList.PageIndex, Is.EqualTo(pageIndex));
            Assert.That(paginatedList.HasNextPage, Is.EqualTo(expectedToHaveNextPage));
            Assert.That(paginatedList.HasPreviousPage, Is.EqualTo(expectedToHavePreviousPage));
        }

        private IQueryable<ObjetoTestes> GetMockedQueryable(int count) => _fixture.CreateMany<ObjetoTestes>(count).AsQueryable();

        private Mock<IUriService> MockUriService()
        {
            var uriService = _mockRepo.Create<IUriService>();
            uriService.Setup(mq => mq.GetPageUri(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new Uri("http://localhost.com"));

            return uriService;
        }
        private class ObjetoTestes
        {
            public int Id { get; set; }
        }
    }
}
