using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models;

namespace Transdit.Tests.Core.Models
{
    [TestFixture]
    internal class PlanEnumerationTestes
    {

        [Test]
        public void GetAll()
        {
            var plans = Enumeration.GetAll<PlanEnumeration>();

            plans.Should().NotBeEmpty().And.HaveCount(6);
        }

        [Test]
        public void Get()
        {

        }

        [Test]
        public void EqualsOverride_EqualToNull_False()
        {
            var freePlan = PlanEnumeration.Free;

            freePlan.Equals(null).Should().BeFalse();
        }

        [Test]
        public void EqualsOverride_DifferentPlans_False()
        {
            var freePlan = PlanEnumeration.Free;
            var PayAsGoPlan = PlanEnumeration.PayAsGo;

            freePlan.Equals(PayAsGoPlan).Should().BeFalse();
        }
        
        [Test]
        public void EqualsOverride_SamePlan_True()
        {
            var freePlan = PlanEnumeration.Free;
            var freeAnotherPlan = PlanEnumeration.Free;

            freePlan.Equals(freeAnotherPlan).Should().BeTrue();
        }

        [Test]
        [TestCase(1, "Free")]
        [TestCase(2, "Basic")]
        [TestCase(3, "Common")]
        [TestCase(6, "Administrator")]
        public void ToStringWithFromValue(int id, string expectedDisplay)
        {
            var plan = Enumeration.FromValue<PlanEnumeration>(id);

            plan.Should().NotBeNull();
            plan.ToString().Should().Be(expectedDisplay);
        }

        [Test]
        [TestCase(1, "Free")]
        [TestCase(2, "Basic")]
        [TestCase(3, "Common")]
        [TestCase(6, "Administrator")]
        public void FromDisplayName(int id, string expectedDisplay)
        {
            var plan = Enumeration.FromDisplayName<PlanEnumeration>(expectedDisplay);

            plan.Should().NotBeNull();
            plan.Value.Should().Be(id);
        }

        [Test]
        [TestCase(1, 2, ExpectedResult = 1)]
        [TestCase(3, 1, ExpectedResult = 2)]
        [TestCase(6, 2, ExpectedResult = 4)]
        public int AbsoluteDif(int fst, int scdn)
        {
            var fstPlan = Enumeration.FromValue<PlanEnumeration>(fst);
            var scdnPlan = Enumeration.FromValue<PlanEnumeration>(scdn);

           return Enumeration.AbsoluteDifference(fstPlan, scdnPlan);
        }
    }
}
