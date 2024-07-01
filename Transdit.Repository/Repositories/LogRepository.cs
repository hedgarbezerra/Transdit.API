using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models;

namespace Transdit.Repository.Repositories
{
    public class LogRepository : BaseRepository<LogItem>
    {
        public LogRepository(SqlIdentityContext dbContext) : base(dbContext)
        {
        }
    }
}
