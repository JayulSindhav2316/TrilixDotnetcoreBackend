using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace Max.Services.Tests
{
    public class GlAccountTypeTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public GlAccountTypeTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateGlAccountType_Add_Newl()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountTypeService = scope.ServiceProvider.GetService<IGlAccountTypeService>();

                GlAccountTypeModel model = TestDataGenerator.GetGlAccountTypeModel();

                var glAccountType = await GlAccountTypeService.CreateGlAccountType(model);
                Assert.True(glAccountType.AccountId > 0, "GlAccountType Created.");
            }

        }

        [Fact]
        public async void GlAccountType_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountTypeService = scope.ServiceProvider.GetService<IGlAccountTypeService>();

                GlAccountTypeModel model = TestDataGenerator.GetGlAccountTypeModel();

                await GlAccountTypeService.CreateGlAccountType(model);

                //Add another

                model = TestDataGenerator.GetGlAccountTypeModel();

                await GlAccountTypeService.CreateGlAccountType(model);

                var glAccountTypes = await GlAccountTypeService.GetAllGlAccountTypes();

                Assert.True(!Extenstions.IsNullOrEmpty(glAccountTypes), "GlAccountType has records.");
            }

        }

        [Fact]
        public async void GlAccountType_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountTypeService = scope.ServiceProvider.GetService<IGlAccountTypeService>();

                GlAccountTypeModel model = TestDataGenerator.GetGlAccountTypeModel();

                var glAccountType = await GlAccountTypeService.CreateGlAccountType(model);

                var returnedCategory = await GlAccountTypeService.GetGlAccountTypeById(glAccountType.AccountId);

                Assert.True(returnedCategory.AccountId == returnedCategory.AccountId, "GlAccountType returns selected Id.");
            }

        }

        [Fact]
        public async void GlAccountType_GetSelectList()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountTypeService = scope.ServiceProvider.GetService<IGlAccountTypeService>();

                GlAccountTypeModel model = TestDataGenerator.GetGlAccountTypeModel();

                await GlAccountTypeService.CreateGlAccountType(model);

                //Add another

                model = TestDataGenerator.GetGlAccountTypeModel();

                await GlAccountTypeService.CreateGlAccountType(model);

                var selectList = await GlAccountTypeService.GetSelectList();

                Assert.True(!Extenstions.IsNullOrEmpty(selectList), "GlAccountType returns select List.");
            }

        }


    }
}