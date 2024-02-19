using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class membermaxContext : DbContext
    {
        public membermaxContext()
        {
        }

        public membermaxContext(DbContextOptions<membermaxContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Communication> Communications { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<Glaccount> Glaccounts { get; set; }
        public virtual DbSet<Glaccounttype> Glaccounttypes { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Invoicedetail> Invoicedetails { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<Membershipcategory> Membershipcategories { get; set; }
        public virtual DbSet<Membershipfee> Membershipfees { get; set; }
        public virtual DbSet<Membershiphistory> Membershiphistories { get; set; }
        public virtual DbSet<Membershipperiod> Membershipperiods { get; set; }
        public virtual DbSet<Membershiptype> Membershiptypes { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Persontag> Persontags { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }
        public virtual DbSet<Relation> Relations { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Rolemenu> Rolemenus { get; set; }
        public virtual DbSet<Staffrole> Staffroles { get; set; }
        public virtual DbSet<Staffuser> Staffusers { get; set; }
        public virtual DbSet<YuniqlSchemaVersion> YuniqlSchemaVersions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;database=membermax;uid=root;password=Anit1066", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.33-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.HasIndex(e => e.PersonId, "fk_address_person_id_idx");

                entity.Property(e => e.AddressId).HasColumnType("int(11)");

                entity.Property(e => e.Address1).HasMaxLength(100);

                entity.Property(e => e.Address2).HasMaxLength(100);

                entity.Property(e => e.Address3).HasMaxLength(100);

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.Country).HasMaxLength(45);

                entity.Property(e => e.Label).HasMaxLength(45);

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.State).HasMaxLength(45);

                entity.Property(e => e.Zip).HasMaxLength(10);

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_address_person_id");
            });

            modelBuilder.Entity<Communication>(entity =>
            {
                entity.ToTable("communication");

                entity.HasIndex(e => e.PersonId, "fk_communication_person_id_idx");

                entity.HasIndex(e => e.StaffUserId, "fk_communication_staffUser_id_idx");

                entity.Property(e => e.CommunicationId).HasColumnType("int(11)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.From).HasMaxLength(45);

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Scheduled).HasColumnType("int(11)");

                entity.Property(e => e.StaffUserId).HasColumnType("int(11)");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Subject).HasMaxLength(100);

                entity.Property(e => e.Type).HasMaxLength(45);

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

                entity.Property(e => e.AddressId).HasColumnType("int(11)");

                entity.Property(e => e.CompanyName).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.PrimaryConatctPhone).HasMaxLength(45);

                entity.Property(e => e.PrimaryContactEmail).HasMaxLength(255);

                entity.Property(e => e.PrimaryContactName).HasMaxLength(100);

                entity.Property(e => e.Website).HasMaxLength(255);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.HasIndex(e => e.OrganizationId, "fk_department_organization_id_idx");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.CostCenterCode).HasMaxLength(45);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(45);

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

                entity.HasIndex(e => e.PersonId, "fk_document_person_id_idx");

                entity.Property(e => e.DocumentId).HasColumnType("int(11)");

                entity.Property(e => e.ContentType).HasMaxLength(100);

                entity.Property(e => e.DisplayFileName).HasMaxLength(100);

                entity.Property(e => e.FileName).HasMaxLength(500);

                entity.Property(e => e.FilePath).HasMaxLength(255);

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Title).HasMaxLength(100);

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

                entity.Property(e => e.EmailAddress).HasMaxLength(255);

                entity.Property(e => e.Label).HasMaxLength(45);

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

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CostCenterId).HasColumnType("int(11)");

                entity.Property(e => e.DetailType).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.Glaccounts)
                    .HasForeignKey(d => d.AccountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_glaccount_accounttype_id");

                entity.HasOne(d => d.CostCenter)
                    .WithMany(p => p.Glaccounts)
                    .HasForeignKey(d => d.CostCenterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_glaccount_depart_cc_id");
            });

            modelBuilder.Entity<Glaccounttype>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PRIMARY");

                entity.ToTable("glaccounttype");

                entity.Property(e => e.AccountId).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoice");

                entity.HasIndex(e => e.MembershipId, "fk_invoice_membership_id_idx");

                entity.HasIndex(e => e.BillablePersonId, "fk_invoice_person_id_idx");

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.BankDraftTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.BillablePersonId).HasColumnType("int(11)");

                entity.Property(e => e.BillingType).HasMaxLength(45);

                entity.Property(e => e.CreditCardTransactionId).HasColumnType("int(11)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("date");

                entity.Property(e => e.InvoiceType).HasMaxLength(45);

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.BillablePerson)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.BillablePersonId)
                    .HasConstraintName("fk_invoice_person_id");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.MembershipId)
                    .HasConstraintName("fk_invoice_membership_id");
            });

            modelBuilder.Entity<Invoicedetail>(entity =>
            {
                entity.ToTable("invoicedetail");

                entity.HasIndex(e => e.InvoiceId, "fk_invoice_detail_invoicce_id_idx");

                entity.Property(e => e.InvoiceDetailId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasPrecision(13, 2);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.GlAccount).HasMaxLength(45);

                entity.Property(e => e.InvoiceId).HasColumnType("int(11)");

                entity.Property(e => e.Price).HasPrecision(13, 2);

                entity.Property(e => e.Quantity).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TaxAmount).HasPrecision(13, 2);

                entity.Property(e => e.TaxRate).HasPrecision(13, 2);

                entity.Property(e => e.Taxable).HasColumnType("int(11)");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Invoicedetails)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("fk_invoice_detail_invoicce_id");
            });

            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.ToTable("lookup");

                entity.Property(e => e.LookupId).HasColumnType("int(11)");

                entity.Property(e => e.Group).HasMaxLength(45);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Values).HasMaxLength(5000);
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.ToTable("membership");

                entity.HasIndex(e => e.MembershipTypeId, "fk_membbership_membbership_type_id_idx");

                entity.HasIndex(e => e.BillablePersonId, "fk_mmembbership_billabble_personId_idx");

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.BillablePersonId).HasColumnType("int(11)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.MaxDraftAmount).HasPrecision(13, 2);

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.NextBillDate).HasColumnType("date");

                entity.Property(e => e.RenewalDate).HasColumnType("date");

                entity.Property(e => e.ReviewDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TerminationDate).HasColumnType("date");

                entity.HasOne(d => d.BillablePerson)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.BillablePersonId)
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

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Membershipfee>(entity =>
            {
                entity.HasKey(e => e.FeeId)
                    .HasName("PRIMARY");

                entity.ToTable("membershipfee");

                entity.HasIndex(e => e.MembershipTypeId, "fk_MembershipTypeId_idx");

                entity.HasIndex(e => e.GlAccount, "fk_mmemmbbershipfee_glchartof_account_id_idx");

                entity.Property(e => e.FeeId).HasColumnType("int(11)");

                entity.Property(e => e.BillingFrequency).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.FeeAmount).HasPrecision(13, 2);

                entity.Property(e => e.GlAccount).HasColumnType("int(11)");

                entity.Property(e => e.IsMandatory).HasColumnType("int(11)");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasMaxLength(45);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.GlAccountNavigation)
                    .WithMany(p => p.Membershipfees)
                    .HasForeignKey(d => d.GlAccount)
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

                entity.Property(e => e.Reason).HasMaxLength(250);

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

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(45);

                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Membershiptype>(entity =>
            {
                entity.ToTable("membershiptype");

                entity.HasIndex(e => e.Category, "fk_membershiptype_memmbeeership_category_id_idx");

                entity.HasIndex(e => e.Period, "fk_mmembershiptype_period_id_idx");

                entity.Property(e => e.MembershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Category).HasColumnType("int(11)");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Period).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

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

                entity.Property(e => e.MenuId).HasColumnType("int(11)");

                entity.Property(e => e.Group).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Url).HasMaxLength(255);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("organization");

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Address1).HasMaxLength(100);

                entity.Property(e => e.Address2).HasMaxLength(100);

                entity.Property(e => e.Address3).HasMaxLength(100);

                entity.Property(e => e.City).HasMaxLength(45);

                entity.Property(e => e.Country).HasMaxLength(45);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Createdy).HasMaxLength(45);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FooterImge).HasMaxLength(45);

                entity.Property(e => e.HeaderImage).HasColumnType("blob");

                entity.Property(e => e.Logo).HasColumnType("blob");

                entity.Property(e => e.ModifiedBy).HasMaxLength(45);

                entity.Property(e => e.ModifiedOn).HasMaxLength(45);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Prefix).HasMaxLength(45);

                entity.Property(e => e.PrimaryContacName).HasMaxLength(100);

                entity.Property(e => e.PrimaryContactEmail).HasMaxLength(255);

                entity.Property(e => e.PrimaryContactPhone).HasMaxLength(45);

                entity.Property(e => e.State).HasMaxLength(10);

                entity.Property(e => e.Website).HasMaxLength(255);

                entity.Property(e => e.Zip).HasMaxLength(10);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("person");

                entity.HasIndex(e => e.CompanyId, "fk_person_comapny_id_idx");

                entity.HasIndex(e => e.OrganizationId, "fk_person_organization_id_idx");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.FacebookName).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.Gender).HasMaxLength(45);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.LinkedinName).HasMaxLength(100);

                entity.Property(e => e.MembershipId).HasColumnType("int(11)");

                entity.Property(e => e.MiddleName).HasMaxLength(45);

                entity.Property(e => e.OrganizationId).HasColumnType("int(11)");

                entity.Property(e => e.Prefix).HasMaxLength(45);

                entity.Property(e => e.ProfilePictureId).HasColumnType("int(11)");

                entity.Property(e => e.Salutation).HasMaxLength(45);

                entity.Property(e => e.SkypeName).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Suffix).HasMaxLength(45);

                entity.Property(e => e.Title).HasMaxLength(45);

                entity.Property(e => e.TwitterName).HasMaxLength(100);

                entity.Property(e => e.Website).HasMaxLength(255);

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

                entity.Property(e => e.Label).HasMaxLength(50);

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.Value).HasMaxLength(255);

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

                entity.Property(e => e.Label).HasMaxLength(45);

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PhoneNumber).HasMaxLength(45);

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_phone_person_id");
            });

            modelBuilder.Entity<Relation>(entity =>
            {
                entity.ToTable("relation");

                entity.HasIndex(e => e.PersonId, "fk_relation_person_id_idx");

                entity.HasIndex(e => e.RelatedPersonId, "fk_relation_related_person_id_idx");

                entity.Property(e => e.RelationId).HasColumnType("int(11)");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.RelatedPersonId).HasColumnType("int(11)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.Type).HasMaxLength(45);

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.RelationPeople)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_relation_person_id");

                entity.HasOne(d => d.RelatedPerson)
                    .WithMany(p => p.RelationRelatedPeople)
                    .HasForeignKey(d => d.RelatedPersonId)
                    .HasConstraintName("fk_relation_related_person_id");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(50);

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

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.Department).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastAccessed).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(500);

                entity.Property(e => e.Salt).HasMaxLength(500);

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.UserName).HasMaxLength(50);
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
                    .HasMaxLength(4000)
                    .HasColumnName("additional_artifacts");

                entity.Property(e => e.AppliedByTool)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("applied_by_tool");

                entity.Property(e => e.AppliedByToolVersion)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnName("applied_by_tool_version");

                entity.Property(e => e.AppliedByUser)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("applied_by_user");

                entity.Property(e => e.AppliedOnUtc)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("applied_on_utc")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Checksum)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("checksum");

                entity.Property(e => e.DurationMs)
                    .HasColumnType("int(11)")
                    .HasColumnName("duration_ms");

                entity.Property(e => e.FailedScriptError)
                    .HasMaxLength(4000)
                    .HasColumnName("failed_script_error");

                entity.Property(e => e.FailedScriptPath)
                    .HasMaxLength(4000)
                    .HasColumnName("failed_script_path");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("status");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(190)
                    .HasColumnName("version");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
