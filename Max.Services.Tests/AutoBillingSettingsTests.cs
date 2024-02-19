using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;
using Xunit.Priority;

namespace Max.Services.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class AutoBillingSettingsTests : IClassFixture<DependencySetupFixture>
    {       
        private ServiceProvider _serviceProvider;        

        public AutoBillingSettingsTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact,Priority(-2)]
        
        public async void Create_AutoBillingSetting_AddNew()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutobillingsettingService = scope.ServiceProvider.GetService<IAutoBillingSettingsService>();
                AutoBillingSettingModel autobillingsettingmodel = TestDataGenerator.GetAutoBillingSettingModel();
                var newAutoBillingSettings = await AutobillingsettingService.CreateAutoBillingSetting (autobillingsettingmodel);           
                                             
                Assert.True(newAutoBillingSettings.AutoBillingsettingsId > 0, "Auto Billing Setting Created.");
            }

        }

        [Fact]

        public async void Get_GetAutoBillingSetting()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutobillingsettingService = scope.ServiceProvider.GetService<IAutoBillingSettingsService>();

                //****** lines are commented to avoid second record in In memory database*****
                // AutoBillingSettingModel autobillingsettingmodel = TestDataGenerator.GetAutoBillingSettingModel();
                //var newAutoBillingSettings = await AutobillingsettingService.CreateAutoBillingSetting(autobillingsettingmodel);                

                var selectedrecord = await AutobillingsettingService.GetAutoBillingSetting();

                Assert.True(selectedrecord.AutoBillingsettingsId > 0, "Auto Billing Setting Created.");
            }

        }

        [Fact, Priority(-1)]
        public async void Update_Update_AutoBillingSetting()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutobillingsettingService = scope.ServiceProvider.GetService<IAutoBillingSettingsService>();

                //****** lines are commented to avoid second record in In memory database*****
                //AutoBillingSettingModel autobillingsettingModel = TestDataGenerator.GetAutoBillingSettingModel();
                // var newAutoBillingSettings = await AutobillingsettingService.CreateAutoBillingSetting(autobillingsettingModel);

                var selectedrecord = await AutobillingsettingService.GetAutoBillingSetting();

                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newautobillingsettingModel = mapper.Map<AutoBillingSettingModel>(selectedrecord);

                selectedrecord.EnableAutomatedBillingForMembership = 99;
                selectedrecord.EmailForNotification = "changedEmail@test.com";
                await AutobillingsettingService.UpdateAutoBillingSetting(newautobillingsettingModel);

                var updatedrecord = await AutobillingsettingService.GetAutoBillingSetting();

                Assert.True(updatedrecord.EmailForNotification == "changedEmail@test.com" && updatedrecord.EnableAutomatedBillingForMembership ==99, "Auto Billing Setting Updated.");
            }

        }

        [Fact]


        public async void Get_GetAutoBillingConfiguration()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();
                AutoBillingProcessingDateModel autobillprocessingdtmodel = TestDataGenerator.GetAutoBillingProcessingDateModel();
                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(autobillprocessingdtmodel);

                var AutobillingsettingService = scope.ServiceProvider.GetService<IAutoBillingSettingsService>();

                //****** lines are commented to avoid second record in In memory database*****
                //AutoBillingSettingModel autobillingsettingmodel = TestDataGenerator.GetAutoBillingSettingModel();
                //var newAutoBillingSettings = await AutobillingsettingService.CreateAutoBillingSetting(autobillingsettingmodel);

                var selectedrecord = await AutobillingsettingService.GetAutoBillingConfiguration();

                Assert.True(selectedrecord != null , "Configuration has records.");
            }

        }


    }
}