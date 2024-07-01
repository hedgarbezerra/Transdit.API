using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;
using Transdit.Core.Domain;

namespace Transdit.Repository.Mappings
{
    [ExcludeFromCodeCoverage]
    internal class TranscriptionEntityMap : EntityMapper<Transcription>
    {
        protected override void AsPrimaryKey(EntityTypeBuilder<Transcription> builder)
        {
            builder.HasKey(p => p.Id);
        }

        protected override void ToTable(EntityTypeBuilder<Transcription> builder)
        {
            builder.ToTable("TbTranscriptions");
        }

        protected override void WithForeignKey(EntityTypeBuilder<Transcription> builder)
        {
        }

        protected override void WithProperties(EntityTypeBuilder<Transcription> builder)
        {
            builder.Property(p => p.Name).HasColumnType("varchar(255)");
            builder.Property(p => p.Language).HasColumnType("varchar(20)");
            builder.Property(p => p.InputedFileName).HasColumnType("varchar(max)");
            builder.Property(p => p.StorageFileName).HasColumnType("varchar(max)");
            builder.Property(p => p.Result).HasColumnType("nvarchar(max)");
            builder.Property(p => p.Usage).HasConversion<TimeSpanToTicksConverter>();
            builder.Property(p => p.Date).HasColumnType("datetime");
        }
    }
}
