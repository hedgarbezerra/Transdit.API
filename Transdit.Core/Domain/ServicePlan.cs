using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models;

namespace Transdit.Core.Domain
{
    public class ServicePlan : IdentityRole<int>
    {
        public ServicePlan() { }

        public ServicePlan(string name, string description, bool allowTranscriptionSaving, double price, TimeSpan monthlyLimitUsage, TimeSpan maturity) : base(name)
        {
            Description = description;
            AllowTranscriptionSaving = allowTranscriptionSaving;
            Price = price;
            MonthlyLimitUsage = monthlyLimitUsage;
            Maturity = maturity;
        }
        public ServicePlan(string name, string description, bool allowTranscriptionSaving, double price, TimeSpan monthlyLimitUsage) : base(name)
        {
            Description = description;
            AllowTranscriptionSaving = allowTranscriptionSaving;
            Price = price;
            MonthlyLimitUsage = monthlyLimitUsage;
            Maturity = TimeSpan.FromDays(30);
        }

        public string Description { get; set; } = string.Empty;
        public bool AllowTranscriptionSaving { get; set; }
        public double Price { get; set; }
        public TimeSpan MonthlyLimitUsage { get; set; }
        public TimeSpan Maturity { get; set; }

        public virtual PlanEnumeration Plan { get => Enumeration.FromValue<PlanEnumeration>(Id); }
    }    
}
