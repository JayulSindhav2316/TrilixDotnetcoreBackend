using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace Max.Services.Tests
{
    public class GlaccountTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public GlaccountTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateGlaccount_Add_Newl()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var glAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                var GlAccount = await glAccountService.CreateGlAccount(model);
                Assert.True(GlAccount.GlAccountId > 0, "Glaccount Created.");
            }

        }

        [Fact]
        public async void Glaccount_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel department = TestDataGenerator.GetDepartmentModel();
                var newdepartment = await DepartmentService.CreateDepartment(department);


                var GlAccountTypeService = scope.ServiceProvider.GetService<IGlAccountTypeService>();
                GlAccountTypeModel glaccounttype = TestDataGenerator.GetGlAccountTypeModel();
                var glAccountType = await GlAccountTypeService.CreateGlAccountType(glaccounttype);


                var glAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                await glAccountService.CreateGlAccount(model);

                //Add another

                model = TestDataGenerator.GetGlAccountModel();

                await glAccountService.CreateGlAccount(model);

                var GlAccounts = await glAccountService.GetAllGlaccounts();

                Assert.True(!Extenstions.IsNullOrEmpty(GlAccounts), "Glaccount has records.");
            }

        }

        [Fact]
        public async void Glaccount_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var glAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                var GlAccount = await glAccountService.CreateGlAccount(model);

                var returnedGlaccount = await glAccountService.GetGlAccountById(GlAccount.GlAccountId);

                Assert.True(returnedGlaccount.GlAccountId == returnedGlaccount.GlAccountId, "Glaccount returns selected Id.");
            }

        }

        [Fact]
        public async void Glaccount_GetSelectList()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var glAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                await glAccountService.CreateGlAccount(model);

                //Add another

                model = TestDataGenerator.GetGlAccountModel();

                await glAccountService.CreateGlAccount(model);

                var selectList = await glAccountService.GetSelectList();

                Assert.True(!Extenstions.IsNullOrEmpty(selectList), "Glaccount returns select List.");
            }

        }


    }
}