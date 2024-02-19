using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;
using Xunit.Priority;

namespace Max.Services.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class PaymentTransactionTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public PaymentTransactionTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void PaymentTransaction_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();

                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();

                var paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);

                var newpaymenttransaction = await PaymentTransactionService.GetPaymentTransactionById(paymenttransaction.PaymentTransactionId);

                Assert.True(newpaymenttransaction.PaymentTransactionId == paymenttransaction.PaymentTransactionId, "PaymentTransaction returns selected Id.");
            }

        }

        [Fact]
        public async void PaymentTransaction_Get_By_Date()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();

                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();

                var paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);

                var newpaymenttransaction = await PaymentTransactionService.GetPaymentTransactionsByDate((DateTime)paymenttransaction.TransactionDate);

                Assert.True(newpaymenttransaction.Where(x => x.TransactionDate == model.TransactionDate).Count() > 0, "PaymentTransaction returns selected transaction date.");
            }

        }

        [Fact, Priority(1)]
        public async void PaymentTransaction_Get_By_PersonId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();

                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();

                var paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);

                var newpaymenttransaction = await PaymentTransactionService.GetPaymentTransactionsByEntityIdAsync((int)paymenttransaction.EntityId);

                Assert.True(newpaymenttransaction.Where(x => x.EntityId == model.EntityId).Count() > 0, "PaymentTransaction returns selected person Id.");
            }

        }

        [Fact]
        public async void PaymentTransaction_Get_By_ReceiptId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();

                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();

                var paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);

                var newpaymenttransaction = await PaymentTransactionService.GetPaymentTransactionsByReceiptId((int)paymenttransaction.ReceiptId);

                Assert.True(newpaymenttransaction.Where(x => x.ReceiptId == model.ReceiptId).Count() > 0, "PaymentTransaction returns selected receipt Id.");
            }

        }

        [Fact]
        public async void PaymentTransaction_Get_By_TransactionType()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();

                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();

                var paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);

                var newpaymenttransaction = await PaymentTransactionService.GetPaymentTransactionsByTransactionType(paymenttransaction.TransactionType);

                Assert.True(newpaymenttransaction.Where(x => x.TransactionType == paymenttransaction.TransactionType).Count() > 0, "PaymentTransaction returns selected transaction type.");
            }

        }

        [Fact]
        public async void PaymentTransaction_Get_By_PaymentType()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();

                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();

                var paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);

                var newpaymenttransaction = await PaymentTransactionService.GetPaymentTransactionsByPaymentType(paymenttransaction.PaymentType);

                Assert.True(newpaymenttransaction.Where(x => x.PaymentType == paymenttransaction.PaymentType).Count() > 0, "PaymentTransaction returns selected payment type.");
            }

        }

        [Fact, Priority(0)]
        public async void CreatePaymentTransactions_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();

                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();

                var paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);

                Assert.True(paymenttransaction.PaymentTransactionId > 0, "PaymentTransaction Created.");
            }

        }

        [Theory]
        [InlineData("ALL")]
        [InlineData("VISA")]        
        public async void CreatePaymentTransactions_Get_cardtype_CreditCardReport(string cardtype)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                var newPerson = await PersonService.CreatePerson(Person);

                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();
                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();
                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();
                var paymenttransaction1 = await PaymentTransactionService.CreatePaymentTransaction(model);

                //Add another
                model = TestDataGenerator.GetPaymentTransactionModel();
                model.CardType = "VISA"; //add back dated record
                var paymenttransaction2 = await PaymentTransactionService.CreatePaymentTransaction(model);

                var creditcardreport = await PaymentTransactionService.GetCreditCardReport(cardtype, "DAY", DateTime.Today, DateTime.Today);               

                if (cardtype == "ALL")                {                 

                    Assert.False(creditcardreport.Count()<2, "all types");
                }
                else
                {
                    Assert.False(creditcardreport.Count() > 2, "only Visa type");
                }

            }

        }


        [Theory,Priority(2)]
        [InlineData("DAY")]
        [InlineData("MONTH")]
        [InlineData("DATERANGE")]
        public async void CreatePaymentTransactions_Get_searchtype_CreditCardReport(string searchtype)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                var newPerson = await PersonService.CreatePerson(Person);

                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();
                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();
                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();
                var paymenttransaction1= await PaymentTransactionService.CreatePaymentTransaction(model);

                //Add another
                model = TestDataGenerator.GetPaymentTransactionModel();
                model.TransactionDate = DateTime.Today.AddDays(-4); //add back dated record
                var paymenttransaction2 = await PaymentTransactionService.CreatePaymentTransaction(model);
                

                if (searchtype == "DATERANGE")
                {
                    var creditcardreport = await PaymentTransactionService.GetCreditCardReport("ALL", searchtype, DateTime.Today.AddDays(-5), DateTime.Today.AddDays(-2));

                    Assert.True(creditcardreport.All(x => Convert.ToDateTime(x.TransactionDate) < DateTime.Today), "Report has records."); 
                }
                else if (searchtype == "DAY")
                {
                    var creditcardreport = await PaymentTransactionService.GetCreditCardReport("ALL", searchtype, DateTime.Today, DateTime.Today);

                    Assert.True(creditcardreport.All(x => Convert.ToDateTime(x.TransactionDate) == DateTime.Today), "Report has records.");
                }
                else
                {
                    var creditcardreport = await PaymentTransactionService.GetCreditCardReport("ALL", searchtype, DateTime.Today, DateTime.Today);

                    Assert.True(creditcardreport.All(x=> Convert.ToDateTime(x.TransactionDate) >= DateTime.Today.FirstDayOfMonth() && Convert.ToDateTime(x.TransactionDate)<= DateTime.Today.LastDayOfMonth()), "Report has records.");
                }
                
            }

        }

        [Fact, Priority(-1)]
        public async void PaymentTransaction_Process_CreditCardPayment()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel orgmodel = TestDataGenerator.GetOrganizationModel_WithAccountingSetup();
                var organization = await OrganizationService.CreateOrganization(orgmodel); 

                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();
                GlAccountModel glmodel = TestDataGenerator.GetGlAccountModel();                
                var glaccount = await GlAccountService.CreateGlAccount(glmodel);

                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();
                modelReceiptHeader.OrganizationId = organization.OrganizationId;
                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                ShoppingCartModel shoppingCartmodel = TestDataGenerator.GetShoppingCartModel();
                var shoppingcart = await ShoppingCartService.CreateShoppingCart(shoppingCartmodel);             

                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();
                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();
                var paymenttransaction = await PaymentTransactionService.ProcessCreditPaymentTransaction(shoppingcart.ShoppingCartId);

                Assert.True(paymenttransaction == true , "Transaction completed.");
            }

        }

        [Fact]
        public async void PaymentTransaction_Update_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var paymenttransaction = new PaymentTransactionModel();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();
                PaymentTransactionModel model = TestDataGenerator.GetPaymentTransactionModel();
                paymenttransaction = await PaymentTransactionService.CreatePaymentTransaction(model);
            }

            //Create a new context
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newPaymenttranModel = mapper.Map<PaymentTransactionModel>(paymenttransaction);

                newPaymenttranModel.PaymentType = "Changed payment type";

                await PaymentTransactionService.UpdatePaymentTransaction(newPaymenttranModel);

                var updatedpaymenttransaction = await PaymentTransactionService.GetPaymentTransactionById(paymenttransaction.PaymentTransactionId);

                Assert.True(updatedpaymenttransaction.PaymentType == "Changed payment type", "PaymentTransaction Updated.");
            }

        }

    }
}