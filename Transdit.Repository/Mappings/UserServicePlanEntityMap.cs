using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Repository.Mappings
{
    internal class UserServicePlanEntityMap : EntityMapper<ApplicationUserPlan>
    {
        protected override void AsPrimaryKey(EntityTypeBuilder<ApplicationUserPlan> builder)
        {
        }

        protected override void ToTable(EntityTypeBuilder<ApplicationUserPlan> builder)
        {
        }

        protected override void WithForeignKey(EntityTypeBuilder<ApplicationUserPlan> builder)
        {
        }

        protected override void WithProperties(EntityTypeBuilder<ApplicationUserPlan> builder)
        {
            builder.Property(p => p.Maturity).HasColumnType("datetime");
            builder.Property(p => p.IsActive).HasColumnType("bit");
        }
    }
}
