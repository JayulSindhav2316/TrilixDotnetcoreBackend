using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class membermaxContext : DbContext
    {
        private readonly Tenant _tenant;
        public membermaxContext()
        {
        }

        public membermaxContext(DbContextOptions<membermaxContext> options, IHttpContextAccessor httpContextAccessor)
             : base(options)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                _tenant = (Tenant)httpContextAccessor.HttpContext.Items["Tenant"];
            }
            else if (TenantManager.OrganizationName != null)
            {
                _tenant = TenantManager.GetTenantByOrganizationName(TenantManager.OrganizationName);
            }
        }

        public virtual DbSet<Accesslog> Accesslogs { get; set; }
        public virtual DbSet<Accesstoken> Accesstokens { get; set; }
        public virtual DbSet<Accountingsetup> Accountingsetups { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Autobillingdraft> Autobillingdrafts { get; set; }
        public virtual DbSet<Autobillingjob> Autobillingjobs { get; set; }
        public virtual DbSet<Autobillingnotification> Autobillingnotifications { get; set; }
        public virtual DbSet<Autobillingonhold> Autobillingonholds { get; set; }
        public virtual DbSet<Autobillingpayment> Autobillingpayments { get; set; }
        public virtual DbSet<Autobillingprocessingdate> Autobillingprocessingdates { get; set; }
        public virtual DbSet<Autobillingsetting> Autobillingsettings { get; set; }
        public virtual DbSet<Billingbatch> Billingbatches { get; set; }
        public virtual DbSet<Billingcycle> Billingcycles { get; set; }
        public virtual DbSet<Billingdocument> Billingdocuments { get; set; }
        public virtual DbSet<Billingemail> Billingemails { get; set; }
        public virtual DbSet<Billingfee> Billingfees { get; set; }
        public virtual DbSet<Billingjob> Billingjobs { get; set; }
        public virtual DbSet<Communication> Communications { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Credittransaction> Credittransactions { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<Glaccount> Glaccounts { get; set; }
        public virtual DbSet<Glaccounttype> Glaccounttypes { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Invoicedetail> Invoicedetails { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Itemtype> Itemtypes { get; set; }
        public virtual DbSet<Journalentrydetail> Journalentrydetails { get; set; }
        public virtual DbSet<Journalentryheader> Journalentryheaders { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<Membershipcategory> Membershipcategories { get; set; }
        public virtual DbSet<Membershipconnection> Membershipconnections { get; set; }
        public virtual DbSet<Membershipfee> Membershipfees { get; set; }
        public virtual DbSet<Membershiphistory> Membershiphistories { get; set; }
        public virtual DbSet<Membershipperiod> Membershipperiods { get; set; }
        public virtual DbSet<Membershiptype> Membershiptypes { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Menugroup> Menugroups { get; set; }
        public virtual DbSet<Multifactorcode> Multifactorcodes { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Paperinvoice> Paperinvoices { get; set; }
        public virtual DbSet<Paymentprocessor> Paymentprocessors { get; set; }
        public virtual DbSet<Paymentprofile> Paymentprofiles { get; set; }
        public virtual DbSet<Paymenttransaction> Paymenttransactions { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Persontag> Persontags { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }
        public virtual DbSet<Promocode> Promocodes { get; set; }
        public virtual DbSet<Receiptdetail> Receiptdetails { get; set; }
        public virtual DbSet<Receiptheader> Receiptheaders { get; set; }
        public virtual DbSet<Receiptitemdetail> Receiptitemdetails { get; set; }
        public virtual DbSet<Refunddetail> Refunddetails { get; set; }
        public virtual DbSet<Relation> Relations { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Reportargument> Reportarguments { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Rolemenu> Rolemenus { get; set; }
        public virtual DbSet<Shoppingcart> Shoppingcarts { get; set; }
        public virtual DbSet<Shoppingcartdetail> Shoppingcartdetails { get; set; }
        public virtual DbSet<Staffrole> Staffroles { get; set; }
        public virtual DbSet<Staffuser> Staffusers { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Userdevice> Userdevices { get; set; }
        public virtual DbSet<YuniqlSchemaVersion> YuniqlSchemaVersions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //                optionsBuilder.UseMySql("server=localhost;port=3306;database=membermax;uid=root;password=Anit1066", Microsoft.EntityFrameworkCore.ServerVersion.FromString("5.7.34-mysql"));
            //            }

            if (_tenant != null)
            {
                optionsBuilder.UseMySql(_tenant.ConnectionString, Microsoft.EntityFrameworkCore.ServerVersion.FromString("5.7.34-mysql"));
            }

            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accesslog>(entity =>
            {
                entity.ToTable("accesslog");

                entity.Property(e => e.AccessLogId).HasColumnType("int(11)");

                entity.Property(e => e.AccessDate).HasColumnType("datetime");

                entity.Property(e => e.IpAddress)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Portal).HasColumnType("int(11)");

                entity.Property(e => e.Referrer)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserAgent)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Accesstoken>(entity =>
            {
                entity.ToTable("accesstoken");

                entity.HasIndex(e => e.UserId, "fk_accesstoken_user_id_idx");

                entity.Property(e => e.AccessTokenId).HasColumnType("int(11)");

                entity.Property(e => e.Create).HasColumnType("datetime");

                entity.Property(e => e.CreatedIp)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("CreatedIP")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Expire).HasColumnType("datetime");

                entity.Property(e => e.RefreshToken)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Revoked).HasColumnType("datetime");

                entity.Property(e => e.RevokedIp)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("RevokedIP")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Token)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Accesstokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_accesstoken_user_id");
            });

            modelBuilder.Entity<Accountingsetup>(entity =>
            {
                entity.HasKey(e => e.AccountSetupId)
                    .HasName("PRIMARY");

                entity.ToTable("accountingsetup");

                entity.HasIndex(e => e.OffLinePaymentGlAccount, "fk_as_offlinepaymentglid_idx");

                entity.HasIndex(e => e.OrganizationId, "fk_as_organizationid_idx");

                entity.HasIndex(e => e.ProcessingFeeGlAccount, "fk_as_processingfeeglid_idx");

                entity.HasIndex(e => e.SalesReturnGlAccount, "fk_as_salesreturnglid_idx");

                entity.HasIndex(e => e.OnlineCreditGlAccount, "gf_as_onlinecreditglid_idx");

                entity.Property(e => e.AccountSetupId).HasColumnType("int(11)");

                entity.Property(e => e.OffLinePaymentGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.OnlineCreditGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.ProcessingFeeGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.SalesReturnGlAccount).HasColumnType("int(11)");

                entity.HasOne(d => d.OffLinePaymentGlAccountNavigation)
                    .WithMany(p => p.AccountingsetupOffLinePaymentGlAccountNavigations)
                    .HasForeignKey(d => d.OffLinePaymentGlAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_as_offlinepaymentglid");

                entity.HasOne(d => d.OnlineCreditGlAccountNavigation)
                    .WithMany(p => p.AccountingsetupOnlineCreditGlAccountNavigations)
                    .HasForeignKey(d => d.OnlineCreditGlAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_as_onlinecreditglid");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Accountingsetups)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_as_organizationid");

                entity.HasOne(d => d.ProcessingFeeGlAccountNavigation)
                    .WithMany(p => p.AccountingsetupProcessingFeeGlAccountNavigations)
                    .HasForeignKey(d => d.ProcessingFeeGlAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_as_processingfeeglid");

                entity.HasOne(d => d.SalesReturnGlAccountNavigation)
                    .WithMany(p => p.AccountingsetupSalesReturnGlAccountNavigations)
                    .HasForeignKey(d => d.SalesReturnGlAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_as_salesreturnglid");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.HasIndex(e => e.PersonId, "fk_address_person_id_idx");

                entity.Property(e => e.AddressId).HasColumnType("int(11)");

                entity.Property(e => e.Address1)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address2)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address3)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AddressType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.City)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Country)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsPrimary).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.State)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Zip)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_address_person_id");
            });

            modelBuilder.Entity<Autobillingdraft>(entity =>
            {
                entity.ToTable("autobillingdraft");

                entity.HasIndex(e => e.MembershipId, "fk_abd_Membership_id_idx");

                entity.HasIndex(e => e.BillingDocumentId, "fk_abd_billingdocumentid_idx");

                entity.HasIndex(e => e.InvoiceId, "fk_abd_invoiceId_idx");

                entity.HasIndex(e => e.PersonId, "fk_personid_idx");

                entity.Property(e => e.AutoBillingDraftId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(15, 2);

                entity.Property(e => e.BankAccountNumber)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BankAccountType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BillingDocumentId).HasColumnType("int(11)");

                entity.Property(e => e.CardNumber)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CardType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("varchar(25)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.IsProcessed).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.NextDueDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentProfileId)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PersonName)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProfileId)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RoutingNumber)
                    .HasColumnType("varchar(9)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.BillingDocument)
                    .WithMany(p => p.Autobillingdrafts)
                    .HasForeignKey(d => d.BillingDocumentId)
                    .HasConstraintName("fk_abd_billingdocumentid");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Autobillingdrafts)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("fk_abd_invoiceId");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Autobillingdrafts)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_abd_Membership_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Autobillingdrafts)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_abd_personid");
            });

            modelBuilder.Entity<Autobillingjob>(entity =>
            {
                entity.ToTable("autobillingjob");

                entity.Property(e => e.AutoBillingJobId).HasColumnType("int(11)");

                entity.Property(e => e.AbpdId).HasColumnType("int(11)");

                entity.Property(e => e.BillingType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Create).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.InvoiceType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.JobType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.ThroughDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Autobillingnotification>(entity =>
            {
                entity.ToTable("autobillingnotification");

                entity.Property(e => e.AutoBillingNotificationId).HasColumnType("int(11)");

                entity.Property(e => e.AbpdId).HasColumnType("int(11)");

                entity.Property(e => e.BillingType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.InvoiceType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NotificationSentDate).HasColumnType("datetime");

                entity.Property(e => e.NotificationType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Autobillingonhold>(entity =>
            {
                entity.ToTable("autobillingonhold");

                entity.HasIndex(e => e.MembershipId, "fk_autobillingonhold_membershipId_idx");

                entity.HasIndex(e => e.UserId, "fk_autobillingonhold_usrrId_idx");

                entity.Property(e => e.AutoBillingOnHoldId).HasColumnType("int(11)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.Reason)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReviewDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Autobillingonholds)
                    .HasForeignKey(d => d.MembershipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_autobillingonhold_membershipId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Autobillingonholds)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_autobillingonhold_usrrId");
            });

            modelBuilder.Entity<Autobillingpayment>(entity =>
            {
                entity.ToTable("autobillingpayment");

                entity.HasIndex(e => e.AutoBillingDraftId, "fk_abp_abd_id_idx");

                entity.HasIndex(e => e.PaymentTransactionId, "fk_abp_paymentTransactionId_idx");

                entity.HasIndex(e => e.ReceiptId, "fk_abp_receiptId_idx");

                entity.Property(e => e.AutoBillingPaymentId).HasColumnType("int(11)");

                entity.Property(e => e.AutoBillingDraftId).HasColumnType("int(11)");

                entity.Property(e => e.PaymentTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.ReceiptId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.AutoBillingDraft)
                    .WithMany(p => p.Autobillingpayments)
                    .HasForeignKey(d => d.AutoBillingDraftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_abp_abd_id");

                entity.HasOne(d => d.PaymentTransaction)
                    .WithMany(p => p.Autobillingpayments)
                    .HasForeignKey(d => d.PaymentTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_abp_paymentTransactionId");

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.Autobillingpayments)
                    .HasForeignKey(d => d.ReceiptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_abp_receiptId");
            });

            modelBuilder.Entity<Autobillingprocessingdate>(entity =>
            {
                entity.HasKey(e => e.AutoBillingProcessingDatesId)
                    .HasName("PRIMARY");

                entity.ToTable("autobillingprocessingdate");

                entity.Property(e => e.AutoBillingProcessingDatesId).HasColumnType("int(11)");

                entity.Property(e => e.BillingType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsLastDayOfEffectiveDate).HasColumnType("int(11)");

                entity.Property(e => e.IsLastDayOfThroughDate).HasColumnType("int(11)");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.ThroughDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Autobillingsetting>(entity =>
            {
                entity.HasKey(e => e.AutoBillingsettingsId)
                    .HasName("PRIMARY");

                entity.ToTable("autobillingsetting");

                entity.Property(e => e.AutoBillingsettingsId).HasColumnType("int(11)");

                entity.Property(e => e.EmailForNotification)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EnableAutomatedBillingForMembership).HasColumnType("int(11)");

                entity.Property(e => e.IsPauseOrEnableForMembership).HasColumnType("int(11)");

                entity.Property(e => e.SmsforNotification)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("SMSForNotification")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Billingbatch>(entity =>
            {
                entity.ToTable("billingbatch");

                entity.HasIndex(e => e.BatchCycleId, "fk_billingbatch_cycle_id_idx");

                entity.HasIndex(e => e.MembershipTypeId, "fk_billingbatch_membership_typeId_idx");

                entity.Property(e => e.BillingBatchId).HasColumnType("int(11)");

                entity.Property(e => e.BatchCycleId).HasColumnType("int(11)");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.BatchCycle)
                    .WithMany(p => p.Billingbatches)
                    .HasForeignKey(d => d.BatchCycleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billingbatch_cycle_id");

                entity.HasOne(d => d.MembershipType)
                    .WithMany(p => p.Billingbatches)
                    .HasForeignKey(d => d.MembershipTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billingbatch_membership_typeId");
            });

            modelBuilder.Entity<Billingcycle>(entity =>
            {
                entity.ToTable("billingcycle");

                entity.Property(e => e.BillingCycleId).HasColumnType("int(11)");

                entity.Property(e => e.CycleName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RunDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.ThroughDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Billingdocument>(entity =>
            {
                entity.ToTable("billingdocument");

                entity.Property(e => e.BillingDocumentId).HasColumnType("int(11)");

                entity.Property(e => e.AbpdId).HasColumnType("int(11)");

                entity.Property(e => e.BillingType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsFinalized).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.ThroughDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Billingemail>(entity =>
            {
                entity.ToTable("billingemail");

                entity.HasIndex(e => e.BillingCycleId, "fk_billingemail_billingCycleId_idx");

                entity.HasIndex(e => e.InvoiceId, "fk_billingemail_invoiceId_idx");

                entity.Property(e => e.BillingEmailId).HasColumnType("int(11)");

                entity.Property(e => e.BillingCycleId).HasColumnType("int(11)");

                entity.Property(e => e.CartId).HasColumnType("int(11)");

                entity.Property(e => e.CommunicationDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.Response)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Token)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.BillingCycle)
                    .WithMany(p => p.Billingemails)
                    .HasForeignKey(d => d.BillingCycleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billingemail_billingCycleId");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Billingemails)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billingemail_invoiceId");
            });

            modelBuilder.Entity<Billingfee>(entity =>
            {
                entity.ToTable("billingfee");

                entity.HasIndex(e => e.MembershipFeeId, "fk_billingfee_membershipFee_id_idx");

                entity.HasIndex(e => e.MembershipId, "fk_billingfee_membership_id_idx");

                entity.Property(e => e.BillingFeeId).HasColumnType("int(11)");

                entity.Property(e => e.Discount).HasPrecision(13, 2);

                entity.Property(e => e.Fee).HasPrecision(13, 2);

                entity.Property(e => e.MembershipFeeId).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.MembershipFee)
                    .WithMany(p => p.Billingfees)
                    .HasForeignKey(d => d.MembershipFeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billingfee_membershipFee_id");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Billingfees)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_billingfee_membership_id");
            });

            modelBuilder.Entity<Billingjob>(entity =>
            {
                entity.ToTable("billingjob");

                entity.HasIndex(e => e.BillingCycleId, "fk_billingjob_billing_cycle_id_idx");

                entity.Property(e => e.BillingJobId).HasColumnType("int(11)");

                entity.Property(e => e.BillingCycleId).HasColumnType("int(11)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.BillingCycle)
                    .WithMany(p => p.Billingjobs)
                    .HasForeignKey(d => d.BillingCycleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billingjob_billing_cycle_id");
            });

            modelBuilder.Entity<Communication>(entity =>
            {
                entity.ToTable("communication");

                entity.HasIndex(e => e.PersonId, "fk_communication_person_id_idx");

                entity.HasIndex(e => e.StaffUserId, "fk_communication_staffUser_id_idx");

                entity.Property(e => e.CommunicationId).HasColumnType("int(11)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.From)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Notes)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Scheduled).HasColumnType("int(11)");

                entity.Property(e => e.StaffUserId).HasColumnType("int(11)");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Subject)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Communications)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_communication_person_id");

                entity.HasOne(d => d.StaffUser)
                    .WithMany(p => p.Communications)
                    .HasForeignKey(d => d.StaffUserId)
                    .HasConstraintName("fk_communication_staffUser_id");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.BillableContactId).HasColumnType("int(11)");

                entity.Property(e => e.City)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CompanyName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Phone)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.State)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StreetAddress)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Website)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Zip)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Credittransaction>(entity =>
            {
                entity.ToTable("credittransaction");

                entity.HasIndex(e => e.ReceiptDetailId, "fk_creditTransaction_receiptdetail_id_idx");

                entity.HasIndex(e => e.PersonId, "fk_credittransaction_personId_idx");

                entity.Property(e => e.CreditTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(13, 3);

                entity.Property(e => e.CreditGlAccount)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DebitGlAccount)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EntryType).HasColumnType("int(11)");

                entity.Property(e => e.ExpirDate).HasColumnType("datetime");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Reason)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReceiptDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Credittransactions)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_credittransaction_personId");

                entity.HasOne(d => d.ReceiptDetail)
                    .WithMany(p => p.Credittransactions)
                    .HasForeignKey(d => d.ReceiptDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_creditTransaction_receiptdetail_id");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.HasIndex(e => e.OrganizationId, "fk_department_organization_id_idx");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.CostCenterCode)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_department_organization_id");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("document");

                entity.HasIndex(e => e.OrganizationId, "fk_department_organization_id_idx");

                entity.HasIndex(e => e.PersonId, "fk_document_person_id_idx");

                entity.Property(e => e.DocumentId).HasColumnType("int(11)");

                entity.Property(e => e.ContentType)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DisplayFileName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FileName)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FilePath)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_department_organizationId_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_document_person_id");
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.ToTable("email");

                entity.HasIndex(e => e.PersonId, "fk_email_person_id_idx");

                entity.Property(e => e.EmailId).HasColumnType("int(11)");

                entity.Property(e => e.EmailAddress)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EmailAddressType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsPrimary).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Emails)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_email_person_id");
            });

            modelBuilder.Entity<Glaccount>(entity =>
            {
                entity.ToTable("glaccount");

                entity.HasIndex(e => e.AccountTypeId, "fk_glaccount_Accounttype_id_idx");

                entity.HasIndex(e => e.CostCenterId, "fk_glaccount_depart_CC_id_idx");

                entity.Property(e => e.GlAccountId).HasColumnType("int(11)");

                entity.Property(e => e.AccountTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CostCenterId).HasColumnType("int(11)");

                entity.Property(e => e.DetailType)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.Glaccounts)
                    .HasForeignKey(d => d.AccountTypeId)
                    .HasConstraintName("fk_glaccount_accounttype_id");

                entity.HasOne(d => d.CostCenter)
                    .WithMany(p => p.Glaccounts)
                    .HasForeignKey(d => d.CostCenterId)
                    .HasConstraintName("fk_glaccount_depart_cc_id");
            });

            modelBuilder.Entity<Glaccounttype>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PRIMARY");

                entity.ToTable("glaccounttype");

                entity.Property(e => e.AccountId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoice");

                entity.HasIndex(e => e.MembershipId, "fk_invoice_membership_id_idx");

                entity.HasIndex(e => e.PaymentTransactionId, "fk_invoice_paymenttransaction_id_idx");

                entity.HasIndex(e => e.BillablePersonId, "fk_invoice_person_id_idx");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.BillablePersonId).HasColumnType("int(11)");

                entity.Property(e => e.BillingType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("date");

                entity.Property(e => e.InvoiceItemType).HasColumnType("int(11)");

                entity.Property(e => e.InvoiceType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.Notes)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PaymentTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PromoCodeId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.BillablePerson)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.BillablePersonId)
                    .HasConstraintName("fk_invoice_person_id");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_invoice_membership_id");

                entity.HasOne(d => d.PaymentTransaction)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PaymentTransactionId)
                    .HasConstraintName("fk_invoice_paymenttransaction_id");
            });

            modelBuilder.Entity<Invoicedetail>(entity =>
            {
                entity.ToTable("invoicedetail");

                entity.HasIndex(e => e.ItemId, "fk_invoice_detail_Item_itemId_idx");

                entity.HasIndex(e => e.InvoiceId, "fk_invoice_detail_invoicce_id_idx");

                entity.HasIndex(e => e.FeeId, "fk_invoice_detail_membership_feeId_idx");

                entity.Property(e => e.InvoiceDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(13, 2);

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Discount).HasPrecision(13, 2);

                entity.Property(e => e.FeeId).HasColumnType("int(11)");

                entity.Property(e => e.GlAccount)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.ItemId).HasColumnType("int(11)");

                entity.Property(e => e.ItemType).HasColumnType("int(11)");

                entity.Property(e => e.Price).HasPrecision(13, 2);

                entity.Property(e => e.Quantity).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TaxAmount).HasPrecision(13, 2);

                entity.Property(e => e.TaxRate).HasPrecision(13, 2);

                entity.Property(e => e.Taxable).HasColumnType("int(11)");

                entity.HasOne(d => d.Fee)
                    .WithMany(p => p.Invoicedetails)
                    .HasForeignKey(d => d.FeeId)
                    .HasConstraintName("fk_invoice_detail_membership_feeId");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Invoicedetails)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("fk_invoice_detail_invoicce_id");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Invoicedetails)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("fk_invoice_detail_Item_itemId");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("item");

                entity.HasIndex(e => e.ItemGlAccount, "fk_ServiceItem_GlAccount_idx");

                entity.HasIndex(e => e.ItemType, "fk_serviceItem_itemType_idx");

                entity.Property(e => e.ItemId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Image).HasColumnType("blob");

                entity.Property(e => e.ItemCode)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ItemGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.ItemType).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.UnitRate).HasPrecision(13, 2);

                entity.HasOne(d => d.ItemGlAccountNavigation)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ItemGlAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ServiceItem_GlAccount");

                entity.HasOne(d => d.ItemTypeNavigation)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ItemType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_serviceItem_itemType");
            });

            modelBuilder.Entity<Itemtype>(entity =>
            {
                entity.ToTable("itemtype");

                entity.Property(e => e.ItemTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Journalentrydetail>(entity =>
            {
                entity.ToTable("journalentrydetail");

                entity.HasIndex(e => e.JournalEntryHeaderId, "fk_journalentrydetail_header_id_idx");

                entity.HasIndex(e => e.ReceiptDetailId, "fk_journalentrydetail_receiptdetail_id__idx");

                entity.Property(e => e.JournalEntryDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(14, 2);

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EntryType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.GlAccountCode)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.JournalEntryHeaderId).HasColumnType("int(11)");

                entity.Property(e => e.ReceiptDetailId).HasColumnType("int(11)");

                entity.HasOne(d => d.JournalEntryHeader)
                    .WithMany(p => p.Journalentrydetails)
                    .HasForeignKey(d => d.JournalEntryHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journalentrydetail_header_id");

                entity.HasOne(d => d.ReceiptDetail)
                    .WithMany(p => p.Journalentrydetails)
                    .HasForeignKey(d => d.ReceiptDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journalentrydetail_receiptdetail_id_");
            });

            modelBuilder.Entity<Journalentryheader>(entity =>
            {
                entity.ToTable("journalentryheader");

                entity.HasIndex(e => e.ReceiptHeaderId, "fk_journalentryheader_receipt_Id_idx");

                entity.HasIndex(e => e.UserId, "fk_journalentryheader_userId_idx");

                entity.Property(e => e.JournalEntryHeaderId).HasColumnType("int(11)");

                entity.Property(e => e.EntryDate).HasColumnType("datetime");

                entity.Property(e => e.ReceiptHeaderId).HasColumnType("int(11)");

                entity.Property(e => e.TransactionType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.ReceiptHeader)
                    .WithMany(p => p.Journalentryheaders)
                    .HasForeignKey(d => d.ReceiptHeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journalentryheader_receipt_Id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Journalentryheaders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journalentryheader_userId");
            });

            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.ToTable("lookup");

                entity.Property(e => e.LookupId).HasColumnType("int(11)");

                entity.Property(e => e.Group)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Values)
                    .HasColumnType("varchar(5000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.ToTable("membership");

                entity.HasIndex(e => e.MembershipTypeId, "fk_membbership_membbership_type_id_idx");

                entity.HasIndex(e => e.BillablePersonId, "fk_mmembbership_billabble_personId_idx");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.AutoPayEnabled).HasColumnType("int(11)");

                entity.Property(e => e.BillablePersonId).HasColumnType("int(11)");

                entity.Property(e => e.BillingOnHold).HasColumnType("int(11)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.NextBillDate).HasColumnType("date");

                entity.Property(e => e.PaymentProfileId)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RenewalDate).HasColumnType("date");

                entity.Property(e => e.ReviewDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TerminationDate).HasColumnType("date");

                entity.HasOne(d => d.BillablePerson)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.BillablePersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_mmembbership_billabble_personId");

                entity.HasOne(d => d.MembershipType)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.MembershipTypeId)
                    .HasConstraintName("fk_membership_membership_type_id");
            });

            modelBuilder.Entity<Membershipcategory>(entity =>
            {
                entity.ToTable("membershipcategory");

                entity.Property(e => e.MembershipCategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Membershipconnection>(entity =>
            {
                entity.ToTable("membershipconnection");

                entity.HasIndex(e => e.PersonId, "fk_membershiprelation_person_idx");

                entity.HasIndex(e => e.MembershipId, "fk_membershiprelationship_membership_id_idx");

                entity.Property(e => e.MembershipConnectionId).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Membershipconnections)
                    .HasForeignKey(d => d.MembershipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_membershiprelationship_membership_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Membershipconnections)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_membershiprelation_person");
            });

            modelBuilder.Entity<Membershipfee>(entity =>
            {
                entity.HasKey(e => e.FeeId)
                    .HasName("PRIMARY");

                entity.ToTable("membershipfee");

                entity.HasIndex(e => e.MembershipTypeId, "fk_MembershipTypeId_idx");

                entity.HasIndex(e => e.GlAccountId, "fk_mmemmbbershipfee_glchartof_account_id_idx");

                entity.Property(e => e.FeeId).HasColumnType("int(11)");

                entity.Property(e => e.BillingFrequency).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FeeAmount).HasPrecision(13, 2);

                entity.Property(e => e.GlAccountId).HasColumnType("int(11)");

                entity.Property(e => e.IsMandatory).HasColumnType("int(11)");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.GlAccount)
                    .WithMany(p => p.Membershipfees)
                    .HasForeignKey(d => d.GlAccountId)
                    .HasConstraintName("fk_mmemmbbershipfee_glchartof_account_id");

                entity.HasOne(d => d.MembershipType)
                    .WithMany(p => p.Membershipfees)
                    .HasForeignKey(d => d.MembershipTypeId)
                    .HasConstraintName("fk_MembershipTypeId");
            });

            modelBuilder.Entity<Membershiphistory>(entity =>
            {
                entity.ToTable("membershiphistory");

                entity.HasIndex(e => e.MembershipId, "fk_embbershiphistory_membership_id_idx");

                entity.HasIndex(e => e.ChangedBy, "fk_membership_staff_user_id_idx");

                entity.Property(e => e.MembershipHistoryId).HasColumnType("int(11)");

                entity.Property(e => e.ChangedBy).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.PreviousMembershipId).HasColumnType("int(11)");

                entity.Property(e => e.Reason)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.StatusDate).HasColumnType("datetime");

                entity.HasOne(d => d.ChangedByNavigation)
                    .WithMany(p => p.Membershiphistories)
                    .HasForeignKey(d => d.ChangedBy)
                    .HasConstraintName("fk_membership_staff_user_id");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Membershiphistories)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_embbershiphistory_membership_id");
            });

            modelBuilder.Entity<Membershipperiod>(entity =>
            {
                entity.ToTable("membershipperiod");

                entity.Property(e => e.MembershipPeriodId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Duration).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PeriodUnit)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Membershiptype>(entity =>
            {
                entity.ToTable("membershiptype");

                entity.HasIndex(e => e.Category, "fk_membershiptype_memmbeeership_category_id_idx");

                entity.HasIndex(e => e.Period, "fk_mmembershiptype_period_id_idx");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Category).HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Period).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Units).HasColumnType("int(11)");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Membershiptypes)
                    .HasForeignKey(d => d.Category)
                    .HasConstraintName("fk_membershiptype_membership_category_id");

                entity.HasOne(d => d.PeriodNavigation)
                    .WithMany(p => p.Membershiptypes)
                    .HasForeignKey(d => d.Period)
                    .HasConstraintName("fk_membershiptype_period_id");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("menu");

                entity.HasIndex(e => e.GroupId, "fk_menu_menugroupId_idx");

                entity.Property(e => e.MenuId).HasColumnType("int(11)");

                entity.Property(e => e.Disabled).HasColumnType("int(11)");

                entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

                entity.Property(e => e.Expanded).HasColumnType("int(11)");

                entity.Property(e => e.GroupId).HasColumnType("int(11)");

                entity.Property(e => e.Icon)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Label)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentMenuId).HasColumnType("int(11)");

                entity.Property(e => e.RouterUrl)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("RouterURL")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Tooltip)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("fk_menu_menugroupId");
            });

            modelBuilder.Entity<Menugroup>(entity =>
            {
                entity.ToTable("menugroup");

                entity.Property(e => e.MenuGroupId).HasColumnType("int(11)");

                entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

                entity.Property(e => e.GroupName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.MenuName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Multifactorcode>(entity =>
            {
                entity.ToTable("multifactorcode");

                entity.HasIndex(e => e.UserId, "fk_multifactorcode_staffuser_id_idx");

                entity.Property(e => e.MultiFactorCodeId).HasColumnType("int(11)");

                entity.Property(e => e.AccessCode)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Attempts).HasColumnType("int(11)");

                entity.Property(e => e.CodeExpired).HasColumnType("int(11)");

                entity.Property(e => e.CodeUsed).HasColumnType("int(11)");

                entity.Property(e => e.CommunicationMode)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CreatDate).HasColumnType("datetime");

                entity.Property(e => e.DeviceId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.IpAddress)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Otpcode)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("OTPCode")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Token)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserAgent)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Multifactorcodes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_multifactorcode_staffuser_id");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("note");

                entity.HasIndex(e => e.PersonId, "fk_note_personid_idx");

                entity.Property(e => e.NoteId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DisplayOnProfile).HasColumnType("int(11)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Severity)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_note_personid");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("organization");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Address1)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address2)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address3)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.City)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(5)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Country)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Facebook)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FooterImge)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.HeaderImage)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LinkedIn)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Logo)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Phone)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Prefix)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PrimaryContactEmail)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PrimaryContactName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PrimaryContactPhone)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PrintMessage)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.State)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TwilioAccountSid)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TwilioAuthToken)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Twitter)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.WebMessage)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Website)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Zip)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Paperinvoice>(entity =>
            {
                entity.ToTable("paperinvoice");

                entity.HasIndex(e => e.PaperBillingCycleId, "fk_paper_invoice_billing_cycle_id_idx");

                entity.HasIndex(e => e.InvoiceId, "fk_paper_invoice_invoice_id_idx");

                entity.HasIndex(e => e.PersonId, "fk_paper_invoice_person_id_idx");

                entity.Property(e => e.PaperInvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(14, 2);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.PaperBillingCycleId).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Paperinvoices)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("fk_paper_invoice_invoice_id");

                entity.HasOne(d => d.PaperBillingCycle)
                    .WithMany(p => p.Paperinvoices)
                    .HasForeignKey(d => d.PaperBillingCycleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_paper_invoice_billing_cycle_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Paperinvoices)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_paper_invoice_person_id");
            });

            modelBuilder.Entity<Paymentprocessor>(entity =>
            {
                entity.ToTable("paymentprocessor");

                entity.HasIndex(e => e.OrganizationId, "fk_payment_processor_organization_id_idx");

                entity.Property(e => e.PaymentProcessorId).HasColumnType("int(11)");

                entity.Property(e => e.ApiKey)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LiveUrl)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("LiveURL")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LoginId)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MerchantId)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.TestUrl)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("TestURL")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TransactionKey)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TransactionMode).HasColumnType("int(11)");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Paymentprocessors)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_payment_processor_organization_id");
            });

            modelBuilder.Entity<Paymentprofile>(entity =>
            {
                entity.ToTable("paymentprofile");

                entity.HasIndex(e => e.PersonId, "fk_paymentProfileId_person_id_idx");

                entity.Property(e => e.PaymentProfileId).HasColumnType("int(11)");

                entity.Property(e => e.AuthNetPaymentProfileId)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CardNumber)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CardType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PreferredPaymentMethod).HasColumnType("int(11)");

                entity.Property(e => e.ProfileId)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Paymentprofiles)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_paymentProfileId_person_id");
            });

            modelBuilder.Entity<Paymenttransaction>(entity =>
            {
                entity.ToTable("paymenttransaction");

                entity.HasIndex(e => e.ReceiptId, "fk_paymenttransaction_recei[pt_id_idx");

                entity.HasIndex(e => e.PersonId, "fk_pt_personid_idx");

                entity.Property(e => e.PaymentTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.AccountHolderName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AccountNumber)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AccountType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Amount).HasPrecision(15, 2);

                entity.Property(e => e.AuthCode)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BankName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CardType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreditCardHolderName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsAdjusted).HasColumnType("int(11)");

                entity.Property(e => e.MessageDetails)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PaymentType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.ReceiptId).HasColumnType("int(11)");

                entity.Property(e => e.RefId)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ReferenceNumber)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ReferenceTransactionId)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ResponseCode)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ResponseDetails)
                    .HasColumnType("varchar(1500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Result).HasColumnType("int(11)");

                entity.Property(e => e.RoutingNumber)
                    .HasColumnType("varchar(9)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ShoppingCartId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionId)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TransactionType).HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Paymenttransactions)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_pt_personid");

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.Paymenttransactions)
                    .HasForeignKey(d => d.ReceiptId)
                    .HasConstraintName("fk_paymenttransaction_receipt_id");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("person");

                entity.HasIndex(e => e.CompanyId, "fk_person_comapny_id_idx");

                entity.HasIndex(e => e.OrganizationId, "fk_person_organization_id_idx");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.CasualName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.FacebookName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Gender)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LinkedinName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MiddleName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.PreferredContact).HasColumnType("int(11)");

                entity.Property(e => e.Prefix)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProfilePictureId).HasColumnType("int(11)");

                entity.Property(e => e.Salutation)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SkypeName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Suffix)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TwitterName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.WebLoginName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.WebPassword)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.WebPasswordSalt)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Website)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.People)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_person_comapny_id");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.People)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_person_organization_id");
            });

            modelBuilder.Entity<Persontag>(entity =>
            {
                entity.HasKey(e => e.TagId)
                    .HasName("PRIMARY");

                entity.ToTable("persontag");

                entity.HasIndex(e => e.PersonId, "fk_persontagg_person_id_idx");

                entity.Property(e => e.TagId).HasColumnType("int(11)");

                entity.Property(e => e.Label)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Persontags)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_persontagg_person_id");
            });

            modelBuilder.Entity<Phone>(entity =>
            {
                entity.ToTable("phone");

                entity.HasIndex(e => e.PersonId, "fk_phone_person_id_idx");

                entity.Property(e => e.PhoneId).HasColumnType("int(11)");

                entity.Property(e => e.IsPrimary).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PhoneType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_phone_person_id");
            });

            modelBuilder.Entity<Promocode>(entity =>
            {
                entity.ToTable("promocode");

                entity.HasIndex(e => e.GlAccountId, "pfk_promocode-GlAccount_id_idx");

                entity.Property(e => e.PromoCodeId).HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Discount).HasPrecision(13, 2);

                entity.Property(e => e.DiscountType).HasColumnType("int(11)");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.GlAccountId).HasColumnType("int(11)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TransactionType).HasColumnType("int(11)");

                entity.HasOne(d => d.GlAccount)
                    .WithMany(p => p.Promocodes)
                    .HasForeignKey(d => d.GlAccountId)
                    .HasConstraintName("pfk_promocode-GlAccount_id");
            });

            modelBuilder.Entity<Receiptdetail>(entity =>
            {
                entity.ToTable("receiptdetail");

                entity.HasIndex(e => e.InvoiceDetailId, "fk_rcptdetail_invoicedetailid_idx");

                entity.HasIndex(e => e.ReceiptHeaderId, "fk_rcptheader_id_idx");

                entity.Property(e => e.ReceiptDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(15, 2);

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Discount).HasPrecision(15, 2);

                entity.Property(e => e.InvoiceDetailId).HasColumnType("int(11)");

                entity.Property(e => e.ItemType).HasColumnType("int(11)");

                entity.Property(e => e.PastDueInvoiceDetailRef).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Quantity).HasColumnType("int(11)");

                entity.Property(e => e.Rate).HasPrecision(15, 2);

                entity.Property(e => e.ReceiptHeaderId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Tax).HasPrecision(15, 2);

                entity.HasOne(d => d.InvoiceDetail)
                    .WithMany(p => p.Receiptdetails)
                    .HasForeignKey(d => d.InvoiceDetailId)
                    .HasConstraintName("fk_rcptdetail_invoicedetailid");

                entity.HasOne(d => d.ReceiptHeader)
                    .WithMany(p => p.Receiptdetails)
                    .HasForeignKey(d => d.ReceiptHeaderId)
                    .HasConstraintName("fk_rcptdetail_rcptheaderid");
            });

            modelBuilder.Entity<Receiptheader>(entity =>
            {
                entity.HasKey(e => e.Receiptid)
                    .HasName("PRIMARY");

                entity.ToTable("receiptheader");

                entity.HasIndex(e => e.OrganizationId, "fk_organizationid_idx");

                entity.HasIndex(e => e.StaffId, "fk_staffid_idx");

                entity.Property(e => e.Receiptid).HasColumnType("int(11)");

                entity.Property(e => e.CheckNo)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Notes)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.PaymentMode)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PromoCodeId).HasColumnType("int(11)");

                entity.Property(e => e.StaffId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Receiptheaders)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_organizationid");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Receiptheaders)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("fk_staffid");
            });

            modelBuilder.Entity<Receiptitemdetail>(entity =>
            {
                entity.HasKey(e => e.ReciptItemDetailId)
                    .HasName("PRIMARY");

                entity.ToTable("receiptitemdetail");

                entity.HasIndex(e => e.ReceiptDetailId, "fk_rid_rcptdetailid_idx");

                entity.Property(e => e.ReciptItemDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(15, 2);

                entity.Property(e => e.AmountEdited).HasPrecision(15, 2);

                entity.Property(e => e.CurrentPrice).HasPrecision(15, 2);

                entity.Property(e => e.Discount).HasPrecision(15, 2);

                entity.Property(e => e.GlaccountId)
                    .HasColumnType("int(11)")
                    .HasColumnName("GLAccountId");

                entity.Property(e => e.ItemType).HasColumnType("int(11)");

                entity.Property(e => e.ReceiptDetailId).HasColumnType("int(11)");

                entity.Property(e => e.RunningPrice).HasPrecision(15, 2);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.ReceiptDetail)
                    .WithMany(p => p.Receiptitemdetails)
                    .HasForeignKey(d => d.ReceiptDetailId)
                    .HasConstraintName("fk_rid_rcptdetailid");
            });

            modelBuilder.Entity<Refunddetail>(entity =>
            {
                entity.HasKey(e => e.RefundId)
                    .HasName("PRIMARY");

                entity.ToTable("refunddetail");

                entity.HasIndex(e => e.ReceiptDetailId, "fk_refund_receiptdetail_id_idx");

                entity.HasIndex(e => e.UserId, "fk_refund_usr_id_idx");

                entity.Property(e => e.RefundId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(14, 3);

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PaymentTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.ProcessingFee).HasPrecision(14, 3);

                entity.Property(e => e.Reason)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReceiptDetailId).HasColumnType("int(11)");

                entity.Property(e => e.RefundDate).HasColumnType("datetime");

                entity.Property(e => e.RefundMode).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.ReceiptDetail)
                    .WithMany(p => p.Refunddetails)
                    .HasForeignKey(d => d.ReceiptDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_refund_receiptdetail_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Refunddetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_refund_usr_id");
            });

            modelBuilder.Entity<Relation>(entity =>
            {
                entity.ToTable("relation");

                entity.HasIndex(e => e.PersonId, "fk_relation_person_id_idx");

                entity.HasIndex(e => e.RelatedPersonId, "fk_relation_related_person_id_idx");

                entity.HasIndex(e => e.RelationshipId, "fk_relation_relationshipId_idx");

                entity.Property(e => e.RelationId).HasColumnType("int(11)");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Notes)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.RelatedPersonId).HasColumnType("int(11)");

                entity.Property(e => e.RelationshipId).HasColumnType("int(11)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.RelationPeople)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_relation_person_id");

                entity.HasOne(d => d.RelatedPerson)
                    .WithMany(p => p.RelationRelatedPeople)
                    .HasForeignKey(d => d.RelatedPersonId)
                    .HasConstraintName("fk_relation_related_person_id");

                entity.HasOne(d => d.Relationship)
                    .WithMany(p => p.Relations)
                    .HasForeignKey(d => d.RelationshipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_relation_relationshipId");
            });

            modelBuilder.Entity<Relationship>(entity =>
            {
                entity.ToTable("relationship");

                entity.Property(e => e.RelationshipId).HasColumnType("int(11)");

                entity.Property(e => e.Relation)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReverseRelationFemale)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReverseRelationMale)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("report");

                entity.HasIndex(e => e.OrganizationId, "fk_report_organizationId_idx");

                entity.HasIndex(e => e.UserId, "fk_report_userId_idx");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Shared).HasColumnType("tinyint(4)");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_report_organizationId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_report_userId");
            });

            modelBuilder.Entity<Reportargument>(entity =>
            {
                entity.ToTable("reportargument");

                entity.HasIndex(e => e.ReportId, "fk_reportargument_reportId_idx");

                entity.Property(e => e.ReportArgumentId).HasColumnType("int(11)");

                entity.Property(e => e.DataType)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.FieldType)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.Label)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.Property(e => e.Required).HasColumnType("tinyint(4)");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Reportarguments)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_reportargument_reportId");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Rolemenu>(entity =>
            {
                entity.ToTable("rolemenu");

                entity.HasIndex(e => e.MenuId, "fk_menu_id_idx");

                entity.HasIndex(e => e.RoleId, "fk_role_menu_idx");

                entity.Property(e => e.RoleMenuId).HasColumnType("int(11)");

                entity.Property(e => e.MenuId).HasColumnType("int(11)");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Rolemenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_menu_menu_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Rolemenus)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_role_role_id");
            });

            modelBuilder.Entity<Shoppingcart>(entity =>
            {
                entity.ToTable("shoppingcart");

                entity.HasIndex(e => e.ReceiptId, "fk_shoppingcart_receiptId_idx");

                entity.HasIndex(e => e.UserId, "fk_shoppingcart_staff_user_id_idx");

                entity.Property(e => e.ShoppingCartId).HasColumnType("int(11)");

                entity.Property(e => e.CreditBalance).HasPrecision(14, 3);

                entity.Property(e => e.PaymentMode)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PaymentStatus).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PromoCodeId).HasColumnType("int(11)");

                entity.Property(e => e.ReceiptId).HasColumnType("int(11)");

                entity.Property(e => e.SessionId)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.UseCreditBalance).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.Shoppingcarts)
                    .HasForeignKey(d => d.ReceiptId)
                    .HasConstraintName("fk_shoppingcart_receiptId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Shoppingcarts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_shoppingcart_staff_user_id");
            });

            modelBuilder.Entity<Shoppingcartdetail>(entity =>
            {
                entity.ToTable("shoppingcartdetail");

                entity.HasIndex(e => e.MembershipId, "fk_shoppingCartDetail_MembershipId_idx");

                entity.HasIndex(e => e.ShoppingCartId, "fk_shoppingcartdetail_shoppingcart_id_idx");

                entity.Property(e => e.ShoppingCartDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(13, 2);

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Discount).HasPrecision(13, 2);

                entity.Property(e => e.ItemGroup)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ItemGroupDescription)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ItemId).HasColumnType("int(11)");

                entity.Property(e => e.ItemType).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.Price).HasPrecision(13, 2);

                entity.Property(e => e.Quantity).HasColumnType("int(11)");

                entity.Property(e => e.Selected).HasColumnType("int(11)");

                entity.Property(e => e.ShoppingCartId).HasColumnType("int(11)");

                entity.Property(e => e.Tax).HasPrecision(13, 2);

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Shoppingcartdetails)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_shoppingCartDetail_MembershipId");

                entity.HasOne(d => d.ShoppingCart)
                    .WithMany(p => p.Shoppingcartdetails)
                    .HasForeignKey(d => d.ShoppingCartId)
                    .HasConstraintName("fk_shoppingcartdetail_shoppingcart_id");
            });

            modelBuilder.Entity<Staffrole>(entity =>
            {
                entity.ToTable("staffrole");

                entity.HasIndex(e => e.RoleId, "fk_role_id_idx");

                entity.HasIndex(e => e.StaffId, "fk_staff_id_idx");

                entity.Property(e => e.StaffRoleId).HasColumnType("int(11)");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.Property(e => e.StaffId).HasColumnType("int(11)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Staffroles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_role_id");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Staffroles)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_staff_id");
            });

            modelBuilder.Entity<Staffuser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("staffuser");

                entity.HasIndex(e => e.DepartmentId, "fk_staff_user_deprtment_id_idx");

                entity.HasIndex(e => e.OrganizationId, "fk_staff_user_organization_id_idx");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.CellPhoneNumber)
                    .HasColumnType("varchar(15)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FailedAttempts).HasColumnType("int(11)");

                entity.Property(e => e.FirstName)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastAccessed).HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Locked).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Password)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Salt)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.UserName)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Staffusers)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_staff_user_deprtment_id");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Staffusers)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_staff_user_organization_id");
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.ToTable("token");

                entity.HasIndex(e => e.UserId, "fk_token_staffuser_id_idx");

                entity.Property(e => e.TokenId).HasColumnType("int(11)");

                entity.Property(e => e.ClientId)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryTime).HasColumnType("datetime");

                entity.Property(e => e.TokenValue)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_token_staffuser_id");
            });

            modelBuilder.Entity<Userdevice>(entity =>
            {
                entity.ToTable("userdevice");

                entity.HasIndex(e => e.UserId, "fk_userdevice_staffuser_id_idx");

                entity.Property(e => e.UserDeviceId).HasColumnType("int(11)");

                entity.Property(e => e.Authenticated).HasColumnType("int(11)");

                entity.Property(e => e.DeviceName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Ipaddress)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("IPAddress")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.LastAccessed).HasColumnType("datetime");

                entity.Property(e => e.Locked).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userdevices)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_userdevice_staffuser_id");
            });

            modelBuilder.Entity<YuniqlSchemaVersion>(entity =>
            {
                entity.HasKey(e => e.SequenceId)
                    .HasName("PRIMARY");

                entity.ToTable("__yuniql_schema_version");

                entity.HasIndex(e => e.Version, "ix___yuniql_schema_version")
                    .IsUnique();

                entity.Property(e => e.SequenceId)
                    .HasColumnType("int(11)")
                    .HasColumnName("sequence_id");

                entity.Property(e => e.AdditionalArtifacts)
                    .HasColumnType("varchar(4000)")
                    .HasColumnName("additional_artifacts")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AppliedByTool)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("applied_by_tool")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AppliedByToolVersion)
                    .IsRequired()
                    .HasColumnType("varchar(16)")
                    .HasColumnName("applied_by_tool_version")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AppliedByUser)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("applied_by_user")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AppliedOnUtc)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("applied_on_utc")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Checksum)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasColumnName("checksum")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DurationMs)
                    .HasColumnType("int(11)")
                    .HasColumnName("duration_ms");

                entity.Property(e => e.FailedScriptError)
                    .HasColumnType("varchar(4000)")
                    .HasColumnName("failed_script_error")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FailedScriptPath)
                    .HasColumnType("varchar(4000)")
                    .HasColumnName("failed_script_path")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("status")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasColumnType("varchar(190)")
                    .HasColumnName("version")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
