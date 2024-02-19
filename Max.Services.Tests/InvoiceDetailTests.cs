using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Max.Services.Tests
{
    public class InvoiceDetailTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public InvoiceDetailTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateInvoiceDetail_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var InvoiceDetailService = scope.ServiceProvider.GetService<IInvoiceDetailService>();

                InvoiceDetailModel model = TestDataGenerator.GetInvoiceDetailModel();

                var invoicedetail = await InvoiceDetailService.CreateInvoiceDetail(model);

                Assert.True(invoicedetail.InvoiceDetailId > 0, "InvoiceDetail Created.");
            }
            
        }

        [Fact]
        public async void InvoiceDetail_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var InvoiceDetailService = scope.ServiceProvider.GetService<IInvoiceDetailService>();

                InvoiceDetailModel model = TestDataGenerator.GetInvoiceDetailModel();

                await InvoiceDetailService.CreateInvoiceDetail(model);

                //Add another

                model = TestDataGenerator.GetInvoiceDetailModel();

                await InvoiceDetailService.CreateInvoiceDetail(model);

                var invoicedetails = await InvoiceDetailService.GetAllInvoiceDetails();

                Assert.True(!Extenstions.IsNullOrEmpty(invoicedetails), "InvoiceDetail has records.");
            }

        }

        [Fact]
        public async void InvoiceDetail_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var InvoiceDetailService = scope.ServiceProvider.GetService<IInvoiceDetailService>();

                InvoiceDetailModel model = TestDataGenerator.GetInvoiceDetailModel();

                var invoicedetails = await InvoiceDetailService.CreateInvoiceDetail(model);

                var returnedInvoicedetail = await InvoiceDetailService.GetInvoiceDetailById(invoicedetails.InvoiceDetailId);

                Assert.True(returnedInvoicedetail.InvoiceDetailId == invoicedetails.InvoiceDetailId, "InvoiceDetail returns selected Id.");
            }

        }

        [Fact(Skip = "Test not being Invoked. Need a fix")]
        public async void InvoiceDetail_GetByInvoiceId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
           
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var InvoiceDetailService = scope.ServiceProvider.GetService<IInvoiceDetailService>();

                InvoiceDetailModel model = TestDataGenerator.GetInvoiceDetailModel();

                var invoicedetail = await InvoiceDetailService.CreateInvoiceDetail(model);

                var returnedInvoicedetail = await InvoiceDetailService.GetAllInvoiceDetailsByInvoiceId((int)invoicedetail.InvoiceId);

                Assert.True(!Extenstions.IsNullOrEmpty(returnedInvoicedetail), "InvoiceDetail count has records by Invoice.");

            }

        }


    }
}