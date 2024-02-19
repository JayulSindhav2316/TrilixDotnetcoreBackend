using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{
    public class AutoBillingTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public AutoBillingTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void AutoBillingJob_Due()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                //AutoBillingProcessingDatesService Call
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();
                AutoBillingProcessingDateModel autoBillingProcessingDatemodel = TestDataGenerator.GetAutoBillingProcessingDateModel();
                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(autoBillingProcessingDatemodel);
                //var newAutoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByBillingType(autoBillingProcessingDates.BillingType);

                //AutoBillingService Call
                var AutoBillingService = scope.ServiceProvider.GetService<IAutoBillingService>();
                AutoBillingJobModel autoBillingJobModelmodel = TestDataGenerator.GetAutoBillingJobModel();
                var autobillingjob = await AutoBillingService.CreatAutoBillingJob();

                //AutoBillingSettingService Call
                var AutoBillingSettingsService = scope.ServiceProvider.GetService<IAutoBillingSettingsService>();
                AutoBillingSettingModel autoBillingSettingsmodel = TestDataGenerator.GetAutoBillingSettingModel();
                var autoBillingSettings = await AutoBillingSettingsService.CreateAutoBillingSetting(autoBillingSettingsmodel);
                var newAutoBillingSettings = await AutoBillingSettingsService.GetAutoBillingSetting();

                //AutoBillingJobService Call
                var newAutobillingjob = await AutoBillingService.IsAutoBillingJobDue();
                Assert.True(newAutobillingjob == false , "AutoBilling Job is already created.");
            }

        }
        
    }
}