using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{
    public class AutoBillingProcessingDatesTests: IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public AutoBillingProcessingDatesTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateAutoBillingProcessingDates_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);
                Assert.True(autoBillingProcessingDates.AutoBillingProcessingDatesId > 0, "AutoBillingProcessingDate Created.");
            }

        }

        [Fact]
        public async void UpdateAutoBillingProcessingDates_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var newAutoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                model.AutoBillingProcessingDatesId = newAutoBillingProcessingDates.AutoBillingProcessingDatesId;
                model.InvoiceType = "Changed Invoice Type";

                await AutoBillingProcessingDatesService.UpdateAutoBillingProcessingDate(model);

                var updatedAutoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByABPDId(model.AutoBillingProcessingDatesId);

                Assert.True(updatedAutoBillingProcessingDates.InvoiceType == "Changed Invoice Type", "Auto Billing Processing Date Invoice Type Updated.");
            }

        }

        [Fact]
        public async void AutoBillingProcessingDates_Get_By_ABPDId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                var newAutobillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByABPDId(autoBillingProcessingDates.AutoBillingProcessingDatesId);

                Assert.True(newAutobillingProcessingDates.AutoBillingProcessingDatesId == autoBillingProcessingDates.AutoBillingProcessingDatesId, "Auto Billing Notification returns selected Id.");
            }

        }

        [Fact]
        public async void AutoBillingProcessingDates_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                //Add another

                model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDates();

                Assert.True(!Extenstions.IsNullOrEmpty(autoBillingProcessingDates), "Auto Billing Processing Dates has records.");
            }

        }

        [Fact]
        public async void AutoBillingProcessingDates_Get_By_BillingType()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                var newAutoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByBillingType(autoBillingProcessingDates.BillingType);

                Assert.True(newAutoBillingProcessingDates.BillingType == model.BillingType, "AutoBillingProcessingDates Found by Billing Type.");

            }

        }

        [Fact]
        public async void AutoBillingNotifications_Get_By_InvoiceType()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                var newAutoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByInvoiceType(autoBillingProcessingDates.InvoiceType);

                Assert.True(newAutoBillingProcessingDates.InvoiceType == model.InvoiceType, "AutoBillingProcessingDates Found by Invoice Type.");

            }

        }

        [Fact]
        public async void AutoBillingProcessingDates_Get_By_ThroughDate()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                var newAutoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByThroughDate(autoBillingProcessingDates.ThroughDate);

                Assert.True(newAutoBillingProcessingDates.Where(x => x.ThroughDate == model.ThroughDate).Count() > 0, "AutoBillingProcessingDates Found Through Date.");

            }

        }

        [Fact]
        public async void AutoBillingProcessingDates_Get_By_EffectiveDate()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                var newAutoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByEffectiveDate(autoBillingProcessingDates.EffectiveDate);

                Assert.True(newAutoBillingProcessingDates.Where(x => x.EffectiveDate == model.EffectiveDate).Count() > 0, "AutoBillingProcessingDates Found by Effective Date.");

            }

        }

        [Fact]
        public async void AutoBillingProcessingDates_Get_By_Status()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingProcessingDatesService = scope.ServiceProvider.GetService<IAutoBillingProcessingDatesService>();

                AutoBillingProcessingDateModel model = TestDataGenerator.GetAutoBillingProcessingDateModel();

                var autoBillingProcessingDates = await AutoBillingProcessingDatesService.CreateAutoBillingProcessingDate(model);

                var newAutoBillingProcessingDates = await AutoBillingProcessingDatesService.GetAutoBillingProcessingDatesByStatus((int)autoBillingProcessingDates.Status);

                Assert.True(newAutoBillingProcessingDates.Where(x => x.Status == model.Status).Count() > 0, "AutoBillingProcessingDates Found by Status.");

            }

        }

    }
}