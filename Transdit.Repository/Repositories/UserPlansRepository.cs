using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Repository.Repositories
{
    public class UserPlansRepository : BaseRepository<ApplicationUserPlan>
    {
        public UserPlansRepository(SqlIdentityContext dbContext) : base(dbContext)
        {
        }
    }
}
