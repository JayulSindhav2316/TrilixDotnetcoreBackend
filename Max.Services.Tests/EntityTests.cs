using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Max.Services.Tests
{
    public class EntityTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public EntityTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateEntity_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var EntityService = scope.ServiceProvider.GetService<IEntityService>();

                EntityModel model = TestDataGenerator.GetEntityModel();

                var entity = await EntityService.CreateEntity(model) ;
                Assert.True(entity.EntityId > 0, "Entity Created.");
            }

        }

        [Fact]
        public async void Entity_GetCompanyProfile_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);


                var CompanyService = scope.ServiceProvider.GetService<ICompanyService>();
                CompanyModel company = TestDataGenerator.GetCompanyModel();
                var newCompany = await CompanyService.CreateCompany(company);

                var EntityService = scope.ServiceProvider.GetService<IEntityService>();

                var returnedEntity = await EntityService.GetEntityProfileById(newCompany.EntityId);

                Assert.True (returnedEntity.EntityId.Equals(newCompany.EntityId));
            }

        }


        [Fact]
        public async void Entity_GetPersonProfile_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                var newPerson = await PersonService.CreatePerson(Person);


                var EntityService = scope.ServiceProvider.GetService<IEntityService>();

                var returnedEntity = await EntityService.GetEntityProfileById((int)newPerson.EntityId);

                Assert.True(returnedEntity.EntityId.Equals(newPerson.EntityId));
            }

        }


        [Fact]
        public async void Entity_GetBy_Name()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);


                var EntityService = scope.ServiceProvider.GetService<IEntityService>();

                EntityModel model = TestDataGenerator.GetEntityModel();

                var entity = await EntityService.CreateEntity(model);

                var returnedEntity = await EntityService.GetEntitiesByName(entity.Name);

                Assert.True(returnedEntity.Where(x => x.Name == entity.Name).Count() > 0, "Entity record returns selected name");
            }

        }

        [Fact]
        public async void Entity_GetBy_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);


                var EntityService = scope.ServiceProvider.GetService<IEntityService>();

                EntityModel model = TestDataGenerator.GetEntityModel();

                var entity = await EntityService.CreateEntity (model);

                var returnedEntity = await EntityService.GetEntityById (entity.EntityId);

                Assert.True(returnedEntity.EntityId == entity.EntityId, "Entity returns selected Id.");
            }

        }     


    }
}