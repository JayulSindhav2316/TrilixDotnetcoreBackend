using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Max.Services.Tests
{
    public class GlAccountTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider; 

        public GlAccountTests(DependencySetupFixture fixture) 
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateGlAccount_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                var glaccount = await GlAccountService.CreateGlAccount(model);

                Assert.True(glaccount.GlAccountId > 0, "GlAcoount Created.");
            }

        }

        [Fact]
        public async void GlAccount_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();
                var glaccount = await GlAccountService.CreateGlAccount(model);

                model.GlAccountId = glaccount.GlAccountId;
                model.Name = "Changed GL Name";               

                await GlAccountService.UpdateGlAccount(model);

                var updatedglaccount = await GlAccountService.GetGlAccountById(glaccount.GlAccountId);

                Assert.True(updatedglaccount.Name == "Changed GL Name", "GL Account updated.");              
               
            }

        }

        [Fact]
        public async void GlAccount_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();
                var glaccount = await GlAccountService.CreateGlAccount(model);                

                await GlAccountService.DeleteGlAccount(glaccount.GlAccountId);

                var deletetedglaccount = await GlAccountService.GetGlAccountById(glaccount.GlAccountId);

                Assert.True(deletetedglaccount == null, "GL Account Deleted.");
                
            }

        }

        [Fact]
        public async void GlAccount_Validate_BlankName()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel_withBlankName();
                //var glaccount = await GlAccountService.CreateGlAccount(model);

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => GlAccountService.CreateGlAccount(model));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("GL Chart Of Account Name can not be empty.", ex.Message);

            }

        }

        [Fact]
        public async void GlAccount_Validate_duplicateName()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();
                var glaccount = await GlAccountService.CreateGlAccount(model);

                //Add Another with same name
                model = TestDataGenerator.GetGlAccountModel_withduplicateNameorCode(glaccount.Name, "");

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => GlAccountService.CreateGlAccount(model));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("Duplicate GL Name.", ex.Message);

            }

        }

        [Fact]
        public async void GlAccount_Validate_duplicatecode()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();
                var glaccount = await GlAccountService.CreateGlAccount(model);

                //Add Another with same code
                model = TestDataGenerator.GetGlAccountModel_withduplicateNameorCode("", glaccount.Code);

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => GlAccountService.CreateGlAccount(model));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("A GL Account already exists with Code:", ex.Message);

            }

        }

        [Fact]
        public async void GlAccount_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel department = TestDataGenerator.GetDepartmentModel();
                var newdepartment= await DepartmentService.CreateDepartment(department);


                var GlAccountTypeService = scope.ServiceProvider.GetService<IGlAccountTypeService>();
                GlAccountTypeModel glaccounttype = TestDataGenerator.GetGlAccountTypeModel();
                var glAccountType = await GlAccountTypeService.CreateGlAccountType(glaccounttype);


                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                await GlAccountService.CreateGlAccount(model);
                

                //Add another

                model = TestDataGenerator.GetGlAccountModel();

                await GlAccountService.CreateGlAccount(model);

                var glaccount = await GlAccountService.GetAllGlaccounts();

                Assert.True(!Extenstions.IsNullOrEmpty(glaccount), "GlAccount has records.");
            }

        }

        [Fact]
        public async void GlAccount_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                var glaccount = await GlAccountService.CreateGlAccount(model);

                var newGlAccount = await GlAccountService.GetGlAccountById(glaccount.GlAccountId);

                Assert.True(newGlAccount.GlAccountId == glaccount.GlAccountId, "GlAccount returns selected Id.");
            }

        }

        [Fact]
        public async void GlAccount_GetSelectList()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var GlAccountService = scope.ServiceProvider.GetService<IGlAccountService>();

                GlAccountModel model = TestDataGenerator.GetGlAccountModel();

                await GlAccountService.CreateGlAccount(model);

                //Add another

                model = TestDataGenerator.GetGlAccountModel();

                await GlAccountService.CreateGlAccount(model);

                var selectList = await GlAccountService.GetSelectList();

                Assert.True(!Extenstions.IsNullOrEmpty(selectList), "GlAccount returns select List.");
            }

        }

    }
}
