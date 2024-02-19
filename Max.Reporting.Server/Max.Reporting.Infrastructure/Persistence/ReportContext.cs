using Max.Reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Max.Reporting.Infrastructure.Persistence
{
    public partial class ReportContext : DbContext
    {
        
        private readonly IDataProtectorExtention _dataProtector;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        public ReportContext()
        {
        }

        public ReportContext(DbContextOptions<ReportContext> options, IMemoryCache memoryCache, IDataProtectorExtention dataProtector,IConfiguration configuration)
            : base(options)
        {
            _dataProtector = dataProtector;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public virtual DbSet<TrApplock> TrApplocks { get; set; }
        public virtual DbSet<TrCategory> TrCategories { get; set; }
        public virtual DbSet<TrDefinition> TrDefinitions { get; set; }
        public virtual DbSet<TrObject> TrObjects { get; set; }
        public virtual DbSet<TrReport> TrReports { get; set; }
        public virtual DbSet<TrSet> TrSets { get; set; }
        public virtual DbSet<TrString> TrStrings { get; set; }
        public virtual DbSet<TrTemplate> TrTemplates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL(_configuration.GetConnectionString("Default"));

            }
            var tenantId = _memoryCache.Get<string>("TenantId");
            if (!string.IsNullOrEmpty(tenantId))
            {
                var tenantCN = _memoryCache.Get<string>($"CN-{tenantId}");
                if (!string.IsNullOrEmpty(tenantCN))
                {
                    optionsBuilder.UseMySQL(_dataProtector.Decrypt(tenantCN));
                }
                
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrApplock>(entity =>
            {
                entity.ToTable("tr_applock");
            });

            modelBuilder.Entity<TrCategory>(entity =>
            {
                entity.ToTable("tr_category");

                entity.HasIndex(e => e.Name, "IX_tr_category_Name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TrDefinition>(entity =>
            {
                entity.ToTable("tr_definition");

                entity.HasIndex(e => e.ReportId, "ReportId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnName("definition");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.TrDefinitions)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("tr_definition_ibfk_1");
            });

            modelBuilder.Entity<TrObject>(entity =>
            {
                entity.ToTable("tr_object");

                entity.Property(e => e.Value).IsRequired();
            });

            modelBuilder.Entity<TrReport>(entity =>
            {
                entity.ToTable("tr_report");

                entity.HasIndex(e => e.CategoryId, "IX_tr_report_CategoryId");

                entity.HasIndex(e => e.Name, "IX_tr_report_Name")
                    .IsUnique();

                entity.HasIndex(e => e.TemplateId, "IX_tr_report_TemplateId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Bookmark).HasColumnType("bit(1)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TemplateId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.TrReports)
                    .HasForeignKey(d => d.CategoryId);

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TrReports)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("tr_templateid_ibfk_1");
            });

            modelBuilder.Entity<TrSet>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Member })
                    .HasName("PRIMARY");

                entity.ToTable("tr_set");
            });

            modelBuilder.Entity<TrString>(entity =>
            {
                entity.ToTable("tr_string");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TrTemplate>(entity =>
            {
                entity.ToTable("tr_template");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnName("definition");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
