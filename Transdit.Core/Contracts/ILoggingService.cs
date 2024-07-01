using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models;

namespace Transdit.Core.Contracts
{
    public interface ILoggingService
    {
        IEnumerable<LogItem> Get(DateTime begin, DateTime end);
        PaginatedList<LogItem> Get(PaginationInput pagination, string route);
    }
}
