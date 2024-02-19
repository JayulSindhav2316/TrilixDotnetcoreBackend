using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{
    public class AutoBillingDraftTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public AutoBillingDraftTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateAutoBillingDraft_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

                AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

                var autobillingdraft = await AutoBillingDraftService.CreateAutoBillingDraft(model);
                Assert.True(autobillingdraft.AutoBillingDraftId > 0, "AutoBillingDraft Created.");
            }

        }

        [Fact]
        public async void UpdateAutoBillingDraft_Update() 
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

                AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

                var newAutoBillingDraft = await AutoBillingDraftService.CreateAutoBillingDraft(model);

                model.AutoBillingDraftId = newAutoBillingDraft.AutoBillingDraftId;
                model.Name = "Changed Person Name";

                await AutoBillingDraftService.UpdateAutoBillingDraft(model);

                var updatedAutoBillingDraft = await AutoBillingDraftService.GetAutobillingDraftById(model.AutoBillingDraftId);

                Assert.True(updatedAutoBillingDraft.Name == "Changed Person Name", "Auto Billing Draft Person Name Updated.");
            }

        }

        [Fact]
        public async void AutoBillingDraft_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

                AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

                var autobillingdraft = await AutoBillingDraftService.CreateAutoBillingDraft(model);

                var newAutoBillingDraft = await AutoBillingDraftService.GetAutobillingDraftById(autobillingdraft.AutoBillingDraftId);

                Assert.True(newAutoBillingDraft.AutoBillingDraftId == autobillingdraft.AutoBillingDraftId, "AutoBillingDraft returns selected Id.");
            }

        }

        [Fact]
        public async void AutoBillingDraft_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

                AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

                await AutoBillingDraftService.CreateAutoBillingDraft(model);

                //Add another

                model = TestDataGenerator.GetAutoBillingDraftModel();

                await AutoBillingDraftService.CreateAutoBillingDraft(model);

                var commmunication = await AutoBillingDraftService.GetAllAutobillingDrafts();

                Assert.True(!Extenstions.IsNullOrEmpty(commmunication), "AutoBillingDraft has records.");
            }

        }

        
        [Fact]
        public async void AutoBillingDraft_Get_By_PersonId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

                AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

                var autobillingdraft = await AutoBillingDraftService.CreateAutoBillingDraft(model);

                var newAutoBillingDraft = await AutoBillingDraftService.GetAutobillingDraftsByPersonId((int)autobillingdraft.EntityId);

                Assert.True(newAutoBillingDraft.Where(x => x.EntityId == model.EntityId).Count() > 0, "AutoBillingDraft returns person Id.");
            }

        }

        //[Fact]
        //public async void AutoBillingDraft_Get_By_PaymentTransactionId()
        //{
        //    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        //    using (var scope = serviceScopeFactory.CreateScope())
        //    {
        //        var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

        //        AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

        //        var autobillingdraft = await AutoBillingDraftService.CreateAutoBillingDraft(model);

        //        var newAutoBillingDraft = await AutoBillingDraftService.GetAutobillingDraftByPaymentTransactionId((int)autobillingdraft.PaymentTransactionId);

        //        Assert.True(newAutoBillingDraft.PaymentTransactionId == model.PaymentTransactionId, "AutoBillingDraft returns payment transaction Id.");
        //    }

        //}

        [Fact]
        public async void AutoBillingDraft_Get_By_ProcessStatus()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

                AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

                var autobillingdraft = await AutoBillingDraftService.CreateAutoBillingDraft(model);

                var newAutoBillingDraft = await AutoBillingDraftService.GetAutobillingDraftsByProcessStatus((int)model.IsProcessed);

                Assert.True(newAutoBillingDraft.Where(x=>x.IsProcessed == model.IsProcessed).Count() > 0, "AutoBillingDraft Found by Prosess Status.");

             }

        }

        [Fact]
        public async void AutoBillingDraft_Get_By_BillingDocumentId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();

                AutoBillingDraftModel model = TestDataGenerator.GetAutoBillingDraftModel();

                var autobillingdraft = await AutoBillingDraftService.CreateAutoBillingDraft(model);

                var newAutoBillingDraft = await AutoBillingDraftService.GetAutobillingDraftsByBillingDocumentId((int)autobillingdraft.BillingDocumentId);

                Assert.True(newAutoBillingDraft.Where(x => x.BillingDocumentId == model.BillingDocumentId).Count() > 0, "AutoBillingDraft returns Billing Document Id.");
            }

        }

    }
}