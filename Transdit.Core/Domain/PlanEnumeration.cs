using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models;

namespace Transdit.Core.Domain
{
    public class PlanEnumeration : Enumeration
    {
        public PlanEnumeration() { }
        protected PlanEnumeration(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly PlanEnumeration Free = new FreePlan();
        public static readonly PlanEnumeration Basic = new BasicPlan();
        public static readonly PlanEnumeration Standard = new StandardPlan();
        public static readonly PlanEnumeration Premium = new PremiumPlan();
        public static readonly PlanEnumeration PayAsGo = new PayAsGoPlan();
        public static readonly PlanEnumeration Administrator = new AdministratorPlan();

        protected double MinutePrice { get => 0.2357736; }

        public virtual double CalculateCosts(TimeSpan totalUse, ServicePlan selectedPlan) => selectedPlan.Price;
        public virtual bool PlanReachedLimit(TimeSpan totalUse, ServicePlan selectedPlan) => totalUse > selectedPlan.MonthlyLimitUsage;
        public bool PlanOverdue(DateTime planMaturity) => DateTime.Now > planMaturity;

        private class FreePlan : PlanEnumeration
        {
            public FreePlan() : base(1, "Free") { }
        }
        private class BasicPlan : PlanEnumeration
        {
            public BasicPlan() : base(2, "Basic") { }
        }
        private class StandardPlan : PlanEnumeration
        {
            public StandardPlan() : base(3, "Common") { }
        }
        private class PremiumPlan : PlanEnumeration
        {
            public PremiumPlan() : base(4, "Premium") { }
        }
        private class PayAsGoPlan : PlanEnumeration
        {
            public PayAsGoPlan() : base(5, "PayAsGo") { }

            public override double CalculateCosts(TimeSpan totalUse, ServicePlan selectedPlan) => totalUse.Minutes * MinutePrice;
        }
        private class AdministratorPlan : PlanEnumeration
        {
            public AdministratorPlan() : base(6, "Administrator") { }
        }
    }
}
