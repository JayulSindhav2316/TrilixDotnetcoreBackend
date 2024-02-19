using System;
using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;


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
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();


                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var newStaff = await staffUserService.CreateStaffUser(staff);
                Assert.True(newStaff.UserId > 0, "Staff User Created.");
            }

        }

        [Fact]
        public async void GetStaffUser_Get_ById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);
                
                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newStaff = await staffUserService.CreateStaffUser(staff);
               
                var selectedrecord = await staffUserService.GetStaffUserById(newStaff.UserId);
                Assert.True(selectedrecord.UserId == newStaff.UserId, "Staff User selected.");
            }
        }


        [Theory]
        [InlineData(null, "staffemail@testing.com")] // null name
        [InlineData("", "staffemail@testing.com")]   // Blank name   
        [InlineData("StaffName", null)] // null Email
        [InlineData("StaffName", "")]  // Blank name   
        public async void StaffUser_Get_Validate(string staffusername, string staffuseremail)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserValidationModel(staffusername, staffuseremail);

                var param = String.IsNullOrWhiteSpace(staffusername) ? "User Name" : "Email";

                if (staffusername == null || staffuseremail == null)
                {
                    var ex = await Assert.ThrowsAsync<NullReferenceException>(() => staffUserService.CreateStaffUser(staff));
                    Assert.Contains("" + param + " can not be NULL.", ex.Message);
                }
                else
                {
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.CreateStaffUser(staff));
                    Assert.Contains("" + param + " can not be empty.", ex.Message);
                }

            }

        }

        [Fact]
        public async void GetStaffUser_Get_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel departmentmodel = TestDataGenerator.GetDepartmentModel();
                var newdepartment = await DepartmentService.CreateDepartment(departmentmodel);

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                staff.DepartmentId = newdepartment.DepartmentId;
                await staffUserService.CreateStaffUser(staff);

                //Add another
                staff = TestDataGenerator.GetStaffUserModel();
                staff.DepartmentId = newdepartment.DepartmentId;
                await staffUserService.CreateStaffUser(staff);

                var staffusers = staffUserService.GetAllStaffUsers();

                Assert.True(staffusers.Result.Count == 2, "staffusers has records.");
            }

        }

        [Fact]
        public async void StaffUser_Validate_duplicateUserName()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newstaffuser = await staffUserService.CreateStaffUser(staff);

                //Add Another with same name
                StaffUserModel duplicatemodel = new StaffUserModel();
                duplicatemodel.UserName = newstaffuser.UserName;

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.CreateStaffUser(duplicatemodel));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("Duplicate User Name.", ex.Message);

            }

        }

        [Fact]
        public async void StaffUser_Updateloginstatus()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var addedStaff = await staffUserService.CreateStaffUser(staff);

                addedStaff.FailedAttempts = 5;
                addedStaff.Status = 0;

                await staffUserService.UpdateLoginStatus(addedStaff.UserId, 0);

                var selectedrecord = await staffUserService.GetStaffUserById(addedStaff.UserId);

                Assert.True(selectedrecord.Locked == 1, "login Status Changed.");

            }

        }

        [Fact]
        public async void CreateStaffUser_Add_Existing_User_Name_ThrowException()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

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

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

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

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

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

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

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
        public async void UpdateStaffUser_WithExistingPassword_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);



                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var addedStaff = await staffUserService.CreateStaffUser(staff);

                staff.UserId = addedStaff.UserId;

                //Change FirstName Last Namme
                staff.FirstName = "Changed First Name";
                staff.LastName = "Changed Last Name";
                staff.Password = staff.Password;                             

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.UpdateStaffUser(staff));             

                Assert.IsType<InvalidOperationException>(ex);

                Assert.Equal("New password cannot be same as existing password.", ex.Message);               

            }

        }

        [Fact]
        public async void UpdateStaffUser_WithNewPassword_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);



                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var addedStaff = await staffUserService.CreateStaffUser(staff);

                staff.UserId = addedStaff.UserId;

                //Change FirstName Last Namme
                staff.FirstName = "Changed First Name";
                staff.LastName = "Changed Last Name";
                staff.Password = "newpass123";

                await staffUserService.UpdateStaffUser(staff);

                var updatedStaff = await staffUserService.GetStaffUserById(addedStaff.UserId);                

                Assert.True(updatedStaff.FirstName == "Changed First Name", "Staff User Updated.");
                Assert.True(updatedStaff.LastName == "Changed Last Name", "Staff User Updated.");
                Assert.True(updatedStaff.Password == addedStaff.Password, "Password Updated.");
                Assert.True(updatedStaff.Salt == addedStaff.Salt, "Password Updated.");               

            }

        }


        [Fact]
        public async void DeleteStaffUser_withoutassociatedreceipts_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);             


                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var addedStaff = await staffUserService.CreateStaffUser(staff);

                await staffUserService.DeleteStaffUser(addedStaff.UserId);

                var deletedStaff = await staffUserService.GetStaffUserById(addedStaff.UserId);

                Assert.True(deletedStaff == null, "Staff User Deleted.");

            }

        }


        [Fact]
        public async void DeleteStaffUser_withassociatedreceipts_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();                


                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();
                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var ex=await Assert.ThrowsAsync<InvalidOperationException>(() => staffUserService.DeleteStaffUser((int)receiptheader.StaffId));
                             
                Assert.IsType<InvalidOperationException>(ex);            

                Assert.Equal("User can not be deleted as there are linked transactions. You can make him InActive.", ex.Message);
               
            }

        }
    }


}
