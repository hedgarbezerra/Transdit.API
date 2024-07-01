using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Users
{
    public class UserDataComponents
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public DateTime PlanMaturity { get; set; }
        public TimeSpan TimeConsumed { get; set; }
        public TimeSpan TimeAllowed { get; set; }
        public bool PlanIsActive { get; set; }
        public bool PlanAllowSaving { get; set; }
    }
}
