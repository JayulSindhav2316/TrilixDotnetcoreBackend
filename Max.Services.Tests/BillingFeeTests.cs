using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class BillingFeeTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public BillingFeeTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateBillingFee_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var BillingFeeService = scope.ServiceProvider.GetService<IBillingFeeService>();
                BillingFeeModel model = TestDataGenerator.GetBillingfeeModel();
                var BillingFee = await BillingFeeService.CreateBillingFee(model);
                
                Assert.True(BillingFee.BillingFeeId>0 , "Billing Fee created.");
            }

        }
    }
}