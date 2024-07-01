using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Users;
using Transdit.Repository.Repositories;
using Transdit.Services.Emails;
using Transdit.Services.Logs;

namespace Transdit.Tests.Services.Logs
{
    [TestFixture]
    public class LoggingServiceTests
    {
        private Fixture _fixture;

        [OneTimeSetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        [TestCase(5, 1, 5, 1)]
        [TestCase(2, 2, 5, 2)]
        [TestCase(2, 2, 3, 1)]
        public void Get_FilterPaginationNoFilter(int index, int pageSize, int expectedCount, int expectedDataCount)
        {
            var mqUriService =  new Mock<IUriService>();
            var mqRepository =  new Mock<IRepository<LogItem>>();
            mqRepository.Setup(mq => mq.Get()).Returns(_fixture.CreateMany<LogItem>(expectedCount).AsQueryable());
            var logService = new LoggingService(mqRepository.Object, mqUriService.Object);
            var result = logService.Get(new PaginationInput(index, pageSize), string.Empty);

            result.Should().NotBeNull();
            result.Data.Should().NotBeNullOrEmpty().And.HaveCount(expectedDataCount);
            result.TotalCount.Should().Be(expectedCount);
        }

        [Test]
        public void Get_BetweenDates_DatesNullOrMinDate()
        {
            var mqUriService = new Mock<IUriService>();
            var mqRepository = new Mock<IRepository<LogItem>>();
            var logService = new LoggingService(mqRepository.Object, mqUriService.Object);

            var result = logService.Get(DateTime.MinValue, DateTime.MaxValue);

            result.Should().BeEmpty();
        }

        [Test]
        public void Get_BetweenDates()
        {
            var mqUriService = new Mock<IUriService>();
            var mqRepository = new Mock<IRepository<LogItem>>();
            mqRepository.Setup(mq => mq.Get(It.IsAny<Expression<Func<LogItem, bool>>>())).Returns(_fixture.CreateMany<LogItem>(2).AsQueryable());
            var logService = new LoggingService(mqRepository.Object, mqUriService.Object);

            var result = logService.Get(DateTime.Now.AddDays(3), DateTime.Now.AddDays(-1));

            result.Should().NotBeEmpty().And.HaveCount(2);
        }
    }
}
