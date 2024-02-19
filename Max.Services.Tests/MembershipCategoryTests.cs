using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace Max.Services.Tests
{
    public class MembershipCategoryTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public MembershipCategoryTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateMembershipCategory_Add_Newl()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();

                MembershipCategoryModel model = TestDataGenerator.GetMembershipCategoryModel();

                var category = await membershipCategoryService.CreateMembershipCategory(model);
                Assert.True(category.MembershipCategoryId > 0, "Membership Category Created.");
            }

        }

        [Fact]
        public async void MembershipCategory_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();

                MembershipCategoryModel model = TestDataGenerator.GetMembershipCategoryModel();

                await membershipCategoryService.CreateMembershipCategory(model);

                //Add another

                model = TestDataGenerator.GetMembershipCategoryModel();

                await membershipCategoryService.CreateMembershipCategory(model);

                var categories = await membershipCategoryService.GetAllMembershipCategories();

                Assert.True(!Extenstions.IsNullOrEmpty(categories), "Membership Category has records.");
            }

        }

        [Fact]
        public async void MembershipCategory_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();

                MembershipCategoryModel model = TestDataGenerator.GetMembershipCategoryModel();

                var category = await membershipCategoryService.CreateMembershipCategory(model);

                var returnedCategory = await membershipCategoryService.GetMembershipCategoryById(category.MembershipCategoryId);

                Assert.True(returnedCategory.MembershipCategoryId == category.MembershipCategoryId, "Membership Category returns selected Id.");
            }

        }

        [Fact]
        public async void MembershipCategory_GetSelectList()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();

                MembershipCategoryModel model = TestDataGenerator.GetMembershipCategoryModel();

                await membershipCategoryService.CreateMembershipCategory(model);

                //Add another

                model = TestDataGenerator.GetMembershipCategoryModel();

                await membershipCategoryService.CreateMembershipCategory(model);

                var selectList = await membershipCategoryService.GetSelectList();

                Assert.True(!Extenstions.IsNullOrEmpty(selectList), "Membership Category returns select List.");
            }

        }


    }
}