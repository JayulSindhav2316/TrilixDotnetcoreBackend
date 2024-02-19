using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Max.Services.Tests
{
    public class OrganizationTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public OrganizationTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }
        
        [Fact]
        public async void CreateOrganization_Add_Newl()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();

                OrganizationModel model = TestDataGenerator.GetOrganizationModel();

                var organization = await OrganizationService.CreateOrganization(model);

                Assert.True(organization.OrganizationId > 0, "Organization Created.");
            }

        }

        [Fact]
        public async void Organization_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();

                OrganizationModel model = TestDataGenerator.GetOrganizationModel();

                await OrganizationService.CreateOrganization(model);

                //Add another

                model = TestDataGenerator.GetOrganizationModel();

                await OrganizationService.CreateOrganization(model);

                var organizations = await OrganizationService.GetAllOrganizations();

                Assert.True(!Extenstions.IsNullOrEmpty(organizations), "Organization has records.");
            }

        }

        [Fact]
        public async void Organization_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();

                OrganizationModel model = TestDataGenerator.GetOrganizationModel();

                var organization = await OrganizationService.CreateOrganization(model);

                var returnedOrganization = await OrganizationService.GetOrganizationById(organization.OrganizationId);

                Assert.True(returnedOrganization.OrganizationId == organization.OrganizationId, "Organization returns selected Id.");
            }

        }

        [Fact]
        public async void Organization_GetSelectList()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();

                OrganizationModel model = TestDataGenerator.GetOrganizationModel();

                await OrganizationService.CreateOrganization(model);

                //Add another

                model = TestDataGenerator.GetOrganizationModel();

                await OrganizationService.CreateOrganization(model);

                var selectList = await OrganizationService.GetSelectList();

                Assert.True(!Extenstions.IsNullOrEmpty(selectList), "Organization returns select List.");
            }

        }


    }
}
