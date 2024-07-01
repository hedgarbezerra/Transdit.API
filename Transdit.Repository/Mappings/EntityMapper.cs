using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Repository.Mappings
{
    [ExcludeFromCodeCoverage]
    internal abstract class EntityMapper<T> : IEntityTypeConfiguration<T> where T : class
    {
        protected abstract void ToTable(EntityTypeBuilder<T> builder);
        protected abstract void AsPrimaryKey(EntityTypeBuilder<T> builder);
        protected abstract void WithForeignKey(EntityTypeBuilder<T> builder);
        protected abstract void WithProperties(EntityTypeBuilder<T> builder);

        public void Configure(EntityTypeBuilder<T> builder)
        {
            ToTable(builder);
            AsPrimaryKey(builder);
            WithForeignKey(builder);
            WithProperties(builder);
        }
    }
}
