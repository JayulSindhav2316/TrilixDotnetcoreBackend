using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Max.Services.Tests
{
    public class RoleTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public RoleTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateRole_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();

                RoleModel role = TestDataGenerator.GetRoleModel();

                var newRole = await roleService.CreateRole(role);
                Assert.True(newRole.RoleId > 0, "Role Created.");
            }

        }

        [Theory]
        [InlineData("")] // null Role Name         
        [InlineData(null)] // Blank Role Name  
        [InlineData("DuplicateName")] //Duplicate Name
        public async void CreateRole_Validate_Validate(string rolename)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                RoleModel role = TestDataGenerator.GetRoleModel();
                role.Name = rolename;
                if (rolename == null)
                {
                    var ex = await Assert.ThrowsAsync<NullReferenceException>(() => roleService.CreateRole(role));
                    Assert.Contains("Role Name can not be NULL.", ex.Message);
                }
                else if (rolename == "")
                {
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => roleService.CreateRole(role));
                    Assert.Contains("Role Name can not be empty.", ex.Message);
                }
                else //(duplicate Name)
                {
                    await roleService.CreateRole(role);
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => roleService.CreateRole(role));
                    Assert.Contains("Duplicate Name.", ex.Message);
                }
            }

        }

        [Fact]
        public async void GetRole_Get_ById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                RoleModel role = TestDataGenerator.GetRoleModel();
                var newRole = await roleService.CreateRole(role);

                var selectedrecord = await roleService.GetRoleById(newRole.RoleId);
                Assert.True(selectedrecord.RoleId == newRole.RoleId, "selected Id returns.");

            }

        }

        [Fact]
        public async void GetRole_Get_GetallRoles()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();
                RoleModel role = TestDataGenerator.GetRoleModel();
                await roleService.CreateRole(role);

                //Add another
                role = TestDataGenerator.GetRoleModel();
                await roleService.CreateRole(role);

                var selectedrecords = await roleService.GetAllRoles();
                Assert.True(selectedrecords.Count >= 2, "Selecetd all records.");

            }

        }


        [Fact]
        public async void UpdateRole_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetService<IRoleService>();

                RoleModel role = TestDataGenerator.GetRoleModel();

                var newRole = await roleService.CreateRole(role);

                role.RoleId = newRole.RoleId;
                role.Name = "Changed Name";

                await roleService.UpdateRole(role);

                var updatedRole = await roleService.GetRoleById(role.RoleId);

                Assert.True(updatedRole.Name == "Changed Name", "Role Updated.");
            }

        }

        [Fact]
        public async void DeleteRole_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var roleService = scope.ServiceProvider.GetService<IRoleService>();

                RoleModel role = TestDataGenerator.GetRoleModel();

                var newRole = await roleService.CreateRole(role);

                await roleService.DeleteRole(newRole.RoleId);

                var deletedRole = await roleService.GetRoleById(newRole.RoleId);

                Assert.True(deletedRole == null, "Role Deleted.");

            }

        }
    }
}