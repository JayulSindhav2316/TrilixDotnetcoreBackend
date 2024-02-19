using System;
using Xunit;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Data;
using Max.Services.Interfaces;
using Max.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;


namespace Max.Services.Tests
{
    public class StaffUserTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public StaffUserTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateStaffUser_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var newStaff = await staffUserService.CreateStaffUser(staff);
                Assert.True(newStaff.UserId > 0, "Staff User Created.");
            }

        }

        [Fact]
        public async void CreateStaffUser_Add_Existing_User_Name_ThrowException()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var newStaff = await staffUserService.CreateStaffUser(staff);

                // Add again. Should throw exception

                await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.CreateStaffUser(staff));

            }

        }

        [Fact]
        public async void CreateStaffUser_Blank_UserName_ThrowException()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                staff.UserName = String.Empty;

                await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.CreateStaffUser(staff));

            }

        }

        [Fact]
        public async void CreateStaffUser_Null_UserName_ThrowException()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                staff.UserName = null;

                await Assert.ThrowsAsync<NullReferenceException>(() => staffUserService.CreateStaffUser(staff));

            }

        }

        [Fact]
        public async void CreateStaffUser_Null_Email_ThrowException()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                staff.Email = String.Empty;

                await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.CreateStaffUser(staff));

            }

        }

        [Fact]
        public async void CreateStaffUser_Blank_Email_ThrowException()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                staff.Email = null;

                await Assert.ThrowsAsync<NullReferenceException>(() => staffUserService.CreateStaffUser(staff));

            }

        }

        [Fact]
        public async void CreateStaffUser_Existing_Email_ThrowException()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            string email = "test@emmail.com";

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                staff.Email = email;

                var newStaff = await staffUserService.CreateStaffUser(staff);

                staff = TestDataGenerator.GetStaffUserModel();
                staff.Email = email;

                // Add again. Should throw exception

                await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.CreateStaffUser(staff));

            }

        }

         [Fact]
        public async void UpdateStaffUser_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var addedStaff = await staffUserService.CreateStaffUser(staff);

                //Change FirstName Last Namme
                addedStaff.FirstName = "Changed First Name";
                addedStaff.LastName = "Changed Last Name";

                await  staffUserService.UpdateStaffUser(addedStaff);

                var updatedStaff = await  staffUserService.GetStaffUserById(addedStaff.UserId);

                Assert.True(updatedStaff.FirstName == "Changed First Name", "Staff User Updated.");
                Assert.True(updatedStaff.LastName == "Changed Last Name", "Staff User Updated.");

            }

        }
    }


}
