using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Max.Core;
using System.Linq;
namespace Max.Services.Tests
{
    public class GroupRegistrationTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;
        public GroupRegistrationTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }
        [Fact]
        public async void GroupRegistration_Register_Group()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var registrationService = scope.ServiceProvider.GetService<IGroupRegistrationService>();
                var model = TestDataGenerator.GetGroupRegistrationModel();
                var res = await registrationService.RegisterGroup(model);
                Assert.True(res);
            }
        }
        [Fact]
        public async void GroupRegistration_Update_Group()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var registrationService = scope.ServiceProvider.GetService<IGroupRegistrationService>();
                var model = TestDataGenerator.GetGroupRegistrationModel();
                var res = await registrationService.RegisterGroup(model);
                model.Name = "Updated";
                model.RegistrationGroupId = 1;
                var updateRes=await registrationService.UpdateGroup(model);
                Assert.True(updateRes);
            }
        }
        [Fact]
        public async void GroupRegistration_Get_Active_Register_Groups()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var registrationService = scope.ServiceProvider.GetService<IGroupRegistrationService>();
                var model = TestDataGenerator.GetGroupRegistrationModel();
                await registrationService.RegisterGroup(model);
                model.Status = 0;
                await registrationService.RegisterGroup(model);
                var res = await registrationService.GetRegisterGroups("Active");
                Assert.True(res.Count() != 0);
                Assert.True(res.All(x => x.Status == 1));
            }
        }
        [Fact]
        public async void GroupRegistration_Get_All_Register_Groups()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var registrationService = scope.ServiceProvider.GetService<IGroupRegistrationService>();
                var model = TestDataGenerator.GetGroupRegistrationModel();
                await registrationService.RegisterGroup(model);
                model.Status = 0;
                await registrationService.RegisterGroup(model);
                var res = await registrationService.GetRegisterGroups(null);
                Assert.True(res.Count() != 0);
            }
        }
        [Fact]
        public async void GroupRegistration_Delete_Group()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var registrationService = scope.ServiceProvider.GetService<IGroupRegistrationService>();
                var model = TestDataGenerator.GetGroupRegistrationModel();
                await registrationService.RegisterGroup(model);
                var res = await registrationService.DeleteGroup(1);
                Assert.True(res);
            }
        }
        [Fact]
        public async void GroupRegistration_Link_Membership()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var registrationService = scope.ServiceProvider.GetService<IGroupRegistrationService>();
                var model = TestDataGenerator.GetGroupRegistrationModel();
                var linkModel = TestDataGenerator.GetLinkMembershipModel();
                var res = await registrationService.RegisterGroup(model);
                model.RegistrationGroupId = 1;
                model.MembershipTypeIds.Add(1);
                var linkRes = await registrationService.LinkMembership(model);
                Assert.True(linkRes);
            }
        }
        [Fact]
        public async void GroupRegistration_Delete_Linked_Membership()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var registrationService = scope.ServiceProvider.GetService<IGroupRegistrationService>();
                var model = TestDataGenerator.GetGroupRegistrationModel();
                var linkModel = TestDataGenerator.GetLinkMembershipModel();
                var res = await registrationService.RegisterGroup(model);
                model.RegistrationGroupId = 1;
                model.MembershipTypeIds.Add(1);
                var linkRes = await registrationService.LinkMembership(model);
                var deleteLinkRes = await registrationService.DeleteLink(1);
                Assert.True(deleteLinkRes);
            }
        }
    }
}