using System;
using System.Collections.Generic;
using System.Text;
using Faker;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class TestDataGenerator
    {
        //public static Staffusersearchhistories GetStaffUserSearchHistoryModel()
        //{
        //    staffusersearchhistory model = new staffusersearchhistory();
        //    model.StaffUserId = Faker.RandomNumber.Next();
        //    model.SearchText = Faker.Name.First();
        //    model.Id = Faker.RandomNumber.Next();
        //    return model;
        //}
        public static RegistrationGroupModel GetGroupRegistrationModel()
        {
            var model = new RegistrationGroupModel();
            model.Name = Faker.Name.First();
            model.Type = Faker.Name.First();
            model.Status = 1;
            model.MembershipTypeIds = new List<int>();
            return model;
        }
         public static Registrationgroupmembershiplink GetLinkMembershipModel()
        {
            var model = new Registrationgroupmembershiplink();
            model.MembershipId = Faker.RandomNumber.Next();
            return model;
        }
        public static StaffUserModel GetStaffUserModel()
        {
            StaffUserModel staffUser = new StaffUserModel();
            staffUser.UserId = Faker.RandomNumber.Next(100);
            staffUser.FirstName = Faker.Name.First();
            staffUser.LastName = Faker.Name.Last();
            staffUser.UserName = Faker.Internet.UserName();
            staffUser.Email = Faker.Internet.Email();
            staffUser.DepartmentName = Faker.Company.Name();
            staffUser.DepartmentId = Faker.RandomNumber.Next();
            staffUser.OrganizationId = Faker.RandomNumber.Next();
            staffUser.CreatedBy = Faker.Internet.UserName();
            staffUser.CreatedOn = DateTime.Now;
            staffUser.Password = Faker.Lorem.GetFirstWord();
            staffUser.LastAccessed = DateTime.Now;
            staffUser.ModifiedBy = Faker.Internet.UserName();
            staffUser.ModifiedOn = DateTime.Now;
            staffUser.Status = 1;

            //Add Department
            var department = GetDepartmentModel();
            staffUser.Department = department;

            return staffUser;

        }

        public static StaffUserModel GetStaffUserValidationModel(string username, string useremail)
        {
            StaffUserModel staffUser = new StaffUserModel();
            staffUser.UserId = Faker.RandomNumber.Next(100);
            staffUser.FirstName = Faker.Name.First();
            staffUser.LastName = Faker.Name.Last();
            staffUser.UserName = String.IsNullOrWhiteSpace(username) ? username : Faker.Company.Name();
            staffUser.Email = String.IsNullOrWhiteSpace(useremail) ? useremail : Faker.Internet.Email();
            staffUser.DepartmentName = Faker.Company.Name();
            staffUser.DepartmentId = Faker.RandomNumber.Next();
            staffUser.OrganizationId = Faker.RandomNumber.Next();
            staffUser.CreatedBy = Faker.Internet.UserName();
            staffUser.CreatedOn = DateTime.Now;
            staffUser.Password = Faker.Lorem.GetFirstWord();
            staffUser.LastAccessed = DateTime.Now;
            staffUser.ModifiedBy = Faker.Internet.UserName();
            staffUser.ModifiedOn = DateTime.Now;
            staffUser.Status = 1;

            //Add Department
            var department = GetDepartmentModel();
            staffUser.Department = department;

            return staffUser;
        }

        public static RoleModel GetRoleModel()
        {
            RoleModel role = new RoleModel();

            role.Name = Faker.Name.First();
            role.Description = Faker.Lorem.Sentence();
            role.Status = 1;

            return role;

        }

        public static MembershipTypeModel GetMembershipTypeModel()
        {
            MembershipTypeModel model = new MembershipTypeModel();

            model.MembershipTypeId = Faker.RandomNumber.Next();
            model.Name = Faker.Company.Name();
            model.Code = Faker.Identification.SocialSecurityNumber();
            //model.Period = Faker.RandomNumber.Next();
            //model.Category = Faker.RandomNumber.Next();
            model.Description = Faker.Lorem.Paragraph();
            //model.PeriodName = Faker.Lorem.Sentence();
            //model.CategoryName = Faker.Lorem.Sentence();
            model.TotalFee = Faker.RandomNumber.Next();
            model.MembershipFee = Faker.RandomNumber.Next();
            model.Status = 1;
            model.FeeDetailTag = Faker.Lorem.Sentence();

            //Add MembershipFee
            var membershifees = GetMembershipFeeModel();
            model.MembershipFees.Add(membershifees);



            model.Period = 1;
            model.Category = 1;

            return model;

        }


        public static MembershipTypeModel GetMembershipTypeValidationModel(string typename, string typecode)
        {
            MembershipTypeModel model = new MembershipTypeModel();

            model.MembershipTypeId = Faker.RandomNumber.Next();
            model.Name = String.IsNullOrWhiteSpace(typename) ? typename : Faker.Company.Name();
            model.Code = String.IsNullOrWhiteSpace(typecode) ? typecode : Faker.Identification.SocialSecurityNumber();
            model.Period = Faker.RandomNumber.Next();
            model.Category = Faker.RandomNumber.Next();
            model.Description = Faker.Lorem.Paragraph();
            model.PeriodName = Faker.Lorem.Sentence();
            model.CategoryName = Faker.Lorem.Sentence();
            model.TotalFee = Faker.RandomNumber.Next();
            model.MembershipFee = Faker.RandomNumber.Next();
            model.Status = 1;
            model.FeeDetailTag = Faker.Lorem.Sentence();

            //Add MembershipFee
            var membershifees = GetMembershipFeeModel();
            model.MembershipFees.Add(membershifees);


            return model;
        }

        public static MembershipFeeModel GetMembershipFeeModel()
        {
            MembershipFeeModel model = new MembershipFeeModel();

            model.Name = Faker.Company.Name();
            model.FeeAmount = Faker.RandomNumber.Next();
            model.GlAccountId = Faker.RandomNumber.Next();
            model.IsMandatory = 1;
            model.BillingFrequency = Faker.RandomNumber.Next();
            model.MembershipTypeId = 1;
            model.Description = Faker.Lorem.Paragraph();
            model.Status = 1;

            return model;

        }

        public static MembershipPeriodModel GetMembershipPeriodModel()
        {
            MembershipPeriodModel model = new MembershipPeriodModel();

            model.Name = Faker.Company.Name();
            model.Description = Faker.Lorem.Paragraph();
            model.PeriodUnit = "Day";
            model.Duration = 7;
            model.Status = 1;

            return model;

        }



        public static MembershipCategoryModel GetMembershipCategoryModel()
        {
            MembershipCategoryModel model = new MembershipCategoryModel();

            model.Name = Faker.Company.Name();
            model.Status = 1;

            return model;

        }

        public static DepartmentModel GetDepartmentModel()
        {
            DepartmentModel model = new DepartmentModel();

            model.Name = Faker.Company.Name();
            model.Description = Faker.Lorem.Paragraph();
            model.CostCenterCode = Faker.Name.Middle();
            model.Status = 1;

            return model;

        }

        public static DepartmentModel GetDepartmentModel_WithnullName()
        {
            DepartmentModel model = new DepartmentModel();

            model.Name = null;
            model.Description = Faker.Lorem.Paragraph();
            model.CostCenterCode = Faker.Name.Middle();
            model.Status = 1;

            return model;

        }

        public static DepartmentModel GetDepartmentModel_WithBlankName()
        {
            DepartmentModel model = new DepartmentModel();

            model.Name = "";
            model.Description = Faker.Lorem.Paragraph();
            model.CostCenterCode = Faker.Name.Middle();
            model.Status = 1;

            return model;

        }

        public static GlAccountTypeModel GetGlAccountTypeModel()
        {
            GlAccountTypeModel model = new GlAccountTypeModel();

            model.Name = Faker.Company.Name();
            model.Description = Faker.Lorem.Paragraph();
            model.Status = 1;

            return model;

        }
        public static GlAccountModel GetGlAccountModel()
        {
            GlAccountModel model = new GlAccountModel();

            model.Name = Faker.Company.Name();
            model.DetailType = Faker.Lorem.Sentence();
            //model.CostCenter = Faker.RandomNumber.Next();
            model.Code = Faker.Finance.Isin();
            model.Status = 1;


            //Add Department and accounttype

            model.CostCenter = 1; //test costcenter 1 is generated by department model in test GLAccountTests.cs            
            model.Type = 1; //test accounttype 1 is generated by department model in test GLAccountTests.cs

            return model;

        }


        public static GlAccountModel GetGlAccountModel_withBlankName()
        {
            GlAccountModel model = new GlAccountModel();

            model.Name = "";
            model.DetailType = Faker.Lorem.Sentence();
            //model.CostCenter = Faker.RandomNumber.Next();
            model.Code = Faker.Finance.Isin();
            model.Status = 1;

            return model;
        }

        public static GlAccountModel GetGlAccountModel_withduplicateNameorCode(string glname, string glcode)
        {
            GlAccountModel model = new GlAccountModel();
            if (glname == "")
            {
                model.Name = Faker.Company.Name();
                model.DetailType = Faker.Lorem.Sentence();
                model.Code = glcode;
                model.Status = 1;
            }
            else
            {
                model.Name = glname;
                model.DetailType = Faker.Lorem.Sentence();
                model.Code = Faker.Finance.Isin();
                model.Status = 1;

            }
            return model;
        }

        public static PersonModel GetPersonModel()
        {
            PersonModel model = new PersonModel();

            model.PersonId = 1;
            model.Prefix = Faker.Name.Prefix();
            model.FirstName = Faker.Name.First();
            model.LastName = Faker.Name.Last();
            model.MiddleName = Faker.Name.Middle();
            model.Suffix = Faker.Name.Suffix();
            model.Department = "1";
            model.Gender = "Male";
            model.DateOfBirth = DateTime.Now;
            model.CompanyId = Faker.RandomNumber.Next();
            model.FacebookName = Faker.Internet.DomainName();
            model.LinkedinName = Faker.Internet.DomainName();
            model.SkypeName = Faker.Internet.DomainName();
            model.Salutation = "Mr.";
            model.Status = 1;
            model.EntityId = Faker.RandomNumber.Next(1, 100);
            //model.MembershipId = Faker.RandomNumber.Next(0, 100);

            //Add Email Address
            EmailModel email = new EmailModel();
            email.EmailAddress = Faker.Internet.Email();
            email.EmailAddressType = "Work";
            model.Emails.Add(email);

            //Add Another
            email = new EmailModel();
            email.EmailAddress = Faker.Internet.Email();
            email.EmailAddressType = "Personal";
            model.Emails.Add(email);

            //Add Phone
            PhoneModel phone = new PhoneModel();
            phone.PhoneType = "Home";
            phone.PhoneNumber = Faker.Phone.Number();
            model.Phones.Add(phone);

            //Add another  Phone
            phone = new PhoneModel();
            phone.PhoneType = "Work";
            phone.PhoneNumber = Faker.Phone.Number();
            model.Phones.Add(phone);

            //Add company

            var company = GetCompanyModel();
            model.Company = company;

            return model;

        }

        public static CompanyModel GetCompanyModel()
        {
            CompanyModel model = new CompanyModel();

            model.CompanyId = Faker.RandomNumber.Next();
            model.CompanyName = Faker.Company.Name();
            model.Email = Faker.Internet.Email();
            model.Website = Faker.Internet.Url();
            model.Phone = Faker.Phone.Number();

            return model;

        }

        public static CommunicationModel GetCommunicationModel()
        {
            CommunicationModel model = new CommunicationModel();

            model.From = Faker.Company.Name();
            model.Notes = Faker.Lorem.Sentence();
            model.Subject = Faker.Lorem.Sentence();
            model.EntityId = Faker.RandomNumber.Next();
            model.Start = DateTime.Now;
            // model.End = DateTime.Now;
            model.Scheduled = 0;
            model.StaffUserId = Faker.RandomNumber.Next();

            return model;

        }

        public static InvoiceModel GetInvoiceModel()
        {
            InvoiceModel model = new InvoiceModel();

            model.InvoiceId = 1;
            model.Date = DateTime.Now;
            model.EntityId = Faker.RandomNumber.Next();
            model.DueDate = DateTime.Now;
            model.BillingType = "BnkDraft";
            model.InvoiceType = "Dues";
            model.MembershipId = Faker.RandomNumber.Next();
            model.BillableEntityId = 1;
            model.PaymentTransactionId = 0;
            model.Status = 1;

            //Add Membership
            var membership = GetMembershipModel();
            model.Membership = membership;

            //Add PaymentTransaction
            var paymenttransaction = GetPaymentTransactionModel();
            model.PaymentTransaction = paymenttransaction;

            //Add InvoiceDetaails
            var detail = GetInvoiceDetailModel();
            model.InvoiceDetails.Add(detail);

            return model;

        }

        public static InvoiceModel GetInvoiceValidationModel(int personId, int billablepersonId)
        {
            InvoiceModel model = new InvoiceModel();

            model.InvoiceId = 1;
            model.Date = DateTime.Now;
            //model.PersonId = Faker.RandomNumber.Next();

            model.EntityId = personId == 0 ? personId : Faker.RandomNumber.Next();

            model.DueDate = DateTime.Now;
            model.BillingType = "BnkDraft";
            model.InvoiceType = "Dues";
            model.MembershipId = Faker.RandomNumber.Next();
            //model.BillablePersonId = 1;
            model.BillableEntityId = billablepersonId == 0 ? billablepersonId : Faker.RandomNumber.Next();
            model.PaymentTransactionId = 0;
            model.Status = 1;

            //Add Membership
            var membership = GetMembershipModel();
            model.Membership = membership;

            //Add PaymentTransaction
            var paymenttransaction = GetPaymentTransactionModel();
            model.PaymentTransaction = paymenttransaction;

            //Add InvoiceDetaails
            var detail = GetInvoiceDetailModel();
            model.InvoiceDetails.Add(detail);

            return model;

        }

        public static InvoiceDetailModel GetInvoiceDetailModel()
        {
            InvoiceDetailModel model = new InvoiceDetailModel();

            model.InvoiceId = Faker.RandomNumber.Next(100);
            model.Description = Faker.Lorem.Sentence();
            model.Amount = Faker.RandomNumber.Next();
            model.Quantity = 1;
            model.Taxable = 1;
            model.GlAccount = Faker.Finance.Isin();
            model.Status = 1;

            return model;

        }

        public static MembershipModel GetMembershipModel()
        {
            MembershipModel model = new MembershipModel();

            model.MembershipTypeId = Faker.RandomNumber.Next();
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now.AddMonths(12);
            model.ReviewDate = DateTime.Now.AddMonths(12);
            //model.BillablePersonId = Faker.RandomNumber.Next();
            model.NextBillDate = DateTime.Now.AddMonths(1);
            model.BillingOnHold = Faker.RandomNumber.Next();
            model.CreateDate = DateTime.Now;
            model.RenewalDate = Constants.MySQL_MinDate;
            model.TerminationDate = Constants.MySQL_MinDate;
            model.NextBillDate = DateTime.Today;
            model.Status = 1;

            var membershiptype = GetMembershipTypeModel();
            model.MembershipType = membershiptype;

            model.BillableEntityId = 1; //

            return model;

        }

        public static MembershipSessionModel GetMembershipSessionModel()
        {
            MembershipSessionModel model = new MembershipSessionModel();
            model.BillableEntityId = 1;//Math.Abs(Faker.RandomNumber.Next());
            model.MembershipTypeId = Math.Abs(Faker.RandomNumber.Next());
            model.UserId = Faker.RandomNumber.Next();
            model.StartDate = DateTime.Today;
            model.EndDate = DateTime.Today.AddDays(1);
            model.Notes = Faker.Lorem.Sentence();

            model.MembershipFeeIds = new int[] { 1 };
            model.MembershipFees = new decimal[] { 100, 200, 300, 400 };
            return model;
        }
        public static BillingFeeModel GetBillingfeeModel()
        {
            BillingFeeModel model = new BillingFeeModel();
            model.MembershipId = 1;//Faker.RandomNumber.Next();
            model.MembershipFeeId = 1;// Faker.RandomNumber.Next();
            model.Fee = Faker.RandomNumber.Next();
            model.Status = 1;
            return model;
        }


        public static MembershipHistoryModel GetMembershipHistoryModel()
        {
            MembershipHistoryModel model = new MembershipHistoryModel();

            model.MembershipId = Faker.RandomNumber.Next();
            model.StatusDate = DateTime.Now;
            model.Status = 1;
            model.ChangedBy = Faker.RandomNumber.Next();
            model.Reason = Faker.Lorem.Sentence();
            model.PreviousMembershipId = Faker.RandomNumber.Next();

            return model;

        }

        public static MembershipHistoryModel GetMembershipHistoryUpdateModel()
        {
            MembershipHistoryModel model = new MembershipHistoryModel();

            model.MembershipId = 1;
            model.StatusDate = DateTime.Now;
            model.Status = 1;
            model.ChangedBy = Faker.RandomNumber.Next();
            model.Reason = Faker.Lorem.Sentence();
            model.PreviousMembershipId = Faker.RandomNumber.Next();

            return model;

        }


        public static RelationModel GetRelationModel()
        {
            RelationModel model = new RelationModel();

            model.EntityId = Faker.RandomNumber.Next();
            model.RelatedEntityId = Faker.RandomNumber.Next();
            model.RelationshipId = 0;
            model.StartDate = DateTime.Now.AddDays(-20);
            model.EndDate = DateTime.Now;
            model.Status = 1;
            model.Notes = Faker.Lorem.Sentence();

            return model;

        }

        public static List<Rolemenu> GetRoleMenu()
        {
            List<Max.Data.DataModel.Rolemenu> lstRoleMenu = new List<Max.Data.DataModel.Rolemenu>();
            Rolemenu rolemenu = new Rolemenu();

            rolemenu.MenuId = 1;
            rolemenu.RoleId = 1;

            lstRoleMenu.Add(rolemenu);

            return lstRoleMenu;

        }


        public static AutoBillingDraftModel GetAutoBillingDraftModel()
        {
            AutoBillingDraftModel model = new AutoBillingDraftModel();

            model.AutoBillingDraftId = Faker.RandomNumber.Next();
            model.BillingDocumentId = Faker.RandomNumber.Next();
            model.InvoiceId = Faker.RandomNumber.Next();
            model.Name = Faker.Name.FullName();
            model.EntityId = Faker.RandomNumber.Next();
            model.Amount = Faker.RandomNumber.Next();
            model.NextDueDate = DateTime.Now;
            model.IsProcessed = Faker.RandomNumber.Next();
            model.ProfileId = Faker.RandomNumber.Next().ToString();
            model.PaymentProfileId = Faker.RandomNumber.Next().ToString();

            return model;
        }

        public static OrganizationModel GetOrganizationModel()
        {
            OrganizationModel model = new OrganizationModel();

            model.OrganizationId = Faker.RandomNumber.Next();
            model.Name = Faker.Name.First();
            model.Prefix = Faker.Name.Prefix();
            model.Address1 = Faker.Address.StreetName();
            model.Address2 = Faker.Address.StreetAddress();
            model.Address3 = Faker.Address.SecondaryAddress();
            model.City = Faker.Address.City();
            model.State = Faker.Address.UsState();
            model.Zip = Faker.Address.ZipCode();
            model.Phone = Faker.Phone.Number();
            model.Country = Faker.Country.Name();
            model.Logo = GetByteArray(1).ToString() + ".jpeg";
            model.Website = Faker.Internet.Url();
            model.HeaderImage = GetByteArray(1).ToString() + ".jpeg";
            model.FooterImge = Faker.Lorem.Sentence();
            model.Email = Faker.Internet.Email();
            model.PrimaryContactName = Faker.Name.FullName();
            model.PrimaryContactEmail = Faker.Internet.Email();
            model.PrimaryContactPhone = Faker.Phone.Number();
            model.Createdy = Faker.Internet.UserName();
            model.CreatedOn = DateTime.Now;
            model.ModifiedBy = Faker.Internet.UserName();
            model.ModifiedOn = DateTime.Now.ToLongDateString();

            return model;
        }

        public static OrganizationModel GetOrganizationModel_WithAccountingSetup()
        {
            OrganizationModel model = new OrganizationModel();

            model.OrganizationId = 1;
            model.Name = Faker.Name.First();
            model.Prefix = Faker.Name.Prefix();
            model.Address1 = Faker.Address.StreetName();
            model.Address2 = Faker.Address.StreetAddress();
            model.Address3 = Faker.Address.SecondaryAddress();
            model.City = Faker.Address.City();
            model.State = Faker.Address.UsState();
            model.Zip = Faker.Address.ZipCode();
            model.Phone = Faker.Phone.Number();
            model.Country = Faker.Country.Name();
            model.Logo = GetByteArray(1).ToString() + ".jpeg";
            model.Website = Faker.Internet.Url();
            model.HeaderImage = GetByteArray(1).ToString() + ".jpeg";
            model.FooterImge = Faker.Lorem.Sentence();
            model.Email = Faker.Internet.Email();
            model.PrimaryContactName = Faker.Name.FullName();
            model.PrimaryContactEmail = Faker.Internet.Email();
            model.PrimaryContactPhone = Faker.Phone.Number();
            model.Createdy = Faker.Internet.UserName();
            model.CreatedOn = DateTime.Now;
            model.ModifiedBy = Faker.Internet.UserName();
            model.ModifiedOn = DateTime.Now.ToLongDateString();

            var accountingsetup = GetAccountingSetupModel();

            model.AccountingSetUpModel = accountingsetup;

            return model;
        }


        public static AccountingSetUpModel GetAccountingSetupModel()
        {
            AccountingSetUpModel model = new AccountingSetUpModel();

            model.OrganizationId = 1;
            model.OnlineCreditGlAccount = Faker.RandomNumber.Next();
            model.OffLinePaymentGlAccount = Faker.RandomNumber.Next();
            model.ProcessingFeeGlAccount = Faker.RandomNumber.Next();
            model.SalesReturnGlAccount = Faker.RandomNumber.Next();

            return model;
        }

        private static byte[] GetByteArray(int sizeInKb)
        {
            Random rnd = new Random();
            byte[] b = new byte[sizeInKb * 1024]; // convert kb to byte
            rnd.NextBytes(b);
            return b;
        }

        public static PaymentProfileModel GetPaymentProfileModel()
        {
            PaymentProfileModel model = new PaymentProfileModel();

            model.EntityId = 1;
            //model.ProfileId = Faker.Lorem.Sentence();
            //model.PreferredPaymentMethod = Faker.RandomNumber.Next();
            model.Status = Faker.RandomNumber.Next();
            //model.AuthNetPaymentProfileId = Faker.Lorem.Sentence();

            return model;
        }

        public static AutoBillingProcessingDateModel GetAutoBillingProcessingDateModel()
        {
            AutoBillingProcessingDateModel model = new AutoBillingProcessingDateModel();

            model.AutoBillingProcessingDatesId = Faker.RandomNumber.Next();
            model.BillingType = "Membership";
            model.InvoiceType = Faker.Lorem.Sentence();
            model.ThroughDate = DateTime.Now;
            model.EffectiveDate = DateTime.Now;
            model.Status = Faker.RandomNumber.Next();
            model.IsLastDayOfThroughDate = Faker.RandomNumber.Next();
            model.IsLastDayOfEffectiveDate = Faker.RandomNumber.Next();
            model.LastUpdated = DateTime.Now;

            return model;
        }

        public static AutoBillingSettingModel GetAutoBillingSettingModel()
        {
            AutoBillingSettingModel model = new AutoBillingSettingModel();

            //model.AutoBillingsettingsId = Faker.RandomNumber.Next();
            model.EnableAutomatedBillingForMembership = 1;//Faker.RandomNumber.Next();
            model.AutomatedCoordinatorForMembership = Faker.Name.First();
            model.IsPauseOrEnableForMembership = 0; //Faker.RandomNumber.Next();
            model.EmailForNotification = Faker.Internet.Email();
            model.SmsforNotification = Faker.Phone.Number();

            return model;
        }

        public static AutoBillingJobModel GetAutoBillingJobModel()
        {
            AutoBillingJobModel model = new AutoBillingJobModel();

            model.AutoBillingJobId = Faker.RandomNumber.Next();
            model.JobType = Faker.Lorem.Sentence();
            model.BillingType = "Membership";
            model.Create = DateTime.Now;
            model.Start = DateTime.Now;
            model.End = DateTime.Now;
            model.Status = Faker.RandomNumber.Next();


            return model;
        }



        public static AutoBillingNotificationModel GetAutoBillingNotificationModel()
        {
            AutoBillingNotificationModel model = new AutoBillingNotificationModel();

            model.AutoBillingNotificationId = Faker.RandomNumber.Next();
            model.AbpdId = Faker.RandomNumber.Next();
            model.BillingType = Faker.Lorem.Sentence();
            model.InvoiceType = Faker.Lorem.Sentence();
            model.NotificationType = Faker.Lorem.Sentence();
            model.NotificationSentDate = DateTime.Now;

            return model;
        }

        public static PaymentTransactionModel GetPaymentTransactionModel()
        {
            PaymentTransactionModel model = new PaymentTransactionModel();

            model.PaymentTransactionId = Math.Abs(Faker.RandomNumber.Next());
            model.TransactionDate = DateTime.Now;
            model.ShoppingCartId = Faker.RandomNumber.Next();
            model.EntityId = 1;
            model.ReceiptId = 1;
            model.Amount = Faker.RandomNumber.Next();
            model.TransactionType = 0;
            model.PaymentType = "CreditCard";
            model.CardType = Faker.Lorem.Sentence();
            model.CreditCardHolderName = Faker.Lorem.Sentence();
            model.AccountNumber = Faker.Lorem.Sentence();
            model.RoutingNumber = Faker.Lorem.Sentence();
            model.BankName = Faker.Lorem.Sentence();
            model.AccountHolderName = Faker.Lorem.Sentence();
            model.AccountType = Faker.Lorem.Sentence();
            model.RefId = Faker.Lorem.Sentence();
            model.AuthCode = Faker.Lorem.Sentence();
            model.ResponseCode = Faker.Lorem.Sentence();
            model.MessageDetails = Faker.Lorem.Sentence();
            model.Status = 1;
            model.Result = Faker.RandomNumber.Next();
            model.IsAdjusted = Faker.RandomNumber.Next();
            model.ReferenceNumber = Faker.Lorem.Sentence();
            model.ReferenceTransactionId = Faker.RandomNumber.Next().ToString();
            model.TransactionId = Faker.Lorem.Sentence();
            model.ResponseDetails = Faker.Lorem.Sentence();



            return model;
        }

        public static ReceiptHeaderModel GetReceiptHeaderModel()
        {
            ReceiptHeaderModel model = new ReceiptHeaderModel();

            model.Receiptid = 0;
            model.Date = DateTime.Now;
            model.StaffId = Faker.RandomNumber.Next();
            model.PaymentMode = Faker.Lorem.Sentence();
            model.PaymentTransactionId = Faker.RandomNumber.Next();
            model.CheckNo = Faker.Lorem.Sentence();
            model.Status = 0;
            model.OrganizationId = Faker.RandomNumber.Next();

            var paymenttransaction = GetPaymentTransactionModel();
            model.PaymentTransactionModel.Add(paymenttransaction);

            var receiptdetail = GetReceiptDetailModel();
            model.ReceiptDetailModel.Add(receiptdetail);



            return model;
        }

        public static ReceiptDetailModel GetReceiptDetailModel()
        {
            ReceiptDetailModel model = new ReceiptDetailModel();

            model.ReceiptDetailId = 0;
            model.ReceiptHeaderId = 1;
            model.PersonId = Faker.RandomNumber.Next();
            model.Quantity = Faker.RandomNumber.Next();
            model.Rate = Faker.RandomNumber.Next();
            model.Amount = Faker.RandomNumber.Next();
            model.Status = 1;
            model.Description = Faker.Lorem.Sentence();
            model.MembershipFeeId = Faker.RandomNumber.Next();
            model.InvoiceId = Faker.RandomNumber.Next(100);
            model.InvoiceDetailId = 0;
            model.Tax = Faker.RandomNumber.Next();
            model.PastDueInvoiceDetailRef = Faker.RandomNumber.Next();
            model.ItemType = Faker.RandomNumber.Next();

            return model;
        }

        public static BillingDocumentModel GetBillingDocumentModel()
        {
            BillingDocumentModel model = new BillingDocumentModel();

            model.BillingDocumentId = Faker.RandomNumber.Next();
            model.CreatedDate = DateTime.Now;
            model.InvoiceType = Faker.Lorem.Sentence();
            model.BillingType = "Membership";
            model.IsFinalized = Faker.RandomNumber.Next();
            model.Status = 1;
            model.AbpdId = Faker.RandomNumber.Next();
            model.ThroughDate = DateTime.Now;
            model.EffectiveDate = DateTime.Now;
            model.InvoiceCount = Faker.RandomNumber.Next();
            model.TotalAmount = Faker.RandomNumber.Next();

            return model;
        }

        public static ShoppingCartModel GetShoppingCartModel()
        {
            ShoppingCartModel model = new ShoppingCartModel();

            model.TransactionDate = DateTime.Now;
            model.UserId = Faker.RandomNumber.Next(100);
            model.SessionId = Faker.Lorem.Sentence();
            model.ReceiptId = 1;// Faker.RandomNumber.Next();
            model.PaymentStatus = 0;
            model.PersonId = 1;

            //var receiptheader = GetReceiptHeaderModel();
            // model.Receipt = receiptheader;

            var staffuser = GetStaffUserModel();
            model.User = staffuser;

            var shoppingcartdetails = GetShoppingCartDetailModel();
            model.ShoppingCartDetails.Add(shoppingcartdetails);

            return model;

        }

        public static NotesModel GetNotesModel()
        {
            NotesModel model = new NotesModel();

            model.EntityId = Faker.RandomNumber.Next();
            model.Notes = Faker.Lorem.Sentence();
            model.Severity = Faker.Internet.DomainName();
            model.DisplayOnProfile = 0;
            model.CreatedOn = DateTime.Today;
            model.CreatedBy = Faker.Internet.UserName();
            model.ModifiedOn = DateTime.Today;
            model.ModifiedBy = Faker.Internet.UserName();
            model.Status = 1;

            return model;
        }


        public static ShoppingCartDetailModel GetShoppingCartDetailModel()
        {
            ShoppingCartDetailModel model = new ShoppingCartDetailModel();

            model.ShoppingCartDetailId = Faker.RandomNumber.Next();
            model.ShoppingCartId = Faker.RandomNumber.Next(100);
            model.Description = Faker.Lorem.Sentence();
            model.ItemType = Faker.RandomNumber.Next();
            model.ItemId = Faker.RandomNumber.Next();
            model.Selected = Faker.RandomNumber.Next();
            model.Price = Faker.RandomNumber.Next();
            model.Quantity = Faker.RandomNumber.Next();
            model.Tax = Faker.RandomNumber.Next();
            model.ItemGroup = Faker.Lorem.Sentence();
            model.ItemGroupDescription = Faker.Lorem.Sentence();
            model.MembershipId = Faker.RandomNumber.Next();

            return model;
        }

        public static LookupModel GetLookupModel()
        {
            LookupModel model = new LookupModel();

            model.Group = Faker.Company.Name();
            model.Values = Faker.Lorem.Sentence();
            model.Status = 1;
            return model;
        }

        public static BillingCycleModel GetBillingcycle()
        {
            BillingCycleModel model = new BillingCycleModel();
            model.CycleName = Faker.Company.Name();
            model.RunDate = DateTime.Today;
            model.ThroughDate = DateTime.Today.AddDays(-7);
            model.Status = 1;

            var billingbatch = new Billingbatch();

            return model;
        }

        public static BillingJobModel GetBillingJob()
        {

            BillingJobModel model = new BillingJobModel();

            model.BillingCycleId = 1;
            model.CreateDate = DateTime.Today;
            model.StartDate = DateTime.Today;
            model.EndDate = DateTime.Today.AddMonths(1);
            model.Status = 0;//: Job created

            return model;
        }

        public static AutoBillingEmailNotificationModel GetAutobillingemailnoticicationModel()
        {
            AutoBillingEmailNotificationModel model = new AutoBillingEmailNotificationModel();

            model.EmailAddresses = new string[] { "rohitt@anittectnologies.com", "rohitt@mwembermax.com" };
            model.Subject = "From Membermax Unit Test (GetAutobillingemailnoticicationModel)";
            model.Title = Faker.Company.Name();
            model.BillingType = "Membership";
            model.ProcessDate = DateTime.Today.ToString();
            model.ThroughDate = DateTime.Today.AddDays(-5).ToString();
            model.TotalDue = Faker.RandomNumber.Next();
            model.Approved = 0;
            model.Declined = 1;


            return model;

        }

        public static EmailMessageModel GetEmailMessageModel()
        {
            EmailMessageModel model = new EmailMessageModel();

            model.EntityId = Faker.RandomNumber.Next();
            model.InvoiceId = Faker.RandomNumber.Next();
            model.ReceiptId = Faker.RandomNumber.Next();
            model.Subject = "From Membermax Unit Test(GetEmailMessageModel)";
            model.MessageBody = Faker.Lorem.Sentence();
            model.ReceipientAddress = "rohitt@anittechnologies.com";//Faker.Internet.Email();
            model.PdfData = GetByteArray(10);


            return model;
        }

        public static EntityModel GetEntityModel()
        {
            EntityModel model = new EntityModel();

            model.EntityId = Faker.RandomNumber.Next();
            model.OrganizationId = 1;
            model.Name = Faker.Name.FullName();

            return model;
        }

        //public static MembershipReportConfigurationModel GetMembershipReportConfigurationModel()
        //{
        //    MembershipReportConfigurationModel model = new MembershipReportConfigurationModel();

        //    //model.MembershipReportId = 1; // Faker.RandomNumber.Next();
        //    model.Categories = "1";
        //    model.MembershipTypes = "";// "1";
        //    model.Status = "1";
        //    model.Fields = "1,2,3,4";
        //    model.Description = Faker.Lorem.Sentence();
        //    model.Title = Faker.Name.Prefix();
        //    model.UserId = 1;
        //    model.Users = "1";
        //    return model;

        //}

        public static ReportParameterModel GetReportParameterModel()
        {
            ReportParameterModel model = new ReportParameterModel();
            model.ReportParameterId = 1; // Faker.RandomNumber.Next();
            model.Parameter = "Membership Start Date";
            model.CategoryId = 1;
            model.Type = "Date";

            return model;
        }

        public static ReportParameterModel GetReportParameterDateFilterModel(string strdate)
        {
            ReportParameterModel model = new ReportParameterModel();
            model.ReportParameterId = Faker.RandomNumber.Next();
            model.Parameter = strdate;
            model.CategoryId = 1;
            model.Type = "Date";

            return model;
        }

        public static ReportSortOrderModel GetReportSortorderModel()
        {
            ReportSortOrderModel model = new ReportSortOrderModel();

            model.ReportSortOrderId = Faker.RandomNumber.Next( 1,1000);
            model.ReportId=1;
            model.FieldName=Faker.Name.First();
            model.Order="ASC";

            return model;
        }

        public static ReportFieldModel GetReportFieldModel()
        {
            ReportFieldModel model = new ReportFieldModel();

            model.ReportFieldId = Faker.RandomNumber.Next(1,1000);
            model.ReportCategoryId = Faker.RandomNumber.Next(1,1000);
            model.FieldName = Faker.Name.First();
            model.TableName = Faker.Name.Last();
            model.FieldTitle = Faker.Company.Name();

            return model;
        }

        public static MembershipConnectionModel GetMembershipConnectionModel()
        {
            MembershipConnectionModel model = new MembershipConnectionModel();

            model.MembershipConnectionId = Faker.RandomNumber.Next(1,1000);
            model.MembershipId = 1;
            model.EntityId = Faker.RandomNumber.Next();
            model.Status = 1;

            //Add Membership
            //var membership = GetMembershipModel();
            //membership.MembershipTypeId = 1;            
            //model.Membership= membership;

            return model;
        }

        public static PromoCodeModel GetPromoCodeModel()
        {
            PromoCodeModel model = new PromoCodeModel();

            model.PromoCodeId = Faker.RandomNumber.Next();
            model.Code = Faker.RandomNumber.Next().ToString();
            model.Description = Faker.Name.FullName();
            model.ExpirationDate = DateTime.Now.AddDays(7);
            model.Status = 1;
            model.StartDate = DateTime.Now;
            model.GlAccountId = Faker.RandomNumber.Next();

            return model;
        }

    }
}