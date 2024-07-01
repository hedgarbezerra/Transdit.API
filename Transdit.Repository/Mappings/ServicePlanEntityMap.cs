using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Repository.Mappings
{
    [ExcludeFromCodeCoverage]
    internal class ServicePlanEntityMap : EntityMapper<ServicePlan>
    {
        protected override void AsPrimaryKey(EntityTypeBuilder<ServicePlan> builder)
        {
        }

        protected override void ToTable(EntityTypeBuilder<ServicePlan> builder)
        {
        }

        protected override void WithForeignKey(EntityTypeBuilder<ServicePlan> builder)
        {
        }

        protected override void WithProperties(EntityTypeBuilder<ServicePlan> builder)
        {
            builder.Property(p => p.Description).HasColumnType("varchar(max)");
            builder.Property(p => p.Price).HasColumnType("decimal").HasPrecision(11, 3);
            builder.Property(p => p.MonthlyLimitUsage).HasConversion<TimeSpanToTicksConverter>(); 
            builder.Property(p => p.Maturity).HasConversion<TimeSpanToTicksConverter>(); 
            builder.Property(p => p.AllowTranscriptionSaving).HasColumnType("bit");

            builder.Ignore(p => p.Plan);
        }
    }
}
