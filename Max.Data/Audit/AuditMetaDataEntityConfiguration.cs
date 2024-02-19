using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Max.Data.Audit
{
    public class AuditMetaDataEntityConfiguration : IEntityTypeConfiguration<AuditMetaDataEntity>
    {
        public AuditMetaDataEntityConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<AuditMetaDataEntity> builder)
        {
            #region Configuration
            builder.ToTable("AuditMetaDatas");
            builder.HasKey(x => new { x.HashPrimaryKey, x.SchemaTable });
            builder.HasIndex(e => e.ReadablePrimaryKey, "idx_ReadablePrimaryKey");
            #endregion
        }
    }
}
