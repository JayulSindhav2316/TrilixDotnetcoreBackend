using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Max.Core;

namespace Max.Services.Tests
{
    public class StaffRoleTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public StaffRoleTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateStaffRole_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                RoleModel role = TestDataGenerator.GetRoleModel();
                var newRole = await roleService.CreateRole(role);

                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organizationmodel = TestDataGenerator.GetOrganizationModel();
                await OrganizationService.CreateOrganization(organizationmodel);

                var staffService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newStaff = await  staffService.CreateStaffUser(staff);

                var staffRoleService = scope.ServiceProvider.GetService<IStaffRoleService>();
                var newStaffRole = await staffRoleService.CreateStaffRole(newStaff.UserId, newRole.RoleId);
                Assert.True(newStaffRole.StaffRoleId > 0, "StaffRole Created.");
            }

        }

        [Fact]
        public async void DeleteStaffRole_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var staffRoleService = scope.ServiceProvider.GetService<IStaffRoleService>();
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                var staffService = scope.ServiceProvider.GetService<IStaffUserService>();


                RoleModel role = TestDataGenerator.GetRoleModel();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                var newStaffRole = await staffRoleService.CreateStaffRole(staff.UserId, role.RoleId);

                await staffRoleService.DeleteStaffRole(newStaffRole.StaffRoleId);

                var deletedStaffRole = await staffRoleService.GetStaffRoleById(newStaffRole.StaffRoleId);

                Assert.True(deletedStaffRole == null, "Staff Role Deleted.");

            }

        }

        [Fact]
        public async void StaffRole_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                RoleModel role = TestDataGenerator.GetRoleModel();
                var newRole = await roleService.CreateRole(role);

                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organizationmodel = TestDataGenerator.GetOrganizationModel();
                await OrganizationService.CreateOrganization(organizationmodel);

                var staffService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newStaff = await staffService.CreateStaffUser(staff);

                var staffRoleService = scope.ServiceProvider.GetService<IStaffRoleService>();
                await staffRoleService.CreateStaffRole(newStaff.UserId, newRole.RoleId);

                var staffRole = await staffRoleService.GetAllStaffRoles();

                Assert.True(!Extenstions.IsNullOrEmpty(staffRole), "StaffRole has records.");
            }

        }

        [Fact]
        public async void StaffRole_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                RoleModel role = TestDataGenerator.GetRoleModel();
                var newRole = await roleService.CreateRole(role);

                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organizationmodel = TestDataGenerator.GetOrganizationModel();
                await OrganizationService.CreateOrganization(organizationmodel);

                var staffService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newStaff = await staffService.CreateStaffUser(staff);

                var staffRoleService = scope.ServiceProvider.GetService<IStaffRoleService>();
                var staffRole = await staffRoleService.CreateStaffRole(newStaff.UserId, newRole.RoleId);

                var newstaffRole = await staffRoleService.GetStaffRoleById(newRole.RoleId);

                Assert.True(newstaffRole.RoleId == staffRole.RoleId, "StaffRole returns selected Id.");
            }

        }

        [Fact]
        public async void StaffRole_Get_By_StaffId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                RoleModel role = TestDataGenerator.GetRoleModel();
                var newRole = await roleService.CreateRole(role);

                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organizationmodel = TestDataGenerator.GetOrganizationModel();
                await OrganizationService.CreateOrganization(organizationmodel);

                var staffService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newStaff = await staffService.CreateStaffUser(staff);

                var staffRoleService = scope.ServiceProvider.GetService<IStaffRoleService>();
                var staffRole = await staffRoleService.CreateStaffRole(newStaff.UserId, newRole.RoleId);

                var newstaffRole = await staffRoleService.GetStaffRoleByStaffId(newStaff.UserId);

                Assert.True(!Extenstions.IsNullOrEmpty(newstaffRole), "StaffRole by StaffId has records.");
            }

        }
    }
}