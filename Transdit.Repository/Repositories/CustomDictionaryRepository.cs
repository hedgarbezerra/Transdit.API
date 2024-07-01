using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Repository.Repositories
{
    public class CustomDictionaryRepository : BaseRepository<CustomDictionary>
    {
        public CustomDictionaryRepository(SqlIdentityContext dbContext) : base(dbContext)
        {
        }
    }
}
