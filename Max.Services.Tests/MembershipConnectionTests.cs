using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class MembershipConnectionTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public MembershipConnectionTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateMembershipConnection_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var MembershipConnectionService = scope.ServiceProvider.GetService<IMembershipConnectionService>();
                MembershipConnectionModel model = TestDataGenerator.GetMembershipConnectionModel();
                var MembershipConnection = await MembershipConnectionService.CreateMembershipConnection(model);
                
                Assert.True(MembershipConnection.MembershipConnectionId>0 , "Report sorting record created.");
            }

        }
    }
}