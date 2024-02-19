using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{
    public class PaymentProfileTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public PaymentProfileTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void PaymentProfile_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentProfileService = scope.ServiceProvider.GetService<IPaymentProfileService>();

                PaymentProfileModel model = TestDataGenerator.GetPaymentProfileModel();

                await PaymentProfileService.CreatePaymentProfile(model);

                //Add another

                model = TestDataGenerator.GetPaymentProfileModel();

                await PaymentProfileService.CreatePaymentProfile(model);

                var paymentprofile = await PaymentProfileService.GetAllPaymentProfiles();

                Assert.True(!Extenstions.IsNullOrEmpty(paymentprofile), "PaymentProfile has records.");
            }

        }

        [Fact]
        public async void PaymentProfile_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentProfileService = scope.ServiceProvider.GetService<IPaymentProfileService>();

                PaymentProfileModel model = TestDataGenerator.GetPaymentProfileModel();

                var paymentprofile = await PaymentProfileService.CreatePaymentProfile(model);

                var newpaymentprofile = await PaymentProfileService.GetPaymentProfileById(paymentprofile.PaymentProfileId);

                //Assert.True(newpaymentprofile.ProfileId == paymentprofile.PaymentProfileId, "PaymentProfile returns selected Id.");
            }

        }

        [Fact]
        public async void PaymentProfile_Get_By_PersonId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
             
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentProfileService = scope.ServiceProvider.GetService<IPaymentProfileService>();

                PaymentProfileModel model = TestDataGenerator.GetPaymentProfileModel();

                var paymentprofile = await PaymentProfileService.CreatePaymentProfile(model);

                var newpaymentprofile = await PaymentProfileService.GetPaymentProfileByEntityId(paymentprofile.EntityId);

                Assert.True(newpaymentprofile.Where(x => x.EntityId == model.EntityId).Count() > 0, "PaymentProfile returns selected person Id.");
            }

        }

        [Fact]
        public async void CreatePaymentProfile_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PaymentProfileService = scope.ServiceProvider.GetService<IPaymentProfileService>();

                PaymentProfileModel model = TestDataGenerator.GetPaymentProfileModel();

                var paymentprofile = await PaymentProfileService.CreatePaymentProfile(model);
                Assert.True(paymentprofile.PaymentProfileId > 0, "PaymentProfile Created.");
            }

        }

        [Fact]
        public async void UpdatePaymentProfile_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organizationmodel = TestDataGenerator.GetOrganizationModel();
                var organizatio = await OrganizationService.CreateOrganization(organizationmodel);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel personmodel = TestDataGenerator.GetPersonModel();
                var person = await PersonService.CreatePerson(personmodel);

                var PaymentProfileService = scope.ServiceProvider.GetService<IPaymentProfileService>();
                PaymentProfileModel model = TestDataGenerator.GetPaymentProfileModel();
                var paymentprofile = await PaymentProfileService.CreatePaymentProfile(model);

                //model.PaymentProfileId = paymentprofile.PaymentProfileId;
                //model.AuthNetPaymentProfileId = "Changed AuthNet Payment Profile Id";

                await PaymentProfileService.UpdatePaymentProfile(model);

                var updatedpaymentprofile = await PaymentProfileService.GetPaymentProfileById(paymentprofile.PaymentProfileId);

                //Assert.True(updatedpaymentprofile.AuthNetPaymentProfileId == "Changed AuthNet Payment Profile Id", "PaymentProfile Updated.");

            }

        }

        [Fact]
        public async void DeletePaymentProfile_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var PaymentProfileService = scope.ServiceProvider.GetService<IPaymentProfileService>();
                PaymentProfileModel model = TestDataGenerator.GetPaymentProfileModel();
                var paymentprofile = await PaymentProfileService.CreatePaymentProfile(model);

                await PaymentProfileService.DeletePaymentProfile(paymentprofile.PaymentProfileId);

                var deletedPaymentProfile = await PaymentProfileService.GetPaymentProfileById(paymentprofile.PaymentProfileId);

                Assert.True(deletedPaymentProfile == null, "PaymentProfile Deleted.");

            }

        }

    }
}
