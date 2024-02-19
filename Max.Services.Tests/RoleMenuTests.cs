using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class RoleMenuTests : IClassFixture<DependencySetupFixture>
    {

        private ServiceProvider _serviceProvider;

        public RoleMenuTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact(Skip = "Test not being Invoked. Need a fix")]
        public async void CreateRoleMenu_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var roleMenuService = scope.ServiceProvider.GetService<IRoleMenuService>();

                dynamic requestObject = new System.Dynamic.ExpandoObject();

                requestObject.roleId = 1;
                requestObject.selectedMenuIDs = "1,2,3";
                

                var result = await roleMenuService.UpdateRoleMenubyRoleId(requestObject);
                Assert.True(result == true, "Role Menu Created.");
            }

        }

    }
}
