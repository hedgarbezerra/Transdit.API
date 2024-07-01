using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models;

namespace Transdit.Repository.Mappings
{
    [ExcludeFromCodeCoverage]
    internal class LogItemEntityMap : EntityMapper<LogItem>
    {
        protected override void AsPrimaryKey(EntityTypeBuilder<LogItem> builder)
        {
            builder.HasKey(c => c.Id);
        }

        protected override void ToTable(EntityTypeBuilder<LogItem> builder)
        {
            builder.ToTable("SerilogLoggingTable", tb => tb.ExcludeFromMigrations(true));
        }

        protected override void WithForeignKey(EntityTypeBuilder<LogItem> builder)
        {
        }

        protected override void WithProperties(EntityTypeBuilder<LogItem> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnType("int")
                .HasColumnName("Id");

            builder.Property(c => c.Message)
                .HasColumnType("nvarchar")
                .HasColumnName("Message").IsRequired(false);

            builder.Property(c => c.MessageTemplate)
                .HasColumnType("nvarchar")
                .HasColumnName("MessageTemplate").IsRequired(false);

            builder.Property(c => c.Exception)
                .HasColumnType("nvarchar")
                .HasColumnName("Exception").IsRequired(false);

            builder.Property(c => c.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("TimeStamp");

            builder.Property(c => c.LogLevel)
                .HasColumnType("nvarchar")
                .HasColumnName("Level");

            builder.Property(c => c.Properties)
                .HasColumnType("nvarchar")
                .HasColumnName("Properties").IsRequired(false);

            builder.Ignore(c => c.XmlContent);
        }
    }
}
