using System;
using Max.Data.Audit;
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
        public virtual DbSet<Answeroption> Answeroptions { get; set; }
        public virtual DbSet<Answertoquestion> Answertoquestions { get; set; }
        public virtual DbSet<Answertypelookup> Answertypelookups { get; set; }
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
        public virtual DbSet<Clientlog> Clientlogs { get; set; }
        public virtual DbSet<Communication> Communications { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<Contactactivity> Contactactivities { get; set; }
        public virtual DbSet<Contactactivityinteraction> Contactactivityinteractions { get; set; }
        public virtual DbSet<Contactrole> Contactroles { get; set; }
        public virtual DbSet<Contacttoken> Contacttokens { get; set; }
        public virtual DbSet<Containeraccess> Containeraccesses { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Credittransaction> Credittransactions { get; set; }
        public virtual DbSet<Customfield> Customfields { get; set; }
        public virtual DbSet<Customfielddata> Customfielddata { get; set; }
        public virtual DbSet<Customfieldlookup> Customfieldlookups { get; set; }
        public virtual DbSet<Customfieldoption> Customfieldoptions { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Documentaccess> Documentaccesses { get; set; }
        public virtual DbSet<Documentcontainer> Documentcontainers { get; set; }
        public virtual DbSet<Documentobject> Documentobjects { get; set; }
        public virtual DbSet<Documentobjectaccesshistory> Documentobjectaccesshistories { get; set; }
        public virtual DbSet<Documentsearchhistory> Documentsearchhistories { get; set; }
        public virtual DbSet<Documenttag> Documenttags { get; set; }
        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<Entityrole> Entityroles { get; set; }
        public virtual DbSet<Entityrolehistory> Entityrolehistories { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Eventcontact> Eventcontacts { get; set; }
        public virtual DbSet<Eventregister> Eventregisters { get; set; }
        public virtual DbSet<Eventregisterquestion> Eventregisterquestions { get; set; }
        public virtual DbSet<Eventregistersession> Eventregistersessions { get; set; }
        public virtual DbSet<Eventreport> Eventreports { get; set; }
        public virtual DbSet<Eventtype> Eventtypes { get; set; }
        public virtual DbSet<Fieldtype> Fieldtypes { get; set; }
        public virtual DbSet<Glaccount> Glaccounts { get; set; }
        public virtual DbSet<Glaccounttype> Glaccounttypes { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Grouphistory> Grouphistories { get; set; }
        public virtual DbSet<Groupmember> Groupmembers { get; set; }
        public virtual DbSet<Groupmemberrole> Groupmemberroles { get; set; }
        public virtual DbSet<Grouprole> Grouproles { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Invoicedetail> Invoicedetails { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Itemtype> Itemtypes { get; set; }
        public virtual DbSet<Journalentrydetail> Journalentrydetails { get; set; }
        public virtual DbSet<Journalentryheader> Journalentryheaders { get; set; }
        public virtual DbSet<Linkeventfeetype> Linkeventfeetypes { get; set; }
        public virtual DbSet<Linkeventgroup> Linkeventgroups { get; set; }
        public virtual DbSet<Linkgrouprole> Linkgrouproles { get; set; }
        public virtual DbSet<Linkregistrationgroupfee> Linkregistrationgroupfees { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<Membershipcategory> Membershipcategories { get; set; }
        public virtual DbSet<Membershipconnection> Membershipconnections { get; set; }
        public virtual DbSet<Membershipfee> Membershipfees { get; set; }
        public virtual DbSet<Membershiphistory> Membershiphistories { get; set; }
        public virtual DbSet<Membershipperiod> Membershipperiods { get; set; }
        public virtual DbSet<Membershipreport> Membershipreports { get; set; }
        public virtual DbSet<Membershiptype> Membershiptypes { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Menugroup> Menugroups { get; set; }
        public virtual DbSet<Moduleinfo> Moduleinfos { get; set; }
        public virtual DbSet<Multifactorcode> Multifactorcodes { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Opportunity> Opportunities { get; set; }
        public virtual DbSet<Opportunitypipeline> Opportunitypipelines { get; set; }
        public virtual DbSet<Opportunityreport> Opportunityreports { get; set; }
        public virtual DbSet<Paperinvoice> Paperinvoices { get; set; }
        public virtual DbSet<Paymentprocessor> Paymentprocessors { get; set; }
        public virtual DbSet<Paymentprofile> Paymentprofiles { get; set; }
        public virtual DbSet<Paymenttransaction> Paymenttransactions { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Persontag> Persontags { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }
        public virtual DbSet<Pipelineproduct> Pipelineproducts { get; set; }
        public virtual DbSet<Pipelinestage> Pipelinestages { get; set; }
        public virtual DbSet<Pricingforlookup> Pricingforlookups { get; set; }
        public virtual DbSet<Promocode> Promocodes { get; set; }
        public virtual DbSet<Questionbank> Questionbanks { get; set; }
        public virtual DbSet<Questionlink> Questionlinks { get; set; }
        public virtual DbSet<Receiptdetail> Receiptdetails { get; set; }
        public virtual DbSet<Receiptheader> Receiptheaders { get; set; }
        public virtual DbSet<Receiptitemdetail> Receiptitemdetails { get; set; }
        public virtual DbSet<Refunddetail> Refunddetails { get; set; }
        public virtual DbSet<Registrationfeetype> Registrationfeetypes { get; set; }
        public virtual DbSet<Registrationgroup> Registrationgroups { get; set; }
        public virtual DbSet<Registrationgroupmembershiplink> Registrationgroupmembershiplinks { get; set; }
        public virtual DbSet<Relation> Relations { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Reportargument> Reportarguments { get; set; }
        public virtual DbSet<Reportcategory> Reportcategories { get; set; }
        public virtual DbSet<Reportfield> Reportfields { get; set; }
        public virtual DbSet<Reportfilter> Reportfilters { get; set; }
        public virtual DbSet<Reportparameter> Reportparameters { get; set; }
        public virtual DbSet<Reportshare> Reportshares { get; set; }
        public virtual DbSet<Reportsortorder> Reportsortorders { get; set; }
        public virtual DbSet<Resetpassword> Resetpasswords { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Rolemenu> Rolemenus { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Sessionleaderlink> Sessionleaderlinks { get; set; }
        public virtual DbSet<Sessionregistrationgrouppricing> Sessionregistrationgrouppricings { get; set; }
        public virtual DbSet<Shoppingcart> Shoppingcarts { get; set; }
        public virtual DbSet<Shoppingcartdetail> Shoppingcartdetails { get; set; }
        public virtual DbSet<Solrexport> Solrexports { get; set; }
        public virtual DbSet<Staffrole> Staffroles { get; set; }
        public virtual DbSet<Staffuser> Staffusers { get; set; }
        public virtual DbSet<Staffusersearchhistory> Staffusersearchhistories { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Tabinfo> Tabinfos { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Timezone> Timezones { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<TrApplock> TrApplocks { get; set; }
        public virtual DbSet<TrCategory> TrCategories { get; set; }
        public virtual DbSet<TrDefinition> TrDefinitions { get; set; }
        public virtual DbSet<TrObject> TrObjects { get; set; }
        public virtual DbSet<TrReport> TrReports { get; set; }
        public virtual DbSet<TrSet> TrSets { get; set; }
        public virtual DbSet<TrString> TrStrings { get; set; }
        public virtual DbSet<TrTemplate> TrTemplates { get; set; }
        public virtual DbSet<Userdevice> Userdevices { get; set; }
        public virtual DbSet<Voiddetail> Voiddetails { get; set; }
        public virtual DbSet<Writeoff> Writeoffs { get; set; }
        public virtual DbSet<YuniqlSchemaVersion> YuniqlSchemaVersions { get; set; }
        public virtual DbSet<Customfieldblock> Customfieldblocks { get; set; }
        public virtual DbSet<BillingCycleNotification> BillingCycleNotifications { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //                optionsBuilder.UseMySql("server=localhost;port=3306;database=max_nbma;uid=root;password=Anit1066", Microsoft.EntityFrameworkCore.ServerVersion.FromString("5.7.34-mysql"));
            //            }

            //use new MySqlServerVersion(new Version(5, 7, 34)) for billing service
            if (_tenant != null)
            {
                optionsBuilder.UseMySql(_tenant.ConnectionString, new MySqlServerVersion(new Version(5, 7, 34)));
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

                entity.Property(e => e.AccessTokenId).HasColumnType("int(11)");

                entity.Property(e => e.Create).HasColumnType("datetime");

                entity.Property(e => e.CreatedIp)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("CreatedIP")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

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

                entity.HasIndex(e => e.WriteOffGlAccount, "fk_as_writeoffglaccount_idx");

                entity.HasIndex(e => e.OnlineCreditGlAccount, "gf_as_onlinecreditglid_idx");

                entity.Property(e => e.AccountSetupId).HasColumnType("int(11)");

                entity.Property(e => e.OffLinePaymentGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.OnlineCreditGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.ProcessingFeeGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.SalesReturnGlAccount).HasColumnType("int(11)");

                entity.Property(e => e.WriteOffGlAccount).HasColumnType("int(11)");

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

                entity.HasOne(d => d.WriteOffGlAccountNavigation)
                    .WithMany(p => p.AccountingsetupWriteOffGlAccountNavigations)
                    .HasForeignKey(d => d.WriteOffGlAccount)
                    .HasConstraintName("fk_as_writeoffglaccount");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.HasIndex(e => e.CompanyId, "fk_address_company_id_idx");

                entity.HasIndex(e => e.PersonId, "fk_address_person_id_idx");

                entity.HasIndex(e => e.City, "idx_address_city");

                entity.HasIndex(e => e.Address1, "idx_address_street");

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

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.Country)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CountryCode)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsPrimary).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.State)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StateCode)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Zip)
                    .HasColumnType("varchar(15)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_address_company_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_address_person_id");
            });

            modelBuilder.Entity<Answeroption>(entity =>
            {
                entity.ToTable("answeroption");

                entity.HasIndex(e => e.QuestionBankId, "fk_ao_questionbank_id_idx");

                entity.Property(e => e.AnswerOptionId).HasColumnType("int(11)");

                entity.Property(e => e.Option)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.QuestionBankId).HasColumnType("int(11)");

                entity.HasOne(d => d.QuestionBank)
                    .WithMany(p => p.Answeroptions)
                    .HasForeignKey(d => d.QuestionBankId)
                    .HasConstraintName("fk_ao_questionbank_id");
            });

            modelBuilder.Entity<Answertoquestion>(entity =>
            {
                entity.ToTable("answertoquestion");

                entity.HasIndex(e => e.EntityId, "fk_atq_entity_id_idx");

                entity.HasIndex(e => e.EventId, "fk_atq_event_id_idx");

                entity.HasIndex(e => e.QuestionBankId, "fk_atq_questionbank_id_idx");

                entity.HasIndex(e => e.SessionId, "fk_atq_session_id_idx");

                entity.Property(e => e.AnswerToQuestionId).HasColumnType("int(11)");

                entity.Property(e => e.Answer)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.QuestionBankId).HasColumnType("int(11)");

                entity.Property(e => e.SessionId).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Answertoquestions)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_atq_entity_id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Answertoquestions)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("fk_atq_event_id");

                entity.HasOne(d => d.QuestionBank)
                    .WithMany(p => p.Answertoquestions)
                    .HasForeignKey(d => d.QuestionBankId)
                    .HasConstraintName("fk_atq_questionbank_id");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Answertoquestions)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("fk_atq_session_id");
            });

            modelBuilder.Entity<Answertypelookup>(entity =>
            {
                entity.ToTable("answertypelookup");

                entity.Property(e => e.AnswerTypeLookUpId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AnswerType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Autobillingdraft>(entity =>
            {
                entity.ToTable("autobillingdraft");

                entity.HasIndex(e => e.MembershipId, "fk_abd_Membership_id_idx");

                entity.HasIndex(e => e.BillingDocumentId, "fk_abd_billingdocumentid_idx");

                entity.HasIndex(e => e.EntityId, "fk_abd_entityid_idx");

                entity.HasIndex(e => e.InvoiceId, "fk_abd_invoiceId_idx");

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

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("varchar(25)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.IsProcessed).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NextDueDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentProfileId)
                    .HasColumnType("varchar(45)")
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

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Autobillingdrafts)
                    .HasForeignKey(d => d.EntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_abd_entityId");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Autobillingdrafts)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("fk_abd_invoiceId");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Autobillingdrafts)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_abd_Membership_id");
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

                entity.Property(e => e.CycleType).HasColumnType("int(11)");

                entity.Property(e => e.ThroughDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BillingCycleNotification>(entity =>
            {
                entity.HasKey(e => e.NotificationId)
                    .HasName("PRIMARY");

                entity.ToTable("billingcyclenotification");

                entity.Property(e => e.NotificationId).HasColumnType("int(11)");

                entity.Property(e => e.NotificationText)
                    .HasColumnType("varchar(5000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasIndex(e => e.BillingCycleId, "billingcyclenotification_ibfk_1");

                entity.Property(e => e.IsRead).HasColumnType("int(11)");

                entity.HasOne(d => d.BillingCycle)
                    .WithMany(p => p.BillingCycleNotifications)
                    .HasForeignKey(d => d.BillingCycleId)
                    .HasConstraintName("billingcyclenotification_ibfk_1");
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

            modelBuilder.Entity<Clientlog>(entity =>
            {
                entity.ToTable("clientlog");

                entity.Property(e => e.ClientLogId).HasColumnType("int(11)");

                entity.Property(e => e.ClientUrl)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Message)
                    .HasColumnType("varchar(5000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.OrganizationName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Route)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Stack)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Communication>(entity =>
            {
                entity.ToTable("communication");

                entity.HasIndex(e => e.EntityId, "fk_communication_entity_id_idx");

                entity.HasIndex(e => e.StaffUserId, "fk_communication_staffUser_id_idx");

                entity.Property(e => e.CommunicationId).HasColumnType("int(11)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.From)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Notes)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

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

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Communications)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_communication_entityId");

                entity.HasOne(d => d.StaffUser)
                    .WithMany(p => p.Communications)
                    .HasForeignKey(d => d.StaffUserId)
                    .HasConstraintName("fk_communication_staffUser_id");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.HasIndex(e => e.EntityId, "fk_company_entityid_idx");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.BillableContactId).HasColumnType("int(11)");

                entity.Property(e => e.CompanyName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.Phone)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.Website)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_company_entityid");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.ToTable("configuration");
                entity.HasIndex(e => e.OrganizationId, "fk_configuration_organization_idx");
                entity.Property(e => e.ConfigurationId).HasColumnType("int(11)");
                entity.Property(e => e.ContactDisplayMembership).HasColumnType("int(11)");
                entity.Property(e => e.ContactDisplayTabs)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
                entity.Property(e => e.DashboardType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
                entity.Property(e => e.DataViewLayout)
                    .HasColumnType("varchar(20)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
                entity.Property(e => e.DisplayCodes)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.DisplayContactCRMDeleteCompanyProfile)
                    .HasColumnType("int(11)")
                    .HasColumnName("DisplayContactCRMDeleteCompanyProfile")
                    .HasDefaultValueSql("'1'");
                entity.Property(e => e.DisplayContactCRMDeleteMemberProfile)
                    .HasColumnType("int(11)")
                    .HasColumnName("DisplayContactCRMDeleteMemberProfile")
                    .HasDefaultValueSql("'1'");
                entity.Property(e => e.DisplayCustomFields)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.DisplayDashboard)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");
                entity.Property(e => e.DisplayRoles).HasColumnType("int(11)");
                entity.Property(e => e.DocumentAccessControl)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");
                entity.Property(e => e.SociableBaseUrl)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
                entity.Property(e => e.SociableSyncEnabled).HasColumnType("int(11)");
                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Configurations)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_configuration_organization");
            });

            modelBuilder.Entity<Contactactivity>(entity =>
            {
                entity.ToTable("contactactivity");

                entity.HasIndex(e => e.AccountId, "fk_activity_accountId_idx");

                entity.HasIndex(e => e.EntityId, "fk_activity_entityId_idx");

                entity.HasIndex(e => e.StaffUserId, "fk_activity_staff_user_id_idx");

                entity.Property(e => e.ContactActivityId).HasColumnType("int(11)");

                entity.Property(e => e.AccountId).HasColumnType("int(11)");

                entity.Property(e => e.ActivityConnection).HasColumnType("int(11)");

                entity.Property(e => e.ActivityDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(2500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.InteractionType).HasColumnType("int(11)");

                entity.Property(e => e.StaffUserId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Subject)
                    .HasColumnType("varchar(300)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Contactactivities)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("fk_activity_accountId");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Contactactivities)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_activity_entityId");

                entity.HasOne(d => d.StaffUser)
                    .WithMany(p => p.Contactactivities)
                    .HasForeignKey(d => d.StaffUserId)
                    .HasConstraintName("fk_activity_staff_user_id");
            });

            modelBuilder.Entity<Contactactivityinteraction>(entity =>
            {
                entity.ToTable("contactactivityinteraction");

                entity.HasIndex(e => e.ContactActivityId, "fk_activityInteraction_contactActivityId_idx");

                entity.HasIndex(e => e.InteractionAccountId, "fk_activityInteraction_interaction_companyId_idx");

                entity.HasIndex(e => e.InteractionEntityId, "fk_activityInteraction_interaction_entityId_idx");

                entity.Property(e => e.ContactActivityInteractionId).HasColumnType("int(11)");

                entity.Property(e => e.ContactActivityId).HasColumnType("int(11)");

                entity.Property(e => e.InteractionAccountId).HasColumnType("int(11)");

                entity.Property(e => e.InteractionEntityId).HasColumnType("int(11)");

                entity.HasOne(d => d.ContactActivity)
                    .WithMany(p => p.Contactactivityinteractions)
                    .HasForeignKey(d => d.ContactActivityId)
                    .HasConstraintName("fk_activityInteraction_contactActivityId");

                entity.HasOne(d => d.InteractionAccount)
                    .WithMany(p => p.Contactactivityinteractions)
                    .HasForeignKey(d => d.InteractionAccountId)
                    .HasConstraintName("fk_activityInteraction_interaction_companyId");

                entity.HasOne(d => d.InteractionEntity)
                    .WithMany(p => p.Contactactivityinteractions)
                    .HasForeignKey(d => d.InteractionEntityId)
                    .HasConstraintName("fk_activityInteraction_interaction_entityId");
            });

            modelBuilder.Entity<Contactrole>(entity =>
            {
                entity.ToTable("contactrole");

                entity.Property(e => e.ContactRoleId).HasColumnType("int(11)");

                entity.Property(e => e.Active).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");
            });

            
            modelBuilder.Entity<Contacttoken>(entity =>
            {
                entity.ToTable("contacttoken");

                entity.Property(e => e.ContactTokenId).HasColumnType("int(11)");

                entity.Property(e => e.Create).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Expire).HasColumnType("datetime");

                entity.Property(e => e.IpAddress)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Token)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Containeraccess>(entity =>
            {
                entity.ToTable("containeraccess");

                entity.HasIndex(e => e.ContainerId, "fk_containeraccess_conatiner_id_idx");

                entity.HasIndex(e => e.GroupId, "fk_containeraccess_groupid_idx");

                entity.HasIndex(e => e.MembershipTypeId, "fk_containeraccess_membershipType_id_idx");

                entity.HasIndex(e => e.StaffRoleId, "fk_containeraccess_staffuser_id_idx");

                entity.Property(e => e.ContainerAccessId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.ContainerId).HasColumnType("int(11)");

                entity.Property(e => e.GroupId).HasColumnType("int(11)");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.StaffRoleId).HasColumnType("int(11)");

                entity.HasOne(d => d.Container)
                    .WithMany(p => p.Containeraccesses)
                    .HasForeignKey(d => d.ContainerId)
                    .HasConstraintName("fk_containeraccess_conatiner_id");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Containeraccesses)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("fk_containeraccess_groupid");

                entity.HasOne(d => d.MembershipType)
                    .WithMany(p => p.Containeraccesses)
                    .HasForeignKey(d => d.MembershipTypeId)
                    .HasConstraintName("fk_containeraccess_membershipType_id");

                entity.HasOne(d => d.StaffRole)
                    .WithMany(p => p.Containeraccesses)
                    .HasForeignKey(d => d.StaffRoleId)
                    .HasConstraintName("containeraccess_ibfk_1");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("countries");

                entity.Property(e => e.CountryId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(150)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PhoneCode).HasColumnType("int(11)");

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasColumnType("varchar(3)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ZipFormat)
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Credittransaction>(entity =>
            {
                entity.ToTable("credittransaction");

                entity.HasIndex(e => e.ReceiptDetailId, "fk_creditTransaction_receiptdetail_id_idx");

                entity.HasIndex(e => e.EntityId, "fk_credittransaction_entityId_idx");

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

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.EntryType).HasColumnType("int(11)");

                entity.Property(e => e.ExpirDate).HasColumnType("datetime");

                entity.Property(e => e.Reason)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReceiptDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Credittransactions)
                    .HasForeignKey(d => d.EntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_creditTransaction_entityid");

                entity.HasOne(d => d.ReceiptDetail)
                    .WithMany(p => p.Credittransactions)
                    .HasForeignKey(d => d.ReceiptDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_creditTransaction_receiptdetail_id");
            });

            modelBuilder.Entity<Customfield>(entity =>
            {
                entity.ToTable("customfield");

                entity.HasIndex(e => e.FieldTypeId, "FieldTypeId");

                entity.Property(e => e.CustomFieldId).HasColumnType("int(11)");

                entity.Property(e => e.CharacterLimit).HasColumnType("int(11)");

                entity.Property(e => e.CountryCode).HasColumnType("int(11)");

                entity.Property(e => e.DateFormat)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DefaultDate)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DefaultTime)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Editable).HasColumnType("int(11)");

                entity.Property(e => e.FieldTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Label)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ModuleId).HasColumnType("int(11)");

                entity.Property(e => e.MultipleSelection).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Placeholder)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Required).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TimeFormat)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Validations)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("validations")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.Customfields)
                    .HasForeignKey(d => d.FieldTypeId)
                    .HasConstraintName("customfield_ibfk_1");
            });

            modelBuilder.Entity<Customfielddatum>(entity =>
            {
                entity.ToTable("customfielddata");

                entity.HasIndex(e => e.CustomFieldId, "CustomFieldId");

                entity.HasIndex(e => e.EntityId, "EntityId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CustomFieldId).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.CustomField)
                    .WithMany(p => p.Customfielddata)
                    .HasForeignKey(d => d.CustomFieldId)
                    .HasConstraintName("customfielddata_ibfk_1");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Customfielddata)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("customfielddata_ibfk_2");
            });

            modelBuilder.Entity<Customfieldlookup>(entity =>
            {
                entity.ToTable("customfieldlookup");

                entity.HasIndex(e => e.CustomFieldId, "CustomFieldId");

                entity.HasIndex(e => e.ModuleId, "ModuleId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CustomFieldId).HasColumnType("int(11)");

                entity.Property(e => e.ModuleId).HasColumnType("int(11)");

                entity.Property(e => e.OrderOfDisplay).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TabId).HasColumnType("int(11)");

                entity.HasOne(d => d.CustomField)
                    .WithMany(p => p.Customfieldlookups)
                    .HasForeignKey(d => d.CustomFieldId)
                    .HasConstraintName("customfieldlookup_ibfk_1");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.Customfieldlookups)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("customfieldlookup_ibfk_2");
            });

            modelBuilder.Entity<Customfieldoption>(entity =>
            {
                entity.HasKey(e => e.OptionId)
                    .HasName("PRIMARY");

                entity.ToTable("customfieldoption");

                entity.HasIndex(e => e.CustomFieldId, "CustomFieldId");

                entity.Property(e => e.OptionId).HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("code")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CustomFieldId).HasColumnType("int(11)");

                entity.Property(e => e.Option)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.CustomField)
                    .WithMany(p => p.Customfieldoptions)
                    .HasForeignKey(d => d.CustomFieldId)
                    .HasConstraintName("customfieldoption_ibfk_1");
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

                entity.HasIndex(e => e.EntityId, "fk_document_entity_id_idx");

                entity.Property(e => e.DocumentId).HasColumnType("int(11)");

                entity.Property(e => e.ContentType)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DisplayFileName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.EventBannerImageId).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.FileName)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FilePath)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.StaffId).HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_department_organizationId_id");
            });

            modelBuilder.Entity<Documentaccess>(entity =>
            {
                entity.ToTable("documentaccess");

                entity.HasIndex(e => e.StaffRoleId, "fk_documentStaff_Roles_idx");

                entity.HasIndex(e => e.DocumentObjectId, "fk_documentaccess_documentObject_id_idx");

                entity.HasIndex(e => e.GroupId, "fk_documentaccess_group_id_idx");

                entity.HasIndex(e => e.MembershipTypeId, "fk_documentmembership_type_id_idx");

                entity.Property(e => e.DocumentAccessId).HasColumnType("int(11)");

                entity.Property(e => e.DocumentObjectId).HasColumnType("int(11)");

                entity.Property(e => e.GroupId).HasColumnType("int(11)");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.StaffRoleId).HasColumnType("int(11)");

                entity.HasOne(d => d.DocumentObject)
                    .WithMany(p => p.Documentaccesses)
                    .HasForeignKey(d => d.DocumentObjectId)
                    .HasConstraintName("fk_access_documentObject_id");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Documentaccesses)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("fk_documentobject_group_id");

                entity.HasOne(d => d.MembershipType)
                    .WithMany(p => p.Documentaccesses)
                    .HasForeignKey(d => d.MembershipTypeId)
                    .HasConstraintName("fk_documentobject_membership_type_id");

                entity.HasOne(d => d.StaffRole)
                    .WithMany(p => p.Documentaccesses)
                    .HasForeignKey(d => d.StaffRoleId)
                    .HasConstraintName("fk_documentStaff_Roles");
            });

            modelBuilder.Entity<Documentcontainer>(entity =>
            {
                entity.HasKey(e => e.ContainerId)
                    .HasName("PRIMARY");

                entity.ToTable("documentcontainer");

                entity.HasIndex(e => e.CreatedBy, "fk_documentconatiner_staff_user_id_idx");

                entity.Property(e => e.ContainerId).HasColumnType("int(11)");

                entity.Property(e => e.AccessControlEnabled).HasColumnType("int(11)");

                entity.Property(e => e.BlobContainerId)
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CreatedBy).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EncryptionKey)
                    .HasColumnType("varchar(256)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Documentcontainers)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_documentconatiner_staff_user_id");
            });

            modelBuilder.Entity<Documentobject>(entity =>
            {
                entity.ToTable("documentobject");

                entity.HasIndex(e => e.ContainerId, "fk_documentobject_container_idx");

                entity.HasIndex(e => e.CreatedBy, "fk_documentobject_staffuserid_idx");

                entity.Property(e => e.DocumentObjectId).HasColumnType("int(11)");

                entity.Property(e => e.Active).HasColumnType("int(11)");

                entity.Property(e => e.BlobId)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ContainerId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FileName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.FileSize).HasColumnType("bigint(20)");

                entity.Property(e => e.FileType).HasColumnType("int(11)");

                entity.Property(e => e.PathName)
                    .HasColumnType("varchar(2048)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Container)
                    .WithMany(p => p.Documentobjects)
                    .HasForeignKey(d => d.ContainerId)
                    .HasConstraintName("fk_documentobject_container");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Documentobjects)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_documentobject_staffuserid");
            });
            modelBuilder.Entity<Documentobjectaccesshistory>(entity =>
            {
                entity.HasKey(e => e.AccessId)
                    .HasName("PRIMARY");

                entity.ToTable("documentobjectaccesshistory");

                entity.HasIndex(e => e.EntityId, "fk_documentaccess_entityId_idx");

                entity.HasIndex(e => e.DocumentObjectId, "fk_documentaccess_object_id_idx");

                entity.HasIndex(e => e.StaffUserId, "fk_documentaccess_staffId_idx");

                entity.Property(e => e.AccessId).HasColumnType("int(11)");

                entity.Property(e => e.AccessDate).HasColumnType("datetime");

                entity.Property(e => e.AccessType).HasColumnType("int(11)");

                entity.Property(e => e.DocumentObjectId).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.StaffUserId).HasColumnType("int(11)");

                entity.HasOne(d => d.DocumentObject)
                    .WithMany(p => p.Documentobjectaccesshistories)
                    .HasForeignKey(d => d.DocumentObjectId)
                    .HasConstraintName("fk_documentaccess_object_id");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Documentobjectaccesshistories)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_documentaccess_entityId");

                entity.HasOne(d => d.StaffUser)
                    .WithMany(p => p.Documentobjectaccesshistories)
                    .HasForeignKey(d => d.StaffUserId)
                    .HasConstraintName("fk_documentaccess_staffId");
            });

            modelBuilder.Entity<Documentsearchhistory>(entity =>
            {
                entity.HasKey(e => e.DocumentSearchId)
                    .HasName("PRIMARY");

                entity.ToTable("documentsearchhistory");

                entity.Property(e => e.DocumentSearchId).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.IpAddress)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.SearchDate).HasColumnType("datetime");

                entity.Property(e => e.SearchText)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Documenttag>(entity =>
            {
                entity.ToTable("documenttag");

                entity.HasIndex(e => e.DocumentObjectId, "fk_tag_documentobject_id_idx");

                entity.HasIndex(e => e.TagId, "fk_tag_id_idx");

                entity.Property(e => e.DocumentTagId).HasColumnType("int(11)");

                entity.Property(e => e.DocumentObjectId).HasColumnType("int(11)");

                entity.Property(e => e.TagId).HasColumnType("int(11)");

                entity.Property(e => e.TagValue)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.DocumentObject)
                    .WithMany(p => p.Documenttags)
                    .HasForeignKey(d => d.DocumentObjectId)
                    .HasConstraintName("fk_tag_documentobject_id");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.Documenttags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("fk_tag_id");
            });

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId)
                    .HasColumnType("varchar(150)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.ToTable("email");

                entity.HasIndex(e => e.CompanyId, "fk_email_company_id_idx");

                entity.HasIndex(e => e.PersonId, "fk_email_person_id_idx");

                entity.Property(e => e.EmailId).HasColumnType("int(11)");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.EmailAddress)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EmailAddressType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsPrimary).HasColumnType("int(11)");

                entity.Property(e => e.PersonId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Emails)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_email_company_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Emails)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_email_person_id");
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.ToTable("entity");

                entity.HasIndex(e => e.OrganizationId, "fk_entity_organization_id_idx");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.AccountLocked).HasColumnType("int(11)");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.PasswordFailedAttempts).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PortalLastAccessed).HasColumnType("datetime");

                entity.Property(e => e.PreferredBillingCommunication).HasColumnType("int(11)");

                entity.Property(e => e.ProfilePictureId).HasColumnType("int(11)");

                entity.Property(e => e.SociableProfileId).HasColumnType("int(11)");

                entity.Property(e => e.SociableUserId).HasColumnType("int(11)");

                entity.Property(e => e.WebLoginName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.WebPassword)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.WebPasswordSalt)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Entities)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_entity_organization_id");
            });

            modelBuilder.Entity<Entityrole>(entity =>
            {
                entity.ToTable("entityrole");

                entity.HasIndex(e => e.CompanyId, "fk_entityrole_companyId_idx");

                entity.HasIndex(e => e.ContactRoleId, "fk_entityrole_contactroleId_idx");

                entity.HasIndex(e => e.EntityId, "fk_entityrole_entityId_idx");

                entity.Property(e => e.EntityRoleId).HasColumnType("int(11)");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.ContactRoleId).HasColumnType("int(11)");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Entityroles)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_entityrole_companyId");

                entity.HasOne(d => d.ContactRole)
                    .WithMany(p => p.Entityroles)
                    .HasForeignKey(d => d.ContactRoleId)
                    .HasConstraintName("fk_entityrole_contactroleId");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Entityroles)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_entityrole_entityId");
            });

            modelBuilder.Entity<Entityrolehistory>(entity =>
            {
                entity.ToTable("entityrolehistory");

                entity.HasIndex(e => e.CompanyId, "fk_entityhistory_companyId_idx");

                entity.HasIndex(e => e.ContactRoleId, "fk_entityhistory_contactroleId_idx");

                entity.HasIndex(e => e.EntityId, "fk_entityhistory_entityid_idx");

                entity.HasIndex(e => e.StaffUserId, "fk_entityhistory_staffuserId_idx");

                entity.Property(e => e.EntityRoleHistoryId).HasColumnType("int(11)");

                entity.Property(e => e.ActivityDate).HasColumnType("datetime");

                entity.Property(e => e.ActivityType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.ContactRoleId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.StaffUserId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Entityrolehistories)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_entityhistory_companyId");

                entity.HasOne(d => d.ContactRole)
                    .WithMany(p => p.Entityrolehistories)
                    .HasForeignKey(d => d.ContactRoleId)
                    .HasConstraintName("fk_entityhistory_contactroleId");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Entityrolehistories)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_entityhistory_entityid");

                entity.HasOne(d => d.StaffUser)
                    .WithMany(p => p.Entityrolehistories)
                    .HasForeignKey(d => d.StaffUserId)
                    .HasConstraintName("fk_entityhistory_staffuserId");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("event");

                entity.HasIndex(e => e.EventTypeId, "fk_eventtype_id_idx");

                entity.HasIndex(e => e.OrganizationId, "fk_organization_id_idx");

                entity.HasIndex(e => e.TimeZoneId, "fk_timezone_id_idx");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.AllowMultipleRegistration).HasColumnType("int(11)");

                entity.Property(e => e.AllowNonMember).HasColumnType("int(11)");

                entity.Property(e => e.AllowWaitlist).HasColumnType("int(11)");

                entity.Property(e => e.Area)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.City)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Country)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("longtext")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.EventBannerImageId).HasColumnType("int(11)");

                entity.Property(e => e.EventImageId).HasColumnType("int(11)");

                entity.Property(e => e.EventTypeId).HasColumnType("int(11)");

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.MaxCapacity).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.RegEndDate).HasColumnType("datetime");

                entity.Property(e => e.RegStartDate).HasColumnType("datetime");

                entity.Property(e => e.ShowEventCode).HasColumnType("int(11)");

                entity.Property(e => e.State)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Summary)
                    .HasColumnType("varchar(300)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TimeZoneId).HasColumnType("int(11)");

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.Property(e => e.WebinarLiveLink)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.WebinarRecordedLink)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Zip)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.EventType)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.EventTypeId)
                    .HasConstraintName("fk_eventtype_id");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("fk_organization_id");

                entity.HasOne(d => d.TimeZone)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.TimeZoneId)
                    .HasConstraintName("fk_timezone_id");
            });

            modelBuilder.Entity<Eventcontact>(entity =>
            {
                entity.ToTable("eventcontact");

                entity.HasIndex(e => e.EventId, "fk_eventcontact_eventid_idx");

                entity.HasIndex(e => e.StaffId, "fk_eventcontact_staffid_idx");

                entity.Property(e => e.EventContactId).HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.HidePhoneNumber).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.StaffId).HasColumnType("int(11)");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Eventcontacts)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("fk_eventcontact_eventid");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Eventcontacts)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("fk_eventcontact_staffid");
            });

            modelBuilder.Entity<Eventregister>(entity =>
            {
                entity.ToTable("eventregister");

                entity.HasIndex(e => e.EntityId, "FK_Entity");

                entity.HasIndex(e => e.EventId, "FK_Event");

                entity.Property(e => e.EventRegisterId).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Eventregisters)
                    .HasForeignKey(d => d.EntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Entity");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Eventregisters)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Event");
            });

            modelBuilder.Entity<Eventregisterquestion>(entity =>
            {
                entity.ToTable("eventregisterquestion");

                entity.HasIndex(e => e.AnswerOptionId, "FK_AnswerOption");

                entity.HasIndex(e => e.EventRegisterId, "FK_EventRegisteration");

                entity.HasIndex(e => e.EventId, "FK_Events_idx");

                entity.HasIndex(e => e.QuestionId, "FK_QuestionBank");

                entity.HasIndex(e => e.SessionId, "FK_Sessionons");

                entity.Property(e => e.EventRegisterQuestionId).HasColumnType("int(11)");

                entity.Property(e => e.Answer)
                    .HasColumnType("longtext")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.AnswerOptionId).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.EventRegisterId).HasColumnType("int(11)");

                entity.Property(e => e.QuestionId).HasColumnType("int(11)");

                entity.Property(e => e.SessionId).HasColumnType("int(11)");

                entity.HasOne(d => d.AnswerOption)
                    .WithMany(p => p.Eventregisterquestions)
                    .HasForeignKey(d => d.AnswerOptionId)
                    .HasConstraintName("FK_AnswerOption");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Eventregisterquestions)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_Events");

                entity.HasOne(d => d.EventRegister)
                    .WithMany(p => p.Eventregisterquestions)
                    .HasForeignKey(d => d.EventRegisterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventRegisteration");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Eventregisterquestions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionBank");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Eventregisterquestions)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("FK_Sessionons");
            });

            modelBuilder.Entity<Eventregistersession>(entity =>
            {
                entity.ToTable("eventregistersession");

                entity.HasIndex(e => e.EventRegisterId, "FK_EventRegister");

                entity.HasIndex(e => e.SessionId, "FK_Session");

                entity.Property(e => e.EventRegisterSessionId).HasColumnType("int(11)");

                entity.Property(e => e.EventRegisterId).HasColumnType("int(11)");

                entity.Property(e => e.Price).HasPrecision(13, 2);

                entity.Property(e => e.SessionId).HasColumnType("int(11)");

                entity.HasOne(d => d.EventRegister)
                    .WithMany(p => p.Eventregistersessions)
                    .HasForeignKey(d => d.EventRegisterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventRegister");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Eventregistersessions)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Session");
            });

            modelBuilder.Entity<Eventreport>(entity =>
            {
                entity.ToTable("eventreport");

                entity.HasIndex(e => e.EventReportId, "fk_eventreport_report_id_idx");

                entity.Property(e => e.EventReportId).HasColumnType("int(11)");

                entity.Property(e => e.Event)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Sessions)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Eventreports)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_eventreport_report_id");
            });

            modelBuilder.Entity<Eventtype>(entity =>
            {
                entity.ToTable("eventtype");

                entity.Property(e => e.EventTypeId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.EventType1)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("EventType")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Fieldtype>(entity =>
            {
                entity.ToTable("fieldtype");

                entity.Property(e => e.FieldTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
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

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("group");

                entity.Property(e => e.GroupId).HasColumnType("int(11)");

                entity.Property(e => e.ApplyTerm).HasColumnType("int(11)");

                entity.Property(e => e.GroupDescription)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.GroupName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.PreferredNumbers).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Sync).HasColumnType("int(11)");

                entity.Property(e => e.TermEndDate).HasColumnType("datetime");

                entity.Property(e => e.TerrmStartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Grouphistory>(entity =>
            {
                entity.ToTable("grouphistory");

                entity.Property(e => e.GroupHistoryId).HasColumnType("int(11)");

                entity.Property(e => e.ActivityDate).HasColumnType("datetime");

                entity.Property(e => e.ActivityDescription)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ActivityStaffId).HasColumnType("int(11)");

                entity.Property(e => e.ActivityType)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.GroupId).HasColumnType("int(11)");

                entity.Property(e => e.GroupMemberId).HasColumnType("int(11)");

                entity.Property(e => e.GroupRoleId).HasColumnType("int(11)");

                entity.Property(e => e.LinkGroupRoleId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Groupmember>(entity =>
            {
                entity.ToTable("groupmember");

                entity.HasIndex(e => e.EntityId, "fk_gm_entityId_idx");

                entity.HasIndex(e => e.GroupId, "fk_gm_groupId_idx");

                entity.Property(e => e.GroupMemberId).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.GroupId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Groupmembers)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_gm_entityId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Groupmembers)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("fk_gm_groupId");
            });

            modelBuilder.Entity<Groupmemberrole>(entity =>
            {
                entity.ToTable("groupmemberrole");

                entity.HasIndex(e => e.GroupMemberId, "fk_groupmember_id_idx");

                entity.HasIndex(e => e.GroupRoleId, "fk_role_id_idx");

                entity.Property(e => e.GroupMemberRoleId).HasColumnType("int(11)");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.GroupMemberId).HasColumnType("int(11)");

                entity.Property(e => e.GroupRoleId).HasColumnType("int(11)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.Groupmemberroles)
                    .HasForeignKey(d => d.GroupMemberId)
                    .HasConstraintName("fk_groupmember_id");

                entity.HasOne(d => d.GroupRole)
                    .WithMany(p => p.Groupmemberroles)
                    .HasForeignKey(d => d.GroupRoleId)
                    .HasConstraintName("fk_grouprole_roleid");
            });

            modelBuilder.Entity<Grouprole>(entity =>
            {
                entity.ToTable("grouprole");

                entity.HasIndex(e => e.OrganizationId, "fk_organizationid_idx");

                entity.Property(e => e.GroupRoleId).HasColumnType("int(11)");

                entity.Property(e => e.GroupRoleName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.OrganizationId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoice");

                entity.HasIndex(e => e.BillableEntityId, "fk_invoice_billableentity_id_idx");

                entity.HasIndex(e => e.EntityId, "fk_invoice_entityId_id_idx");

                entity.HasIndex(e => e.EventId, "fk_invoice_event_id_idx");

                entity.HasIndex(e => e.MembershipId, "fk_invoice_membership_id_idx");

                entity.HasIndex(e => e.PaymentTransactionId, "fk_invoice_paymenttransaction_id_idx");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.BillableEntityId).HasColumnType("int(11)");

                entity.Property(e => e.BillingType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("date");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

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

                entity.Property(e => e.PromoCodeId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.BillableEntity)
                    .WithMany(p => p.InvoiceBillableEntities)
                    .HasForeignKey(d => d.BillableEntityId)
                    .HasConstraintName("fk_invoice_billableentity_id");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.InvoiceEntities)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_invoice_entityId_id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("fk_invoice_event_id");

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

                entity.Property(e => e.BillableEntityId).HasColumnType("int(11)");

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

                entity.Property(e => e.EnableMemberPortal)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.EnableStock)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

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

                entity.Property(e => e.StockCount)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TotalStock)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

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
                    .OnDelete(DeleteBehavior.Cascade)
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
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_journalentryheader_receipt_Id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Journalentryheaders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journalentryheader_userId");
            });

            modelBuilder.Entity<Linkeventfeetype>(entity =>
            {
                entity.ToTable("linkeventfeetype");

                entity.HasIndex(e => e.EventId, "fk_left_eventid_idx");

                entity.HasIndex(e => e.RegistrationFeeTypeId, "fk_left_regfeetypeid_idx");

                entity.Property(e => e.LinkEventFeeTypeId).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.RegistrationFeeTypeId).HasColumnType("int(11)");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Linkeventfeetypes)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("fk_left_eventid");

                entity.HasOne(d => d.RegistrationFeeType)
                    .WithMany(p => p.Linkeventfeetypes)
                    .HasForeignKey(d => d.RegistrationFeeTypeId)
                    .HasConstraintName("fk_left_regfeetypeid");
            });

            modelBuilder.Entity<Linkeventgroup>(entity =>
            {
                entity.ToTable("linkeventgroup");

                entity.HasIndex(e => e.EventId, "fk_leg_eventid_idx");

                entity.HasIndex(e => e.RegistrationGroupId, "fk_leg_reggroupid_idx");

                entity.Property(e => e.LinkEventGroupId).HasColumnType("int(11)");

                entity.Property(e => e.EnableOnlineRegistration).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.RegistrationGroupId).HasColumnType("int(11)");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Linkeventgroups)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("fk_leg_eventid");

                entity.HasOne(d => d.RegistrationGroup)
                    .WithMany(p => p.Linkeventgroups)
                    .HasForeignKey(d => d.RegistrationGroupId)
                    .HasConstraintName("fk_leg_reggroupid");
            });

            modelBuilder.Entity<Linkgrouprole>(entity =>
            {
                entity.ToTable("linkgrouprole");

                entity.HasIndex(e => e.GroupId, "fk_group_id_idx");

                entity.HasIndex(e => e.GroupRoleId, "fk_grouprole_id_idx");

                entity.Property(e => e.LinkGroupRoleId).HasColumnType("int(11)");

                entity.Property(e => e.GroupId).HasColumnType("int(11)");

                entity.Property(e => e.GroupRoleId).HasColumnType("int(11)");

                entity.Property(e => e.GroupRoleName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.IsLinked).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Linkgrouproles)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("fk_group_id");

                entity.HasOne(d => d.GroupRole)
                    .WithMany(p => p.Linkgrouproles)
                    .HasForeignKey(d => d.GroupRoleId)
                    .HasConstraintName("fk_grouprole_id");
            });

            modelBuilder.Entity<Linkregistrationgroupfee>(entity =>
            {
                entity.ToTable("linkregistrationgroupfee");

                entity.HasIndex(e => e.RegistrationFeeTypeId, "fk_lrgd_feeid_idx");

                entity.HasIndex(e => e.RegistrationGroupId, "fk_lrgd_groupid_idx");

                entity.HasIndex(e => e.LinkEventGroupId, "fk_lrgd_linkeventgroupid_idx");

                entity.Property(e => e.LinkRegistrationGroupFeeId).HasColumnType("int(11)");

                entity.Property(e => e.LinkEventGroupId).HasColumnType("int(11)");

                entity.Property(e => e.RegistrationFeeTypeId).HasColumnType("int(11)");

                entity.Property(e => e.RegistrationGroupDateTime).HasColumnType("datetime");

                entity.Property(e => e.RegistrationGroupEndDateTime).HasColumnType("datetime");

                entity.Property(e => e.RegistrationGroupId).HasColumnType("int(11)");

                entity.HasOne(d => d.LinkEventGroup)
                    .WithMany(p => p.Linkregistrationgroupfees)
                    .HasForeignKey(d => d.LinkEventGroupId)
                    .HasConstraintName("fk_lrgd_linkeventgroupid");

                entity.HasOne(d => d.RegistrationFeeType)
                    .WithMany(p => p.Linkregistrationgroupfees)
                    .HasForeignKey(d => d.RegistrationFeeTypeId)
                    .HasConstraintName("fk_lrgd_feeid");

                entity.HasOne(d => d.RegistrationGroup)
                    .WithMany(p => p.Linkregistrationgroupfees)
                    .HasForeignKey(d => d.RegistrationGroupId)
                    .HasConstraintName("fk_lrgd_groupid");
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

                entity.HasIndex(e => e.BillableEntityId, "fk_membership_billableEntityId_idx");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.AutoPayEnabled).HasColumnType("int(11)");

                entity.Property(e => e.BillableEntityId).HasColumnType("int(11)");

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

                entity.HasOne(d => d.BillableEntity)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.BillableEntityId)
                    .HasConstraintName("fk_membership_billableEntity_id");

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
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Membershipconnection>(entity =>
            {
                entity.ToTable("membershipconnection");

                entity.HasIndex(e => e.EntityId, "fk_membershiprelation_entity_idx");

                entity.HasIndex(e => e.MembershipId, "fk_membershiprelationship_membership_id_idx");

                entity.Property(e => e.MembershipConnectionId).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Membershipconnections)
                    .HasForeignKey(d => d.EntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_membershiprelation_entity");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Membershipconnections)
                    .HasForeignKey(d => d.MembershipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_membershiprelationship_membership_id");
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

            modelBuilder.Entity<Membershipreport>(entity =>
            {
                entity.ToTable("membershipreport");

                entity.HasIndex(e => e.ReportId, "fk_membershipreport_report_id_idx");

                entity.Property(e => e.MembershipReportId).HasColumnType("int(11)");

                entity.Property(e => e.Categories)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.MembershipTypes)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReportId).HasColumnType("int(11");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Membershipreports)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("fk_membershipreport_report_id");
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

                entity.Property(e => e.PaymentFrequency).HasColumnType("int(11)");

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

            modelBuilder.Entity<Moduleinfo>(entity =>
            {
                entity.HasKey(e => e.ModuleId)
                    .HasName("PRIMARY");

                entity.ToTable("moduleinfo");

                entity.Property(e => e.ModuleId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Multifactorcode>(entity =>
            {
                entity.ToTable("multifactorcode");

                entity.HasIndex(e => e.EntityId, "fk_multifactorcode_entity_id_idx");

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

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

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

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Multifactorcodes)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_multifactorcode_entity_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Multifactorcodes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_multifactorcode_staffuser_id");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("note");

                entity.HasIndex(e => e.EntityId, "fk_note_entityid_idx");

                entity.Property(e => e.NoteId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DisplayOnProfile).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

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

                entity.Property(e => e.Severity)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_note_entityid");
            });

            modelBuilder.Entity<Opportunity>(entity =>
            {
                entity.ToTable("opportunity");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasIndex(e => e.AccountContactId, "fk_opportunity_contact_id_idx");

                entity.HasIndex(e => e.PipelineId, "fk_opportunity_pipeline_id_idx");

                entity.HasIndex(e => e.StaffUserId, "fk_opportunity_staff_user_id_idx");
                entity.HasIndex(e => e.CompanyId, "fk_opportunity_company_id_idx");

                entity.Property(e => e.OpportunityId).HasColumnType("int(11)");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.AccountContactId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EstimatedCloseDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(3000);

                entity.Property(e => e.PipelineId).HasColumnType("int(11)");

                entity.Property(e => e.Potential).HasPrecision(13, 2);

                entity.Property(e => e.Probability).HasColumnType("int(11)");

                entity.Property(e => e.ProductId).HasColumnType("int(11)");

                entity.Property(e => e.StaffUserId).HasColumnType("int(11)");

                entity.Property(e => e.StageId).HasColumnType("int(11)");

                entity.HasOne(d => d.AccountContact)
                    .WithMany(p => p.Opportunities)
                    .HasForeignKey(d => d.AccountContactId)
                    .HasConstraintName("fk_opportunity_contact_id");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Opportunities)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_opportunity_company_id");

                entity.HasOne(d => d.Pipeline)
                    .WithMany(p => p.Opportunities)
                    .HasForeignKey(d => d.PipelineId)
                    .HasConstraintName("fk_opportunity_pipeline_id");

                entity.HasOne(d => d.StaffUser)
                    .WithMany(p => p.Opportunities)
                    .HasForeignKey(d => d.StaffUserId)
                    .HasConstraintName("fk_opportunity_staff_user_id");
            });

            modelBuilder.Entity<Opportunitypipeline>(entity =>
            {
                entity.HasKey(e => e.PipelineId)
                    .HasName("PRIMARY");

                entity.ToTable("opportunitypipeline");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PipelineId).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Opportunityreport>(entity =>
            {
                entity.ToTable("opportunityreport");

                entity.HasIndex(e => e.OpportunityReportId, "fk_opportunityreport_report_id_idx");

                entity.Property(e => e.OpportunityReportId).HasColumnType("int(11)");

                entity.Property(e => e.Pipeline)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Stages)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Products)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Opportunityreports)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_opportunityreport_report_id");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("organization");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.AccountName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

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

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(100)")
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

                entity.HasIndex(e => e.EntityId, "fk_paper_invoice_entity_id_idx");

                entity.HasIndex(e => e.InvoiceId, "fk_paper_invoice_invoice_id_idx");

                entity.Property(e => e.PaperInvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(14, 2);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.PaperBillingCycleId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Paperinvoices)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_paper_invoice_entity_id");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Paperinvoices)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("fk_paper_invoice_invoice_id");

                entity.HasOne(d => d.PaperBillingCycle)
                    .WithMany(p => p.Paperinvoices)
                    .HasForeignKey(d => d.PaperBillingCycleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_paper_invoice_billing_cycle_id");
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

                entity.Property(e => e.LiveAcceptJsurl)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("LiveAcceptJSURL")
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

                entity.Property(e => e.TestAcceptJsurl)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("TestAcceptJSURL")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

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

                entity.HasIndex(e => e.EntityId, "fk_paymentProfileId_entity_id_idx");

                entity.Property(e => e.PaymentProfileId).HasColumnType("int(11)");

                entity.Property(e => e.AccountNumber)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AccountType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AuthNetPaymentProfileId)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CardHolderName)
                    .HasColumnType("varchar(100)")
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

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameOnAccount)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NickName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PreferredPaymentMethod).HasColumnType("int(11)");

                entity.Property(e => e.ProfileId)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RoutingNumber)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.UseForAutobilling).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Paymentprofiles)
                    .HasForeignKey(d => d.EntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_paymentProfileId_entity_id");
            });

            modelBuilder.Entity<Paymenttransaction>(entity =>
            {
                entity.ToTable("paymenttransaction");

                entity.HasIndex(e => e.ReceiptId, "fk_paymenttransaction_recei[pt_id_idx");

                entity.HasIndex(e => e.EntityId, "fk_pt_entityid_idx");

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

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.IsAdjusted).HasColumnType("int(11)");

                entity.Property(e => e.MessageDetails)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NickName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PaymentType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

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

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Paymenttransactions)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_pt_entityid");

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.Paymenttransactions)
                    .HasForeignKey(d => d.ReceiptId)
                    .HasConstraintName("fk_paymenttransaction_receipt_id");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("person");

                entity.HasIndex(e => e.CompanyId, "fk_person_comapny_id_idx");

                entity.HasIndex(e => e.EntityId, "fk_person_entity_id_idx");

                entity.HasIndex(e => e.OrganizationId, "fk_person_organization_id_idx");

                entity.HasIndex(e => e.FirstName, "idx_person_first_name");

                entity.HasIndex(e => e.LastName, "idx_person_last_name");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.CasualName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Designation)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.FacebookName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.InstagramName)
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

                entity.Property(e => e.Website)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.People)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_person_comapny_id");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.People)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_person_entity_id");

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

                entity.HasIndex(e => e.CompanyId, "fk_company_id_idx");

                entity.HasIndex(e => e.PersonId, "fk_phone_person_id_idx");

                entity.Property(e => e.PhoneId).HasColumnType("int(11)");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

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

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("fk_company_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_phone_person_id");
            });
            modelBuilder.Entity<Pipelineproduct>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PRIMARY");

                entity.ToTable("pipelineproduct");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasIndex(e => e.PipelineId, "fk_product_pipelineId_idx");

                entity.Property(e => e.ProductId).HasColumnType("int(11)");

                entity.Property(e => e.PipelineId).HasColumnType("int(11)");

                entity.Property(e => e.ProductIndex).HasColumnType("int(11)");

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Pipeline)
                    .WithMany(p => p.Pipelineproducts)
                    .HasForeignKey(d => d.PipelineId)
                    .HasConstraintName("fk_product_pipelineId");
            });

            modelBuilder.Entity<Pipelinestage>(entity =>
            {
                entity.HasKey(e => e.StageId)
                    .HasName("PRIMARY");

                entity.ToTable("pipelinestage");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasIndex(e => e.PipelineId, "fk_stage_pipilineId_idx");

                entity.Property(e => e.StageId).HasColumnType("int(11)");

                entity.Property(e => e.PipelineId).HasColumnType("int(11)");

                entity.Property(e => e.Probability).HasColumnType("int(11)");

                entity.Property(e => e.StageIndex).HasColumnType("int(11)");

                entity.Property(e => e.StageName).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Pipeline)
                    .WithMany(p => p.Pipelinestages)
                    .HasForeignKey(d => d.PipelineId)
                    .HasConstraintName("fk_stage_pipilineId");
            });

            modelBuilder.Entity<Pricingforlookup>(entity =>
            {
                entity.ToTable("pricingforlookup");

                entity.Property(e => e.PricingForLookUpId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.PricingFor)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
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

            modelBuilder.Entity<Questionbank>(entity =>
            {
                entity.ToTable("questionbank");

                entity.HasIndex(e => e.AnswerTypeLookUpId, "fk_qb_answertype_lookup_id_idx");

                entity.Property(e => e.QuestionBankId).HasColumnType("int(11)");

                entity.Property(e => e.AnswerTypeLookUpId).HasColumnType("int(11)");

                entity.Property(e => e.Question)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.AnswerTypeLookUp)
                    .WithMany(p => p.Questionbanks)
                    .HasForeignKey(d => d.AnswerTypeLookUpId)
                    .HasConstraintName("fk_qb_answertype_lookup_id");
            });

            modelBuilder.Entity<Questionlink>(entity =>
            {
                entity.ToTable("questionlink");

                entity.HasIndex(e => e.QuestionBankId, "fk_el_questionbank_id_idx");

                entity.HasIndex(e => e.SessionId, "fk_el_session_id_idx");

                entity.HasIndex(e => e.EventId, "fk_ql_event_id_idx");

                entity.Property(e => e.QuestionLinkId).HasColumnType("int(11)");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.QuestionBankId).HasColumnType("int(11)");

                entity.Property(e => e.SessionId).HasColumnType("int(11)");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Questionlinks)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("fk_ql_event_id");

                entity.HasOne(d => d.QuestionBank)
                    .WithMany(p => p.Questionlinks)
                    .HasForeignKey(d => d.QuestionBankId)
                    .HasConstraintName("fk_el_questionbank_id");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Questionlinks)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("fk_el_session_id");
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

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.InvoiceDetailId).HasColumnType("int(11)");

                entity.Property(e => e.ItemType).HasColumnType("int(11)");

                entity.Property(e => e.PastDueInvoiceDetailRef).HasColumnType("int(11)");

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

                entity.HasIndex(e => e.BillableEntityId, "fk_billableEntityId_idx");

                entity.HasIndex(e => e.OrganizationId, "fk_organizationid_idx");

                entity.HasIndex(e => e.StaffId, "fk_staffid_idx");

                entity.Property(e => e.Receiptid).HasColumnType("int(11)");

                entity.Property(e => e.BillableEntityId).HasColumnType("int(11)");

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

                entity.Property(e => e.Portal).HasColumnType("int(11)");

                entity.Property(e => e.PromoCodeId).HasColumnType("int(11)");

                entity.Property(e => e.StaffId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.BillableEntity)
                    .WithMany(p => p.Receiptheaders)
                    .HasForeignKey(d => d.BillableEntityId)
                    .HasConstraintName("fk_billableEntityId");

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

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.PaymentTransactionId).HasColumnType("int(11)");

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

            modelBuilder.Entity<Registrationfeetype>(entity =>
            {
                entity.ToTable("registrationfeetype");

                entity.Property(e => e.RegistrationFeeTypeId).HasColumnType("int(11)");

                entity.Property(e => e.RegistrationFeeTypeName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Registrationgroup>(entity =>
            {
                entity.ToTable("registrationgroup");

                entity.Property(e => e.RegistrationGroupId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Registrationgroupmembershiplink>(entity =>
            {
                entity.ToTable("registrationgroupmembershiplink");

                entity.HasIndex(e => e.MembershipId, "fk_reggroupmem_membership_id_idx");

                entity.HasIndex(e => e.RegistrationGroupId, "fk_reggroupmem_registration_id_idx");

                entity.Property(e => e.RegistrationGroupMembershipLinkId).HasColumnType("int(11)");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.RegistrationGroupId).HasColumnType("int(11)");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Registrationgroupmembershiplinks)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_reggroupmem_membership_id");

                entity.HasOne(d => d.RegistrationGroup)
                    .WithMany(p => p.Registrationgroupmembershiplinks)
                    .HasForeignKey(d => d.RegistrationGroupId)
                    .HasConstraintName("fk_reggroupmem_registration_id");
            });

            modelBuilder.Entity<Relation>(entity =>
            {
                entity.ToTable("relation");

                entity.HasIndex(e => e.EntityId, "fk_relation_entity_id_idx");

                entity.HasIndex(e => e.RelatedEntityId, "fk_relation_related_entity_id_idx");

                entity.HasIndex(e => e.RelationshipId, "fk_relation_relationshipId_idx");

                entity.Property(e => e.RelationId).HasColumnType("int(11)");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.Notes)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RelatedEntityId).HasColumnType("int(11)");

                entity.Property(e => e.RelationshipId).HasColumnType("int(11)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.RelationEntities)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_relation_entity_id");

                entity.HasOne(d => d.RelatedEntity)
                    .WithMany(p => p.RelationRelatedEntities)
                    .HasForeignKey(d => d.RelatedEntityId)
                    .HasConstraintName("fk_relation_related_entity_id");

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

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.isCommunity).HasColumnType("int(11");

                entity.Property(e => e.isFavorite).HasColumnType("int(11");

                entity.Property(e => e.ReportType).HasColumnType("varchar(100)");

                entity.Property(e => e.PreviewMode).HasColumnType("int(11)");

                entity.Property(e => e.LastUpdatedOn).HasColumnType("datetime");

                entity.Property(e => e.Fields)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

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

                //entity.HasIndex(e => e.ReportId, "fk_reportargument_reportId_idx");

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

                //entity.HasOne(d => d.Report)
                //    .WithMany(p => p.Reportarguments)
                //    .HasForeignKey(d => d.ReportId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("fk_reportargument_reportId");
            });

            modelBuilder.Entity<Reportcategory>(entity =>
            {
                entity.ToTable("reportcategory");

                entity.Property(e => e.ReportCategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Reportfield>(entity =>
            {
                entity.ToTable("reportfield");

                entity.HasIndex(e => e.ReportCategoryId, "fk_reportfield_reportcategory_id_idx");

                entity.HasIndex(e => e.CustomFieldId, "fk_reportfield_customfield_id_idx");

                entity.Property(e => e.ReportFieldId).HasColumnType("int(11)");

                entity.Property(e => e.DataType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.FieldName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.FieldTitle)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReportCategoryId).HasColumnType("int(11)");

                entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

                entity.Property(e => e.CustomFieldId).HasColumnType("int(11)");

                entity.Property(e => e.DisplayType)
                      .HasColumnType("varchar(45)")
                      .HasCharSet("latin1")
                      .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TableName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Label)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.ReportCategory)
                    .WithMany(p => p.Reportfields)
                    .HasForeignKey(d => d.ReportCategoryId)
                    .HasConstraintName("fk_reportfield_reportcategory_id");

                entity.HasOne(d => d.CustomField)
                    .WithMany(p => p.Reportfields)
                    .HasForeignKey(d => d.CustomFieldId)
                    .HasConstraintName("fk_reportfield_customfield_id");
            });

            modelBuilder.Entity<Reportfilter>(entity =>
            {
                entity.ToTable("reportfilter");

                entity.HasIndex(e => e.ReportId, "fk_reportfilter_report_id_idx");

                entity.HasIndex(e => e.ReportFieldId, "fk_reportfilter_fieldId_idx");

                entity.Property(e => e.ReportFilterId).HasColumnType("int(11)");

                entity.Property(e => e.Operator)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.Property(e => e.ReportFieldId).HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Reportfilters)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_reportfilter_report_id");

                entity.HasOne(d => d.Field)
                   .WithMany(p => p.Reportfilters)
                   .HasForeignKey(d => d.ReportFieldId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("fk_reportfilter_fieldId");

            });

            modelBuilder.Entity<Reportparameter>(entity =>
            {
                entity.ToTable("reportparameter");

                entity.HasIndex(e => e.CategoryId, "fk_reportparameter_categoryId_idx");

                entity.Property(e => e.ReportParameterId).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Parameter)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Type)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Reportparameters)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("fk_reportparameter_categoryId");
            });

            modelBuilder.Entity<Reportshare>(entity =>
            {
                entity.ToTable("reportshare");

                entity.HasIndex(e => e.ReportId, "fk_report_share_report_id_idx");

                entity.Property(e => e.ReportShareId).HasColumnType("int(11)");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.Property(e => e.SharedToUserId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Reportshares)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("fk_report_share_report_id");
            });

            modelBuilder.Entity<Reportsortorder>(entity =>
            {
                entity.ToTable("reportsortorder");

                entity.HasIndex(e => e.ReportId, "fk_reportsortorder_report_id_idx");

                entity.Property(e => e.ReportSortOrderId).HasColumnType("int(11)");

                entity.Property(e => e.FieldName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.FieldPathName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Order)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Reportsortorders)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("fk_reportsortorder_report_id");
            });

            modelBuilder.Entity<Resetpassword>(entity =>
            {
                entity.ToTable("resetpassword");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.IpAddress)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Token)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserId).HasColumnType("int(11)");
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

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("session");

                entity.HasIndex(e => e.EventId, "fk_event_id_idx");

                entity.HasIndex(e => e.GlAccountId, "fk_glaccount_id_idx");

                entity.Property(e => e.SessionId).HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .HasColumnType("varchar(20)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EnableCeu)
                    .HasColumnType("int(11)")
                    .HasColumnName("EnableCEU");

                entity.Property(e => e.EnableMaxCapacity).HasColumnType("int(11)");

                entity.Property(e => e.EnableSamePrice).HasColumnType("int(11)");

                entity.Property(e => e.EnableTax).HasColumnType("int(11)");

                entity.Property(e => e.EndDateTime).HasColumnType("datetime");

                entity.Property(e => e.EventId).HasColumnType("int(11)");

                entity.Property(e => e.GlAccountId)
                    .HasColumnType("int(11)")
                    .HasColumnName("GlAccountID");

                entity.Property(e => e.Location)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.MaxCapacity).HasColumnType("int(11)");

                entity.Property(e => e.MemberPrice).HasPrecision(13, 2);

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.NonMemberPrice).HasPrecision(13, 2);

                entity.Property(e => e.StartDatetime).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Tax).HasPrecision(13, 2);

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("fk_event_id");

                entity.HasOne(d => d.GlAccount)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.GlAccountId)
                    .HasConstraintName("fk_glaccount_id");
            });

            modelBuilder.Entity<Sessionleaderlink>(entity =>
            {
                entity.ToTable("sessionleaderlink");

                entity.HasIndex(e => e.EntityId, "fk_entity_id_idx");

                entity.HasIndex(e => e.SessionId, "fk_session_id_idx");

                entity.Property(e => e.SessionLeaderLinkId).HasColumnType("int(11)");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.SessionId).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Sessionleaderlinks)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_entity_id");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Sessionleaderlinks)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("fk_session_id");
            });

            modelBuilder.Entity<Sessionregistrationgrouppricing>(entity =>
            {
                entity.ToTable("sessionregistrationgrouppricing");

                entity.HasIndex(e => e.RegistrationGroupId, "fk_registrationgroup_id_idx");

                entity.HasIndex(e => e.SessionId, "fk_session_id_idx");

                entity.HasIndex(e => e.RegistrationFeeTypeId, "fk_srgp_registrationfeetype_id_idx");

                entity.Property(e => e.SessionRegistrationGroupPricingId).HasColumnType("int(11)");

                entity.Property(e => e.Price).HasPrecision(13, 2);

                entity.Property(e => e.RegistrationFeeTypeId).HasColumnType("int(11)");

                entity.Property(e => e.RegistrationGroupId).HasColumnType("int(11)");

                entity.Property(e => e.SessionId).HasColumnType("int(11)");

                entity.HasOne(d => d.RegistrationFeeType)
                    .WithMany(p => p.Sessionregistrationgrouppricings)
                    .HasForeignKey(d => d.RegistrationFeeTypeId)
                    .HasConstraintName("fk_srgp_registrationfeetype_id");

                entity.HasOne(d => d.RegistrationGroup)
                    .WithMany(p => p.Sessionregistrationgrouppricings)
                    .HasForeignKey(d => d.RegistrationGroupId)
                    .HasConstraintName("fk_registrationgroup_id");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Sessionregistrationgrouppricings)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("fk_registrationgroup_session_id");
            });

            modelBuilder.Entity<Shoppingcart>(entity =>
            {
                entity.ToTable("shoppingcart");

                entity.HasIndex(e => e.ReceiptId, "fk_shoppingcart_receiptId_idx");

                entity.HasIndex(e => e.UserId, "fk_shoppingcart_staff_user_id_idx");

                entity.Property(e => e.ShoppingCartId).HasColumnType("int(11)");

                entity.Property(e => e.CreditBalance).HasPrecision(14, 3);

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.MemberPortalUser)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("memberPortalUser")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PaymentMode)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PaymentStatus).HasColumnType("int(11)");

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

            modelBuilder.Entity<Solrexport>(entity =>
            {
                entity.HasKey(e => e.SolrId)
                    .HasName("PRIMARY");

                entity.ToTable("solrexport");

                entity.Property(e => e.SolrId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FileName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Text)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Uploaded)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
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

                entity.Property(e => e.ProfilePictureId).HasColumnType("int(11)");

                entity.Property(e => e.Salt)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SociableProfileId).HasColumnType("int(11)");

                entity.Property(e => e.SociableUserId).HasColumnType("int(11)");

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

            modelBuilder.Entity<Staffusersearchhistory>(entity =>
            {
                entity.ToTable("staffusersearchhistory");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.SearchText)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.StaffUserId).HasColumnType("int(11)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("states");

                entity.Property(e => e.StateId).HasColumnType("int(11)");

                entity.Property(e => e.CountryId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Tabinfo>(entity =>
            {
                entity.HasKey(e => e.TabId)
                    .HasName("PRIMARY");

                entity.ToTable("tabinfo");

                entity.HasIndex(e => e.ModuleId, "ModuleId");

                entity.Property(e => e.TabId).HasColumnType("int(11)");

                entity.Property(e => e.ModuleId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.Tabinfos)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("tabinfo_ibfk_1");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag");

                entity.Property(e => e.TagId).HasColumnType("int(11)");

                entity.Property(e => e.TagName)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<Timezone>(entity =>
            {
                entity.ToTable("timezone");

                entity.Property(e => e.TimeZoneId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.TimeZoneAbbreviation)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TimeZoneName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TimeZoneOffset).HasPrecision(5, 2);
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

            modelBuilder.Entity<TrApplock>(entity =>
            {
                entity.ToTable("tr_applock");

                entity.Property(e => e.Id)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<TrCategory>(entity =>
            {
                entity.ToTable("tr_category");

                entity.HasIndex(e => e.Name, "IX_tr_category_Name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<TrDefinition>(entity =>
            {
                entity.ToTable("tr_definition");

                entity.HasIndex(e => e.ReportId, "ReportId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasColumnName("definition")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.TrDefinitions)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("tr_definition_ibfk_1");
            });

            modelBuilder.Entity<TrObject>(entity =>
            {
                entity.ToTable("tr_object");

                entity.Property(e => e.Id)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

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

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

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
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("tr_set");

                entity.Property(e => e.Id)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Member)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<TrString>(entity =>
            {
                entity.ToTable("tr_string");

                entity.Property(e => e.Id)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<TrTemplate>(entity =>
            {
                entity.ToTable("tr_template");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasColumnName("definition")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Userdevice>(entity =>
            {
                entity.ToTable("userdevice");

                entity.HasIndex(e => e.EntityId, "fk_userdevice_entity_id_idx");

                entity.HasIndex(e => e.UserId, "fk_userdevice_staffuser_id_idx");

                entity.Property(e => e.UserDeviceId).HasColumnType("int(11)");

                entity.Property(e => e.Authenticated).HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DeviceName)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.Ipaddress)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("IPAddress")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.LastAccessed).HasColumnType("datetime");

                entity.Property(e => e.LastValidated).HasColumnType("datetime");

                entity.Property(e => e.Locked).HasColumnType("int(11)");

                entity.Property(e => e.RemberDevice).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.Userdevices)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("fk_userdevice_entity_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userdevices)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_userdevice_staffuser_id");
            });

            modelBuilder.Entity<Voiddetail>(entity =>
            {
                entity.HasKey(e => e.VoidId)
                    .HasName("PRIMARY");

                entity.ToTable("voiddetail");

                entity.HasIndex(e => e.ReceiptId, "fk_void_receiptid_id_idx");

                entity.HasIndex(e => e.UserId, "fk_void_usr_id_idx");

                entity.Property(e => e.VoidId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(14, 3);

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EntityId).HasColumnType("int(11)");

                entity.Property(e => e.PaymentTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.Reason)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReceiptId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.VoidDate).HasColumnType("datetime");

                entity.Property(e => e.VoidMode)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.Voiddetails)
                    .HasForeignKey(d => d.ReceiptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_void_receiptid_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Voiddetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_void_usr_id");
            });

            modelBuilder.Entity<Writeoff>(entity =>
            {
                entity.ToTable("writeoff");

                entity.HasIndex(e => e.InvoiceDetailId, "fk_writeoff_invoicedetailid_idx");

                entity.HasIndex(e => e.UserId, "fk_writeoff_userid_idx");

                entity.Property(e => e.WriteOffId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(13, 2);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.InvoiceDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Reason)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.InvoiceDetail)
                    .WithMany(p => p.Writeoffs)
                    .HasForeignKey(d => d.InvoiceDetailId)
                    .HasConstraintName("fk_writeoff_invoicedetailid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Writeoffs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_writeoff_userid");
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

            modelBuilder.Entity<Customfieldblock>(entity =>
            {
                entity.HasKey(e => e.BlockId)
                    .HasName("PRIMARY");

                entity.ToTable("customfieldblock");

                entity.HasIndex(e => e.ModuleId, "ModuleId");

                entity.HasIndex(e => e.TabId, "TabId");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.Customfieldblocks)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("customfieldblock_ibfk_1");

                entity.HasOne(d => d.Tab)
                    .WithMany(p => p.Customfieldblocks)
                    .HasForeignKey(d => d.TabId)
                    .HasConstraintName("customfieldblock_ibfk_2");
            });

            //OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}