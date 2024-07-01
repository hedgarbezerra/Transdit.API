using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Repository.Repositories
{
    public class TranscriptionsRepository : BaseRepository<Transcription>
    {
        public TranscriptionsRepository(SqlIdentityContext dbContext) : base(dbContext)
        {
        }
    }
}
