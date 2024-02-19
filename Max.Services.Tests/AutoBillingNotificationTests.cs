using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{
    public class AutoBillingNotificationTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public AutoBillingNotificationTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateAutoBillingNotification_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var autobillingnotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);
                Assert.True(autobillingnotification.AutoBillingNotificationId > 0, "AutoBillingNotification Created.");
            }

        }

        [Fact]
        public async void UpdateAutoBillingNotification_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var newAutobillingnotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                model.AutoBillingNotificationId = newAutobillingnotification.AutoBillingNotificationId;
                model.BillingType = "Changed Billing Type";

                await AutoBillingNotificationService.UpdateAutoBillingNotification(model);

                var updatedAutoBillingNotification = await AutoBillingNotificationService.GetAutoBillingNotificationById(model.AutoBillingNotificationId);

                Assert.True(updatedAutoBillingNotification.BillingType == "Changed Billing Type", "Auto Billing Notification Billing Type Updated.");
            }

        }

        [Fact]
        public async void AutoBillingNotification_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var autobillingnotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                var newAutobillingnotification = await AutoBillingNotificationService.GetAutoBillingNotificationById(autobillingnotification.AutoBillingNotificationId);

                Assert.True(newAutobillingnotification.AutoBillingNotificationId == autobillingnotification.AutoBillingNotificationId, "Auto Billing Notification returns selected Id.");
            }

        }

        [Fact]
        public async void AutoBillingNotification_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                //Add another

                model = TestDataGenerator.GetAutoBillingNotificationModel();

                await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                var autoBillingNotification = await AutoBillingNotificationService.GetAllAutoBillingNotifications();

                Assert.True(!Extenstions.IsNullOrEmpty(autoBillingNotification), "Auto Billing Notification has records.");
            }

        }


        [Fact]
        public async void AutoBillingNotification_Get_By_ABPDId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var autoBillingNotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                var newAutoBillingNotification = await AutoBillingNotificationService.GetAutoBillingNotificationByABPDId((int)autoBillingNotification.AbpdId);

                Assert.True(newAutoBillingNotification.AbpdId == model.AbpdId, "AutoBillingNotification returns ABPD Id.");
            }

        }

        [Fact]
        public async void AutoBillingNotifications_Get_By_BillingType()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var autoBillingNotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                var newAutoBillingNotification = await AutoBillingNotificationService.GetAutoBillingNotificationsByBillingType(autoBillingNotification.BillingType);

                Assert.True(newAutoBillingNotification.Where(x => x.BillingType == model.BillingType).Count() > 0, "AutoBillingNotification Found by Billing Type.");

            }

        }

        [Fact]
        public async void AutoBillingNotifications_Get_By_InvoiceType()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var autoBillingNotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                var newAutoBillingNotification = await AutoBillingNotificationService.GetAutoBillingNotificationsByInvoiceType(autoBillingNotification.InvoiceType);

                Assert.True(newAutoBillingNotification.Where(x => x.InvoiceType == model.InvoiceType).Count() > 0, "AutoBillingNotification Found by Invoice Type.");

            }

        }

        [Fact]
        public async void AutoBillingNotifications_Get_By_NotificationType()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var autoBillingNotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                var newAutoBillingNotification = await AutoBillingNotificationService.GetAutoBillingNotificationsByNotificationType(autoBillingNotification.NotificationType);

                Assert.True(newAutoBillingNotification.Where(x => x.NotificationType == model.NotificationType).Count() > 0, "AutoBillingNotification Found by Notification Type.");

            }

        }

        [Fact]
        public async void AutoBillingNotifications_Get_By_NotificationDate()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var AutoBillingNotificationService = scope.ServiceProvider.GetService<IAutoBillingNotificationService>();

                AutoBillingNotificationModel model = TestDataGenerator.GetAutoBillingNotificationModel();

                var autoBillingNotification = await AutoBillingNotificationService.CreateAutoBillingNotification(model);

                var newAutoBillingNotification = await AutoBillingNotificationService.GetAutoBillingNotificationsByNotificationDate((DateTime)autoBillingNotification.NotificationSentDate);

                Assert.True(newAutoBillingNotification.Where(x => x.NotificationSentDate == model.NotificationSentDate).Count() > 0, "AutoBillingNotification Found by Send Date.");

            }

        }

    }
}