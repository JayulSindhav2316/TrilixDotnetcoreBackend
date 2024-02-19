using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Max.Core;
using System;

namespace Max.Services.Tests
{
    public class MembershipTypeTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public MembershipTypeTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateMembershipType_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();

                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();

                var newMembershipType = await membershipTypeService.CreateMembershipType(model);
                Assert.True(newMembershipType.MembershipTypeId > 0, "MembershipType Created.");
            }

        }

        [Fact]
        public async void MembershipType_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(model);

                //model.Period = period.MembershipPeriodId;


                var selectedrecord = await membershipTypeService.GetMembershipTypeById(newMembershipType.MembershipTypeId);

                Assert.True(selectedrecord.MembershipTypeId == newMembershipType.MembershipTypeId, "MembershipType returns selected Id.");
            }

        }

        [Theory]
        [InlineData(null, "testtypecode")] // null Membershiptype name
        [InlineData("", "testtypecode")]   // Blank Membershiptype name   
        [InlineData("testtypename", null)] // null Membershiptype code
        [InlineData("testtypename", "")] // Blank Membershiptype code  
        public async void MembershipType_ValidatenullandBlank(string typename, string typecode)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeValidationModel(typename, typecode);                

                var param = String.IsNullOrWhiteSpace(typename) ? "Name" : "Code";


                if (typename == null || typecode == null)
                {                    
                    var ex = await Assert.ThrowsAsync<NullReferenceException>(() => membershipTypeService.CreateMembershipType(model));
                    Assert.Contains("MembershipType " + param + " can not be NULL.", ex.Message);
                }
                else 
                {
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => membershipTypeService.CreateMembershipType(model));
                    Assert.Contains("MembershipType " + param + " can not be empty.", ex.Message);
                }               
                           
            }

        }

       [Fact]
        public async void MembershipType_Validate_duplicateName()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();
                var newmembershiptype = await membershipTypeService.CreateMembershipType(model);

                //Add Another with same name
                MembershipTypeModel duplicatemodel = new MembershipTypeModel();
                duplicatemodel.Name = newmembershiptype.Name;
                duplicatemodel.Code = "new membership type code";
                duplicatemodel.Category = newmembershiptype.Category;

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => membershipTypeService.CreateMembershipType(duplicatemodel));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("Duplicate Membership Type Name.", ex.Message);

            }

        }

        [Fact]
        public async void MembershipType_Validate_duplicateCode()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();
                var newmembershiptype = await membershipTypeService.CreateMembershipType(model);

                //Add Another with same name
                MembershipTypeModel duplicatemodel = new MembershipTypeModel();
                duplicatemodel.Name = "new membership type name";
                duplicatemodel.Code = newmembershiptype.Code; 

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => membershipTypeService.CreateMembershipType(duplicatemodel));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("A Membership Type already exists with Code", ex.Message);

            }

        }

        [Fact]
        public async void MembershipType_Get_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();
                MembershipCategoryModel categorymodel = TestDataGenerator.GetMembershipCategoryModel();
                var newcategory1 = await membershipCategoryService.CreateMembershipCategory(categorymodel);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType1 = await membershipTypeService.CreateMembershipType(model);
                

                //Add Another
                model = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType2 = await membershipTypeService.CreateMembershipType(model);
                var selectedrecords = await membershipTypeService.GetAllMembershipTypes();

                Assert.True(!Extenstions.IsNullOrEmpty(selectedrecords), "MembershipType has records.");
            }

        }


        [Fact]
        public async void CreateMembershipType_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();

                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();

                var newMembershipType = await membershipTypeService.CreateMembershipType(model);
                model.Name = "Changed";
                model.MembershipTypeId = newMembershipType.MembershipTypeId;

                var updatedMembershipType = await membershipTypeService.UpdateMembershipType(model);
                Assert.True(updatedMembershipType.Name == "Changed", "MembershipType Updated.");
            }

        }

        [Fact]
        public async void CreateMembershipType_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();

                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();

                var newMembershipType = await membershipTypeService.CreateMembershipType(model);

                await membershipTypeService.DeleteMembershipType(newMembershipType.MembershipTypeId);
                var deletedMembershipType = await membershipTypeService.GetMembershipTypeById(newMembershipType.MembershipTypeId);
                Assert.True(deletedMembershipType.MembershipTypeId == 0, "MembershipType Deleted.");
            }

        }


        [Fact]
        public async void MembershipType_Get_By_ByCategoryIds()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipCategoryService = scope.ServiceProvider.GetService<IMembershipCategoryService>();
                MembershipCategoryModel categorymodel = TestDataGenerator.GetMembershipCategoryModel();
                var newcategory1 = await membershipCategoryService.CreateMembershipCategory(categorymodel);


                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel model = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType1 = await membershipTypeService.CreateMembershipType(model);

                //Add Another
                model = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType2 = await membershipTypeService.CreateMembershipType(model);

                var selectedrecords = await membershipTypeService.GetMembershipTypesByCategoryIds(newcategory1.MembershipCategoryId.ToString());

                Assert.True(!Extenstions.IsNullOrEmpty(selectedrecords), "MembershipType has records.");
            }

        }

    }
}