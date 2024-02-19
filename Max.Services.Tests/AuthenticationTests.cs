using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Max.Services.Tests
{
    public class AuthenticationTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public AuthenticationTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void AuthenticateUser_Valid_User_Password()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var authService = scope.ServiceProvider.GetService<IAuthenticationService>();
                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                staff.Password = "Sunil@1234.,";
                staff.UserName = "adrianna";

                var newStaff = await staffUserService.CreateStaffUser(staff);

                AuthRequestModel request = new AuthRequestModel();
                request.UserName= "adrianna";
                request.Password = "Sunil@1234.,";

                var authResponse = await authService.Authenticate(request);
                Assert.True(authResponse.UserId > 0, "User Authenticated.");
            }

        }

        [Fact]
        public async void AuthenticateUser_InValid_Password()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var authService = scope.ServiceProvider.GetService<IAuthenticationService>();
                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();

                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();

                staff.Password = "ashok123";

                var newStaff = await staffUserService.CreateStaffUser(staff);

                AuthRequestModel request = new AuthRequestModel();
                request.UserName = newStaff.UserName;
                request.Password = "ashok";

                var authResponse = await authService.Authenticate(request);
                Assert.True(authResponse==null, "User Not Authenticated.");
            }

        }

        [Fact]
        public async void AuthenticateUser_InValid_UserName()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var authService = scope.ServiceProvider.GetService<IAuthenticationService>();
                var staffUserService = scope.ServiceProvider.GetService<IStaffUserService>();
                StaffUserModel staff = TestDataGenerator.GetStaffUserModel();
                var newStaff = await staffUserService.CreateStaffUser(staff);

                AuthRequestModel request = new AuthRequestModel();
                request.UserName = newStaff.UserName+"Changed";
                request.Password = newStaff.Password;

                var authResponse = await authService.Authenticate(request);
                Assert.True(authResponse == null, "User Not Authenticated.");
            }

        }

    }
}
