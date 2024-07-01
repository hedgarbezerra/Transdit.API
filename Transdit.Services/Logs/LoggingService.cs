using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models;
using Transdit.Core.Models.Pagination;
using Transdit.Repository.Repositories;

namespace Transdit.Services.Logs
{
    public class LoggingService : ILoggingService
    {
        private readonly IRepository<LogItem> _repository;
        private readonly IUriService _uriService;

        public LoggingService(IRepository<LogItem> repository, IUriService uriService)
        {
            _repository = repository;
            _uriService = uriService;
        }


        public PaginatedList<LogItem> Get(PaginationInput pagination, string route)
        {
            var query = string.IsNullOrEmpty(pagination.SearchTerm) ?
                _repository.Get() :
                _repository.Get(l => l.Message.Contains(pagination.SearchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    l.Exception.Contains(pagination.SearchTerm, StringComparison.InvariantCultureIgnoreCase));
            var paginatedUsers = new PaginatedList<LogItem>(query, _uriService, route, pagination.Index, pagination.Size);

            return paginatedUsers;
        }
        public IEnumerable<LogItem> Get(DateTime begin, DateTime end)
        {
            if((begin == DateTime.MinValue || end == DateTime.MinValue) || (begin == DateTime.MaxValue || end == DateTime.MaxValue))
                return Enumerable.Empty<LogItem>();

            if(begin > end)
            {
                DateTime tempDate = begin;
                begin = end;
                end = tempDate;
            }

            var query = _repository.Get(l => l.CreatedTime.Date >= begin.Date && l.CreatedTime.Date <= end.Date);

            return query.ToList();
        }
    }
}
