using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Domain
{
    public class ApplicationUserPlan : IdentityUserRole<int>
    {
        public bool IsActive { get; set; }
        public DateTime Maturity { get; set; }
    }
}
