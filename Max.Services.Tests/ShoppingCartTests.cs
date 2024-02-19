using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static Max.Core.Constants;
using System;

namespace Max.Services.Tests
{
    public class ShoppingCartTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public ShoppingCartTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact] 
        public async void ShoppingCarts_Get_All()
        { 
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                
                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();
                
                await ShoppingCartService.CreateShoppingCart(model);

                model = TestDataGenerator.GetShoppingCartModel();

                await ShoppingCartService.CreateShoppingCart(model);

                var shoppingcart = await ShoppingCartService.GetAllShoppingCarts();

                Assert.True(!Extenstions.IsNullOrEmpty(shoppingcart), "ShoppingCarts has records.");
            }

        }

        [Fact]
        public async void ShoppingCarts_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();

                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();

                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);

                var newshoppingcart = await ShoppingCartService.GetShoppingCartById(shoppingcart.ShoppingCartId);

                Assert.True(newshoppingcart.ShoppingCartId == shoppingcart.ShoppingCartId, "ShoppingCarts returns selected Id.");
            }

        }

        [Fact]
        public async void ShoppingCarts_Get_By_UserId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();

                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();

                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);

                var newshoppingcart = await ShoppingCartService.GetShoppingCartByUserId((int)shoppingcart.UserId);

                Assert.True(newshoppingcart.UserId == shoppingcart.UserId, "ShoppingCarts returns selected UserId Id.");
            }

        }

        [Fact]
        public async void CreateShoppingCarts_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();

                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();

                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);

                Assert.True(shoppingcart.ShoppingCartId > 0, "ShoppingCarts Created.");
            }

        }

        [Fact]
        public async void UpdateShoppingCart_Status_To_Complete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();
                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);

                model.ShoppingCartId = shoppingcart.ShoppingCartId;
                model.PaymentStatus = 1;

                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel receiptmodel = TestDataGenerator.GetReceiptHeaderModel();
                var receiptheader = await ReceiptHeaderService.CreateReceipt(receiptmodel);

                await ShoppingCartService.UpdateShoppingCartPaymentStatus((int)shoppingcart.UserId, model.ShoppingCartId, (int)model.PaymentStatus,0,PaymentType.CHECK);

                var updatedshoppingcart = await ShoppingCartService.GetShoppingCartById(model.ShoppingCartId);

                Assert.True(updatedshoppingcart.PaymentStatus == 1, "ShoppingCartDetail PaymentStatus Updated.");
            }
            
        }

        [Fact]
        public async void ShoppingCarts_AddInvoice() 
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organizationmodel = TestDataGenerator.GetOrganizationModel();
                await OrganizationService.CreateOrganization(organizationmodel);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel personmodel = TestDataGenerator.GetPersonModel();
                await PersonService.CreatePerson(personmodel);

                var membershipservice = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel membershipmodel = TestDataGenerator.GetMembershipModel();
                var newmembership = await membershipservice.CreateMembership(membershipmodel);

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel Invoicemodel = TestDataGenerator.GetInvoiceModel();
                var Invoice = await InvoiceService.CreateInvoice(Invoicemodel);                

                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                ShoppingCartModel shoppingcartmodel = TestDataGenerator.GetShoppingCartModel();

                var shoppingcartitem = await ShoppingCartService.AddInvoiceToShoppingCart((int)shoppingcartmodel.UserId, Invoicemodel.InvoiceId);

                Assert.True(shoppingcartitem.ShoppingCartId > 0, "Invoice added in ShoppingCartDetail.");
            }

        }
        [Fact]
        public async void ShoppingCart_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();
                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);
                var newshoppingcart = await ShoppingCartService.GetShoppingCartById(shoppingcart.ShoppingCartId);

                await ShoppingCartService.DeleteShoppingCart(newshoppingcart.ShoppingCartId);

                var deletedshoppingcart = await ShoppingCartService.GetShoppingCartById(newshoppingcart.ShoppingCartId);

                Assert.True(deletedshoppingcart == null, "ShoppingCart Deleted.");
            }

        }

        [Fact]
        public async void ShoppingCart_Delete_Deletewithapprovedpayment()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();
                model.PaymentStatus = 2; //set payment status approved
                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);
                var newshoppingcart = await ShoppingCartService.GetShoppingCartById(shoppingcart.ShoppingCartId);

                

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => ShoppingCartService.DeleteShoppingCart(newshoppingcart.ShoppingCartId));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("Shopping Cart can not be deleted. There could be pending payments on them.", ex.Message);
            }

        }
                
        [Fact]
        public async void ShoppingCart_ApplyPromoCode()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();
                await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                var newPromocode = await promocodeService.CreatePromoCode(promocode);

                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();
                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);
                var newshoppingcart = await ShoppingCartService.GetShoppingCartById(shoppingcart.ShoppingCartId);

                await ShoppingCartService.ApplyPromoCode(newshoppingcart.ShoppingCartId, newPromocode.Code, newPromocode.Discount);

                var shoppingcartpromocodecheeck = await ShoppingCartService.GetShoppingCartById(shoppingcart.ShoppingCartId);

                Assert.True(shoppingcartpromocodecheeck.PromoCode == newPromocode.Code, "Promo Code Applied.");

            }

        }


        [Fact]
        public async void ShoppingCarts_AddReceipt()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();
                await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var ShoppingCartService = scope.ServiceProvider.GetService<IShoppingCartService>();
                ShoppingCartModel model = TestDataGenerator.GetShoppingCartModel();
                var shoppingcart = await ShoppingCartService.CreateShoppingCart(model);

                var shoppingcartitem = await ShoppingCartService.AddReceiptToShoppingCart((int)shoppingcart.UserId, 0);

                Assert.True(shoppingcartitem.ShoppingCartId > 0, "Receipt added in ShoppingCartDetail.");
            }

        }

    }
}
