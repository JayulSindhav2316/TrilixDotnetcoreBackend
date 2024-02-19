using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;


namespace Max.Services.Tests
{
    public class BillingDocumentTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public BillingDocumentTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateBillingDocument_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var BillingDocumentsService = scope.ServiceProvider.GetService<IBillingDocumentsService>();

                BillingDocumentModel model = TestDataGenerator.GetBillingDocumentModel();

                var billingdocuments = await BillingDocumentsService.CreateAutoBillingDocument(model);

                Assert.True(billingdocuments.BillingDocumentId > 0, "BillingDocument Created.");
            }

        }

        [Fact]
        public async void BillingDocumentDetails_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var BillingDocumentsService = scope.ServiceProvider.GetService<IBillingDocumentsService>();

                BillingDocumentModel model = TestDataGenerator.GetBillingDocumentModel();

                await BillingDocumentsService.CreateAutoBillingDocument(model);

                //Add another

                model = TestDataGenerator.GetBillingDocumentModel();

                await BillingDocumentsService.CreateAutoBillingDocument(model);

                var glaccount = await BillingDocumentsService.GetAllBillingDocumentDetails();

                Assert.True(!Extenstions.IsNullOrEmpty(glaccount), "BillingDocument has records.");
            }

        }

    }
}
