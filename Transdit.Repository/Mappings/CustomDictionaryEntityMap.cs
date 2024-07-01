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
    internal class CustomDictionaryEntityMap : EntityMapper<CustomDictionary>
    {
        protected override void AsPrimaryKey(EntityTypeBuilder<CustomDictionary> builder)
        {
            builder.HasKey(x => x.Id);
        }

        protected override void ToTable(EntityTypeBuilder<CustomDictionary> builder)
        {
            builder.ToTable("TbCustomDictionary");
        }

        protected override void WithForeignKey(EntityTypeBuilder<CustomDictionary> builder)
        {
            builder.OwnsMany(q => q.Words,
                w =>
                {
                    w.ToTable("tbCustomDictionaryWord");
                    w.HasKey(wo => wo.Id);
                    w.WithOwner()
                    .HasForeignKey(x => x.IdDictionary);

                    w.Property(wp => wp.Word).HasColumnType("varchar(50)");
                });
        }

        protected override void WithProperties(EntityTypeBuilder<CustomDictionary> builder)
        {
            builder.Property(p => p.Name)
                .HasColumnType("varchar(100)");

            builder.Property(p => p.Description)
                .HasColumnType("varchar(255)")
                .IsRequired(false);

            builder.Property(p => p.Date)
                .HasColumnType("datetime");
        }
    }
}
