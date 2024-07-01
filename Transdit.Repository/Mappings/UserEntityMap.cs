using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Transdit.Core.Domain;

namespace Transdit.Repository.Mappings
{
    [ExcludeFromCodeCoverage]
    internal class UserEntityMap : EntityMapper<ApplicationUser>
    {
        protected override void AsPrimaryKey(EntityTypeBuilder<ApplicationUser> builder)
        {
        }

        protected override void ToTable(EntityTypeBuilder<ApplicationUser> builder)
        {
        }

        protected override void WithForeignKey(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(p => p.Transcriptions).WithOne(c => c.User).HasForeignKey(tr => tr.UserId).IsRequired();
            builder.HasMany(p => p.CustomDictionaries).WithOne(c => c.User).HasForeignKey(tr => tr.UserId).IsRequired();
        }

        protected override void WithProperties(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(p => p.Name).HasColumnType("varchar(360)").IsRequired();
            builder.Property(p => p.TermsAgreed);
            builder.Property(p => p.DateAdded).HasColumnType("datetime");
            builder.Property(p => p.BirthDate).HasColumnType("datetime");
        }
    }
}
