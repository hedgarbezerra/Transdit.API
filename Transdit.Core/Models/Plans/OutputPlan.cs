using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Plans
{
    public class OutputPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool AllowTranscriptionSaving { get; set; }
        public double Price { get; set; }
        public double MonthlyLimitUsageMinutes { get; set; }
    }
}
