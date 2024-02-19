using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace Max.Services.Tests
{
    public class MembershipPeriodTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public MembershipPeriodTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateMembershipPeriod_Add_Newl()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();

                MembershipPeriodModel model = TestDataGenerator.GetMembershipPeriodModel();

                var period = await membershipPeriodService.CreateMembershipPeriod(model);
                Assert.True(period.MembershipPeriodId > 0, "Membership Period Created.");
            }

        }

        [Fact]
        public async void MembershipPeriod_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();

                MembershipPeriodModel model = TestDataGenerator.GetMembershipPeriodModel();

                await membershipPeriodService.CreateMembershipPeriod(model);

                //Add another

                model = TestDataGenerator.GetMembershipPeriodModel();

                await membershipPeriodService.CreateMembershipPeriod(model);

                var periods = await membershipPeriodService.GetAllMembershipPeriods();

                Assert.True(!Extenstions.IsNullOrEmpty(periods), "Membership Period has records.");
            }

        }

        [Fact]
        public async void MembershipPeriod_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();

                MembershipPeriodModel model = TestDataGenerator.GetMembershipPeriodModel();

                var  period = await membershipPeriodService.CreateMembershipPeriod(model);

                var returnedPeriod = await membershipPeriodService.GetMembershipPeriodById(period.MembershipPeriodId);

                Assert.True(period.MembershipPeriodId == returnedPeriod.MembershipPeriodId, "Membership Period returns selected Id.");
            }

        }

        [Fact]
        public async void MembershipPeriod_GetSelectList()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();

                MembershipPeriodModel model = TestDataGenerator.GetMembershipPeriodModel();

                var period = await membershipPeriodService.CreateMembershipPeriod(model);

                //Add another

                model = TestDataGenerator.GetMembershipPeriodModel();

                await membershipPeriodService.CreateMembershipPeriod(model);

                var selectList = await membershipPeriodService.GetSelectList();

                Assert.True(!Extenstions.IsNullOrEmpty(selectList), "Membership Period returns selected Id.");
            }

        }


    }
}