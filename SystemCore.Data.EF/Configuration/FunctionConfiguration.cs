using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemCore.Data.EF.Extensions;
using SystemCore.Data.Entities;

namespace SystemCore.Data.EF.Configuration
{
    public class FunctionConfiguration : DbEntityConfiguration<Function>
    {
        public override void Configure(EntityTypeBuilder<Function> entity)
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasMaxLength(128).IsRequired().HasColumnName("varchar(128)");
            // etc.
        }
    }
}
