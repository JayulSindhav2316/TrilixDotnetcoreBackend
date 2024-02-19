using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Max.Data.DataModel;
using System.Data;



namespace Max.Services.Tests
{
    public class ReportsTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;
        
        public ReportsTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateReport_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                // ReportModel model = TestDataGenerator.GetReportModel();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();

                var report = await ReportService.CreateMembershipReport(model);
                Assert.True(report.MembershipReportId > 0, "Report Created.");
            }

        }

        [Fact]
        public async void GetReport_GetMembershipReport_By_UserId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newStaff = await staffUserService.CreateStaffUser(staff);

                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                // ReportModel model = TestDataGenerator.GetReportModel();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();
                var report = await ReportService.CreateMembershipReport(model);              

                var selectedrecord = await ReportService.GetMembershipReportByUserId(newStaff.UserId);

                Assert.True(!Extenstions.IsNullOrEmpty(selectedrecord), "report has record of selected UserId.");
            }

        }

        [Fact]
        public async void DeleteReport_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                // ReportModel model = TestDataGenerator.GetReportModel();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();

                var report = await ReportService.CreateMembershipReport(model);

                var deletedReport = await ReportService.DeleteMembershipReport(report.MembershipReportId);

                Assert.True(deletedReport == true, "Report Deleted.");
            }

        }

        [Theory] //Category Filter
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("1,2")]
        [InlineData("3")] // Data for this category has not been generated so assert should have no record
        public async void GetReport_GetMembershipReport_ByCategoryId(string filtercategory)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                //Creating Membership Data

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                //Person.EntityId = 1;
                var newBillablePerson1 = await PersonService.CreatePerson(Person);

                newBillablePerson1.EntityId = 1;

                //add another person
                 Person = TestDataGenerator.GetPersonModel();
                //Person.EntityId = 1;
                var newBillablePerson2 = await PersonService.CreatePerson(Person);

                newBillablePerson2.EntityId = 2;

                var EntityService = scope.ServiceProvider.GetService<IEntityService>();
                EntityModel entitymodel = TestDataGenerator.GetEntityModel();
                var newentity1 = await EntityService.GetEntitiesByEntityIds(newBillablePerson1.EntityId.ToString());
                var newentity2 = await EntityService.GetEntitiesByEntityIds(newBillablePerson2.EntityId.ToString());

                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();

                // Create CategoryId 1

                MembershipCategoryModel membershipcategorymodel1 = TestDataGenerator.GetMembershipCategoryModel();
                var category1 = await membershipCategoryService.CreateMembershipCategory(membershipcategorymodel1);

                // add another Category : CategoryId 2
                MembershipCategoryModel membershipcategorymodel2 = TestDataGenerator.GetMembershipCategoryModel();
                var category2 = await membershipCategoryService.CreateMembershipCategory(membershipcategorymodel2);

                // Add membershipType with Category 1
                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel1 = TestDataGenerator.GetMembershipTypeModel();
                membershiptypemodel1.Category =  1; 
                var newMembershipType1 = await membershipTypeService.CreateMembershipType(membershiptypemodel1);

              

                //add another with category 2
                MembershipTypeModel membershiptypemodel2 = TestDataGenerator.GetMembershipTypeModel();
                membershiptypemodel2.Category = 2;
                var newMembershipType2 = await membershipTypeService.CreateMembershipType(membershiptypemodel2);

               

                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel membershipmodel1 = TestDataGenerator.GetMembershipModel();
                membershipmodel1.MembershipTypeId = newMembershipType1.MembershipTypeId ;
                membershipmodel1.MembershipType = membershiptypemodel1; //membershiptypemodel;
                membershipmodel1.BillableEntityId = 1;
                var newmembership_cat1=await MembershipService.CreateMembership(membershipmodel1);



                // add another membership with CategoryId 2

                MembershipModel membershipmodel2 = TestDataGenerator.GetMembershipModel();
                membershipmodel2.MembershipTypeId = newMembershipType2.MembershipTypeId ;
                membershipmodel2.MembershipType = membershiptypemodel2;
                membershipmodel2.BillableEntityId = 2;
                var newmembership_cat2 = await MembershipService.CreateMembership(membershipmodel2);

               

                var MembershipconnectionService = scope.ServiceProvider.GetService<IMembershipConnectionService>();
                MembershipConnectionModel membershipconnectionmodel1 = TestDataGenerator.GetMembershipConnectionModel();
                membershipconnectionmodel1.MembershipId = 1;               
                membershipconnectionmodel1.EntityId = 1;
                var newconnection1 = await MembershipconnectionService.CreateMembershipConnection(membershipconnectionmodel1);

                // add another membershipconnection
                MembershipConnectionModel membershipconnectionmodel2 = TestDataGenerator.GetMembershipConnectionModel();
                membershipconnectionmodel2.MembershipId = 2;               
                membershipconnectionmodel2.EntityId = 2;
                var newconnection2 = await MembershipconnectionService.CreateMembershipConnection(membershipconnectionmodel2);

                var ReportFieldService = scope.ServiceProvider.GetService<IReportFieldService>();
                ReportFieldModel reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 1;
                reportfieldmodel.FieldName = "FirstName";
                reportfieldmodel.TableName = "person";
                await ReportFieldService.CreateReportField(reportfieldmodel);

                reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 2;
                reportfieldmodel.FieldName = "membershipTypeId";
                reportfieldmodel.TableName = "membershiptype";
                await ReportFieldService.CreateReportField(reportfieldmodel);

                reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 3;
                reportfieldmodel.FieldName = "Name";
                reportfieldmodel.TableName = "membershiptype";
                await ReportFieldService.CreateReportField(reportfieldmodel);

                reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 4;
                reportfieldmodel.FieldName = "membershipCategoryId";
                reportfieldmodel.TableName = "membershipcategory";
                await ReportFieldService.CreateReportField(reportfieldmodel);

                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();
                model.Categories = filtercategory.ToString();                
                var report = await ReportService.CreateMembershipReport(model);

                var report1 = await ReportService.GetMembershipReport(report.MembershipReportId);
                DataTable dt = report1.Rows;

                var temparray = filtercategory.Split(',');

                if (filtercategory=="3") // Data has  been created for category 1 and 2 so category 3 has no row
                {
                    Assert.True(report1.Rows.Rows.Count == 0);
                }
                else 
                { 
                    for(int i=0; i< temparray.Length; i++ )
                    {
                        Assert.True(dt.Rows.Count == temparray.Length && dt.Rows[i].ItemArray[3].ToString() == temparray[i].ToString());
                    }
                }
            }

        }

        [Fact]
        public async void GetReport_GetMembershipReport_WithoutFilter()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();
                //model.Filters.Add(filterModel);
                var report = await ReportService.CreateMembershipReport(model);

                var report1 = await ReportService.GetMembershipReport(report.MembershipReportId);
                Assert.True(report1 != null, "Report Exists.");

            }

        }

        [Theory] //Balance Due Filter
        [InlineData("EQ", 10)]
        [InlineData("NEQ", 10)]
        [InlineData("GT", 10)]
        [InlineData("LT", 10)]       
     
        public async void GetReport_GetMembershipReport_WithCurrencyFilter(string strfilter, decimal filterdueamount)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {


                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                //Creating Membership Data

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                //Person.EntityId = 1;
                var newBillablePerson = await PersonService.CreatePerson(Person);

                newBillablePerson.EntityId = 1;

                var EntityService = scope.ServiceProvider.GetService<IEntityService>();
                EntityModel entitymodel = TestDataGenerator.GetEntityModel();
               
                
                var newentity = await EntityService.GetEntitiesByEntityIds(newBillablePerson.EntityId.ToString());                

                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();

                MembershipCategoryModel membershipcategorymodel = TestDataGenerator.GetMembershipCategoryModel();
                var category = await membershipCategoryService.CreateMembershipCategory(membershipcategorymodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                membershiptypemodel.Category = membershipcategorymodel.MembershipCategoryId;
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);

                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();
                MembershipFeeModel membershipfeemodel = TestDataGenerator.GetMembershipFeeModel();
                membershipfeemodel.MembershipTypeId = newMembershipType.MembershipTypeId;
                membershipfeemodel.FeeAmount = 150;
                var newmembershipFee = await membershipFeeService.CreateMembershipFee(membershipfeemodel);


                var BillingFeeService = scope.ServiceProvider.GetService<IBillingFeeService>();
                BillingFeeModel billingfeemodel = new BillingFeeModel();
                billingfeemodel.MembershipId = 1;
                billingfeemodel.MembershipFeeId = 1;
                billingfeemodel.Fee = 150;
                billingfeemodel.Status = 1;
                var BillingFee = await BillingFeeService.CreateBillingFee(billingfeemodel);

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel invoicemodel = new InvoiceModel(); //TestDataGenerator.GetInvoiceModel();


                invoicemodel.InvoiceId = 1;
                invoicemodel.Date = DateTime.Now;                
                invoicemodel.EntityId = 1;
                invoicemodel.DueDate = DateTime.Now;
                invoicemodel.BillingType = "CreditCard";
                invoicemodel.InvoiceType = "Dues";
                invoicemodel.MembershipId = 1;
                invoicemodel.BillableEntityId = 1;
                //invoicemodel.PaymentTransactionId = 1;
                invoicemodel.Status = 1;

                //Add InvoiceDetaails

                var invoicedetailModel = TestDataGenerator.GetInvoiceDetailModel();
                invoicedetailModel.InvoiceId = 1;
                invoicedetailModel.Amount = 150;
                invoicedetailModel.Status = 1;
                invoicemodel.InvoiceDetails.Add(invoicedetailModel);

                var invoice = await InvoiceService.CreateInvoice(invoicemodel);

                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();               
                ReceiptHeaderModel modelReceiptHeader = new ReceiptHeaderModel();
                //model.Receiptid = 0;
                modelReceiptHeader.Date = DateTime.Now;
                modelReceiptHeader.StaffId = 1;
                modelReceiptHeader.PaymentMode = "Credit Card";
                modelReceiptHeader.PaymentTransactionId = 1;
                //model.CheckNo = Faker.Lorem.Sentence();
                modelReceiptHeader.Status = 1;
                modelReceiptHeader.OrganizationId = 1;

                var paymenttransaction = TestDataGenerator.GetPaymentTransactionModel();
                modelReceiptHeader.PaymentTransactionModel.Add(paymenttransaction);

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var ReceiptDetailService = scope.ServiceProvider.GetService<IReceiptDetailService>();
                // ReceiptDetailModel modelReceiptDetail = TestDataGenerator.GetReceiptDetailModel();
                ReceiptDetailModel modelReceiptDetail = new ReceiptDetailModel();
                modelReceiptDetail.ReceiptHeaderId = 1;
                modelReceiptDetail.PersonId = 1;
                modelReceiptDetail.Quantity = 1;
                modelReceiptDetail.Rate = 160;


                switch (strfilter)
                {
                    case "EQ":
                        modelReceiptDetail.Amount = invoice.Amount - filterdueamount;
                        break;
                    case "NEQ":
                        modelReceiptDetail.Amount = invoice.Amount;
                        break;
                    case "GT":
                        modelReceiptDetail.Amount = invoice.Amount - (filterdueamount * 2);
                        break;
                    case "LT":
                        modelReceiptDetail.Amount = invoice.Amount- (filterdueamount / 2);
                        break;
                }               
                modelReceiptDetail.Status = 1;
                modelReceiptDetail.Description = "Test Receipt Detail";
                modelReceiptDetail.MembershipFeeId = 1;
                modelReceiptDetail.InvoiceId = 1;
                modelReceiptDetail.InvoiceDetailId = 1;

                var receiptdetail = await ReceiptDetailService.CreateReceipt(modelReceiptDetail);


                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel membershipmodel = TestDataGenerator.GetMembershipModel();
                membershipmodel.MembershipTypeId = newMembershipType.MembershipTypeId;
                membershipmodel.BillableEntityId = 1;

                var membership = await MembershipService.CreateMembership(membershipmodel);

                var MembershipconnectionService = scope.ServiceProvider.GetService<IMembershipConnectionService>();
                MembershipConnectionModel membershipconnectionmodel = TestDataGenerator.GetMembershipConnectionModel();              
                membershipconnectionmodel.Membership = membershipmodel;
                membershipconnectionmodel.EntityId = 1;
                var newmembershipconnection = await MembershipconnectionService.CreateMembershipConnection(membershipconnectionmodel);

                var ReportFieldService = scope.ServiceProvider.GetService<IReportFieldService>();
                ReportFieldModel reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 1;
                reportfieldmodel.FieldName = "FirstName";
                reportfieldmodel.TableName = "person";
                var reportField = await ReportFieldService.CreateReportField(reportfieldmodel);

                //add another field
                reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 2;
                reportfieldmodel.FieldName = "BalanceDue";               
                reportfieldmodel.TableName = "membership";
                await ReportFieldService.CreateReportField(reportfieldmodel);

                var reportParameterService = scope.ServiceProvider.GetService<IReportParameterService>();
                ReportParameterModel reportParameter = TestDataGenerator.GetReportParameterDateFilterModel("Balance Due");
                var newParameter = await reportParameterService.CreateReportParameter(reportParameter);

                ReportFilterModel filterModel = new ReportFilterModel();
                filterModel.ReportFilterId = Faker.RandomNumber.Next(1, 1000);
                filterModel.ReportId = 1;
                filterModel.ParameterId = newParameter.ReportParameterId;
                filterModel.Operator = strfilter;

                filterModel.Value = filterdueamount.ToString();


                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();
                model.ReportFilters.Add(filterModel);
                var report = await ReportService.CreateMembershipReport(model);

                var report1 = await ReportService.GetMembershipReport(report.MembershipReportId);                

                if (strfilter == "EQ")
                {
                    Assert.True(Decimal.Parse(report1.Rows.Rows[0].ItemArray[1].ToString()) == filterdueamount, "due amount is equal to " + filterdueamount);
                }
                else if (strfilter == "NEQ")
                {
                    Assert.True(Decimal.Parse(report1.Rows.Rows[0].ItemArray[1].ToString()) != filterdueamount, "due amount is not equal to " + filterdueamount);
                }
                else if (strfilter == "LT")
                {
                    Assert.True(Decimal.Parse(report1.Rows.Rows[0].ItemArray[1].ToString()) < filterdueamount, "due amount is less than " + filterdueamount);
                }
                else if (strfilter == "GT")
                {
                    Assert.True(Decimal.Parse(report1.Rows.Rows[0].ItemArray[1].ToString()) > filterdueamount, "due amount is greater than " + filterdueamount);
                }

            }

        }



        [Theory]       
        [InlineData("Membership Start Date", "EQ", "01/01/2021")]       
        [InlineData("Membership Start Date", "NEQ", "01/01/1950")]     
        [InlineData("Membership Start Date", "GT", "01/01/1950")]      
        [InlineData("Membership Start Date", "LT", "01/01/2100")]
        [InlineData("Membership End Date", "EQ", "01/01/2021")]
        [InlineData("Membership End Date", "NEQ", "01/01/1950")]
        [InlineData("Membership End Date", "GT", "01/01/1950")]    
        [InlineData("Membership End Date", "LT", "01/01/2100")]
        //[InlineData("Membership Termination Date", "EQ", "01/01/2021")] // Column is not in Report
        //[InlineData("Membership Termination Date", "NEQ","01/01/1950")] // Column is not in Report
        //[InlineData("Membership Termination Date", "GT", "01/01/1950")] // Column is not in Report
        //[InlineData("Membership Termination Date", "LT", "01/01/2100")] // Column is not in Report
        public async void GetReport_GetMembershipReport_WithDtFilter(string strparameter, string strfilter, string membershipfilterdate)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                //Creating Membership Data

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                //Person.EntityId = 1;
                var newBillablePerson = await PersonService.CreatePerson(Person);

                newBillablePerson.EntityId = 1; 

                var EntityService = scope.ServiceProvider.GetService<IEntityService>();
                EntityModel entitymodel = TestDataGenerator.GetEntityModel();
                entitymodel.PersonId = newBillablePerson.PersonId;
                //var newentity = await EntityService.CreateEntity(entitymodel);
                var newentity = await EntityService.GetEntitiesByEntityIds(newBillablePerson.EntityId.ToString());

                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();

                MembershipCategoryModel membershipcategorymodel = TestDataGenerator.GetMembershipCategoryModel();
                var category = await membershipCategoryService.CreateMembershipCategory(membershipcategorymodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                membershiptypemodel.Category = membershipcategorymodel.MembershipCategoryId;
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);

                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();
                MembershipFeeModel membershipfeemodel = TestDataGenerator.GetMembershipFeeModel();
                membershipfeemodel.MembershipTypeId = newMembershipType.MembershipTypeId;
                var newmembershipFee = await membershipFeeService.CreateMembershipFee(membershipfeemodel);


                var BillingFeeService = scope.ServiceProvider.GetService<IBillingFeeService>();                
                BillingFeeModel billingfeemodel = new BillingFeeModel();
                billingfeemodel.MembershipId = 1;
                billingfeemodel.MembershipFeeId = 1;
                billingfeemodel.Fee = 150;
                billingfeemodel.Status = 1;
                var BillingFee = await BillingFeeService.CreateBillingFee(billingfeemodel);



                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel membershipmodel = TestDataGenerator.GetMembershipModel();
                membershipmodel.MembershipTypeId = newMembershipType.MembershipTypeId;
                membershipmodel.BillableEntityId = 1;
                switch (strparameter)
                {
                    case "Membership Start Date":
                        membershipmodel.StartDate = DateTime.Parse("01/01/2021");
                        break;
                    case "Membership End Date":                       
                        membershipmodel.EndDate = DateTime.Parse("01/01/2021");
                        break;
                    case "Membership Termination Date":
                        membershipmodel.TerminationDate = DateTime.Parse("01/01/2021");
                        break;                       
                }

               
                var membership = await MembershipService.CreateMembership(membershipmodel);


                var MembershipconnectionService = scope.ServiceProvider.GetService<IMembershipConnectionService>();
                MembershipConnectionModel membershipconnectionmodel = TestDataGenerator.GetMembershipConnectionModel();
                //membershipconnectionmodel.PersonId = newBillablePerson.PersonId;
                //membershipconnectionmodel.MembershipId = newmembership.MembershipId;
                membershipconnectionmodel.Membership = membershipmodel;
                membershipconnectionmodel.EntityId = 1;
                var newmembershipconnection = await MembershipconnectionService.CreateMembershipConnection(membershipconnectionmodel);

                var ReportFieldService = scope.ServiceProvider.GetService<IReportFieldService>();
                ReportFieldModel reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 1;
                reportfieldmodel.FieldName = "FirstName";
                reportfieldmodel.TableName = "person";
                var reportField = await ReportFieldService.CreateReportField(reportfieldmodel);               

                //add another field
                reportfieldmodel = TestDataGenerator.GetReportFieldModel();
                reportfieldmodel.ReportFieldId = 2;               
                switch (strparameter)
                {
                    case "Membership Start Date":
                        reportfieldmodel.FieldName = "StartDate";
                        break;
                    case "Membership End Date":
                        reportfieldmodel.FieldName = "EndDate";
                        break;
                    case "Membership Termination Date":
                        reportfieldmodel.FieldName = "TerminationDate";
                        break;                       
                }
                reportfieldmodel.TableName = "membership";
                await ReportFieldService.CreateReportField(reportfieldmodel);
             

                var reportParameterService = scope.ServiceProvider.GetService<IReportParameterService>();
                ReportParameterModel reportParameter = TestDataGenerator.GetReportParameterDateFilterModel(strparameter);
                var newParameter = await reportParameterService.CreateReportParameter(reportParameter);

                ReportFilterModel filterModel = new ReportFilterModel();
                filterModel.ReportFilterId = Faker.RandomNumber.Next(1, 1000);
                filterModel.ReportId = 1;
                filterModel.ParameterId = newParameter.ReportParameterId;
                filterModel.Operator = strfilter;
                filterModel.Value = membershipfilterdate;


                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();               
                model.ReportFilters.Add(filterModel);
                var report = await ReportService.CreateMembershipReport(model);

                var report1 = await ReportService.GetMembershipReport(report.MembershipReportId);
               

                if (strfilter=="EQ")
                { 
                    Assert.True(DateTime.Parse(report1.Rows.Rows[0].ItemArray[1].ToString())==DateTime.Parse(membershipfilterdate), strparameter + "(01/01/2021) is equal to 01/01/2021" + membershipfilterdate);
                }
                else if (strfilter == "NEQ")
                {
                    Assert.True(DateTime.Parse(report1.Rows.Rows[0].ItemArray[1].ToString()) != DateTime.Parse(membershipfilterdate), strparameter + "(01/01/2021) is not equal to " + membershipfilterdate);
                }
                else if (strfilter == "LT")
                {
                    Assert.True(DateTime.Parse(report1.Rows.Rows[0].ItemArray[1].ToString()) < DateTime.Parse(membershipfilterdate), strparameter + " (01/01/2021) is less than " + membershipfilterdate);
                }
                else if (strfilter == "GT")
                {
                    Assert.True(DateTime.Parse(report1.Rows.Rows[0].ItemArray[1].ToString()) > DateTime.Parse(membershipfilterdate), strparameter + " (01/01/2021) is greater than " + membershipfilterdate);
                }                

            }

        }

        [Fact]
        public async void GetReport_GetMembershipReport_WithFilter()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var reportParameterService= scope.ServiceProvider.GetService<IReportParameterService>();
                ReportParameterModel reportParameter = TestDataGenerator.GetReportParameterModel();
                var newParameter = await reportParameterService.CreateReportParameter(reportParameter);


                ReportFilterModel filterModel = new ReportFilterModel();
                filterModel.ReportFilterId = 1; // Faker.RandomNumber.Next();            
                filterModel.ReportId = 1;
                filterModel.ParameterId = 1;
                filterModel.Operator = "GT";
                filterModel.Value = "01/01/2021";

                var ReportService = scope.ServiceProvider.GetService<IReportService>();
                MembershipReportConfigurationModel model = TestDataGenerator.GetMembershipReportConfigurationModel();
                model.ReportFilters.Add(filterModel);
                var report = await ReportService.CreateMembershipReport(model);

                var report1 = await ReportService.GetMembershipReport(1);
                Assert.True(report1 != null, "Report Exists.");

            }

        }     

    }
}