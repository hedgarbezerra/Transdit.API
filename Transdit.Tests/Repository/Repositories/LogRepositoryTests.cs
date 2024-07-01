using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models;
using Transdit.Repository;
using Transdit.Repository.Repositories;

namespace Transdit.Tests.Repository.Repositories
{
    /// <summary>
    /// Considerando que todos repositórios herdam do base, não se faz necessário criar testes de cada entidade
    /// </summary>
    [TestFixture]
    public class LogRepositoryTests
    {
        private SqlIdentityContext _context;
        private LogRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SqlIdentityContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;

            _context = new SqlIdentityContext(options);
            _repo = new LogRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.ChangeTracker.Clear();
            _context?.Database.EnsureDeleted();
        }

        [Test]
        public void Get_AllEventLogs_ReturnsEventLogs()
        {
            AddLogsToContext();
            var logs = _repo.Get();

            Assert.That(logs, Is.Not.Null.And.Not.Empty);
            Assert.That(logs.Count() >= 2);
        }

        [Test]
        
        public void Get_AllEventLogs_EmptyIqueryable()
        {
            var logs = _repo.Get();

            Assert.That(logs, Is.Not.Null.And.Empty);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Get_FilteredById_ReturnsSingleEventLog(int id)
        {
            AddLogsToContext();
            var log = _repo.Get(id);

            Assert.That(log, Is.Not.Null);
            Assert.That(log.Id, Is.EqualTo(id));
        }

        [Test]
        [TestCase(5)]
        [TestCase(8)]
        public void Get_FilteredById_ReturnsNull(int id)
        {
            AddLogsToContext();
            var logs = _repo.Get(id);

            Assert.That(logs, Is.Null);
        }

        [Test]
        public void Get_FilteredByDelegate_ReturnsFilteredEventLogs()
        {
            AddLogsToContext();
            var logs = _repo.Get(x => x.CreatedTime < DateTime.Now);

            Assert.That(logs, Is.Not.Null.And.Not.Empty);
            Assert.That(logs.Count() >= 2);
        }

        [Test]
        public void Get_FilteredByDelegate_ReturnsEmptyQueryable()
        {
            AddLogsToContext();
            var logs = _repo.Get(x => x.CreatedTime > DateTime.Now);

            Assert.That(logs, Is.Not.Null.And.Empty);
        }

        [Test]
        public void Get_FilteredAndOrderedByRegisteredDate_ReturnsOrderedRecordsOldestToNewest()
        {
            AddLogsToContext();
            var logs = _repo.Get(x => x.CreatedTime < DateTime.Now, c => c.CreatedTime);
            var oldLog = logs.First();

            Assert.That(logs, Is.Not.Null.And.Not.Empty);            
            Assert.That(oldLog, Is.Not.Null);
            Assert.That(oldLog.Id, Is.EqualTo(1));
        }

        [Test]
        public void Get_FilteredAndOrderedByRegisteredDate_ReturnsOrderedRecordsNewestToOldest()
        {
            AddLogsToContext();
            var logs = _repo.Get(x => x.CreatedTime < DateTime.Now, c => c.CreatedTime, reverse: true);
            var oldLog = logs.First();

            Assert.That(logs, Is.Not.Null.And.Not.Empty);            
            Assert.That(oldLog, Is.Not.Null);
            Assert.That(oldLog.Id, Is.EqualTo(2));
        }

        [Test]
        public void Get_FilteredAndOrderedWithLimitAndSkippedRecords_ReturnsNoRecord()
        {
            AddLogsToContext();
            var logs = _repo.Get(x => x.CreatedTime < DateTime.Now, c => c.CreatedTime, 1, 1, true);
            var oldLog = logs.FirstOrDefault();

            Assert.That(logs, Is.Not.Null.And.Empty);
            Assert.That(oldLog, Is.Null);
        }

        [Test]
        public void Get_FilteredAndOrderedWithLimitAndSkippedRecords_ReturnsSingeRecord()
        {
            AddLogsToContext();
            var logs = _repo.Get(x => x.CreatedTime < DateTime.Now, c => c.CreatedTime, 2, 1, true);
            var log = logs.FirstOrDefault();

            Assert.That(logs, Is.Not.Null.And.Not.Empty);            
            Assert.IsNotNull(log);
            Assert.That(log.Id == 1);
        }

        [Test]
        public void Get_FilteredAndOrderedWithNoMatches_ReturnsEmpty()
        {
            AddLogsToContext();
            var logs = _repo.Get(x => x.CreatedTime > DateTime.Now, c => c.CreatedTime);

            Assert.That(logs, Is.Not.Null.And.Empty);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Delete_DeletingEventLogs_EventLogDeletedFromContext(int id)
        {
            AddLogsToContext();
            _repo.Delete(id);
            _repo.SaveChanges();

            Assert.That(!_context.Logs.Any(x => x.Id == id));
        }

        [Test]
        public void GetDbContext_ReturnsCurrentDbContext()
        {
            var dbContext = _repo.GetDbContext();

            Assert.IsNotNull(dbContext);
            Assert.IsInstanceOf<SqlIdentityContext>(dbContext);
        }

        [Test]
        public void Add_EventLogAdded()
        {
            var log = new LogItem { Message = "New log", Exception = "roleEventLog", CreatedTime = new DateTime(2021, 08, 29) };
            var addedEventLog = _repo.Add(log);
            _repo.SaveChanges();
            var logs = _repo.Get();
            Assert.That(logs.Count() == 1);
            Assert.That(logs.First(), Is.EqualTo(addedEventLog));
        }


        private void AddLogsToContext()
        {
            _context.Logs.Add(new LogItem { Id = 1, Message = "Error 1", CreatedTime = new DateTime(2021, 5, 10) });
            _context.Logs.Add(new LogItem { Id = 2, Message = "Error 2", CreatedTime = new DateTime(2021, 8, 10) });
            _context.SaveChanges();
        }
    }
}
