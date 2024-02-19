using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AutoMapper;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class MembershipHistoryTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public MembershipHistoryTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateMembershipHistory_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var MembershipHistoryService = scope.ServiceProvider.GetService<IMembershipHistoryService>();

                MembershipHistoryModel model = TestDataGenerator.GetMembershipHistoryModel();

                var MembershipHistory = await MembershipHistoryService.CreateMembershipHistory(model);
                Assert.True(MembershipHistory.MembershipHistoryId > 0, "MembershipHistory Created.");
            }

        }

        [Fact]
        public async void MembershipHistory_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var MembershipHistoryService = scope.ServiceProvider.GetService<IMembershipHistoryService>();

                MembershipHistoryModel model = TestDataGenerator.GetMembershipHistoryModel();

                await MembershipHistoryService.CreateMembershipHistory(model);

                //Add another

                model = TestDataGenerator.GetMembershipHistoryModel();

                await MembershipHistoryService.CreateMembershipHistory(model);

                var MembershipHistorys = await MembershipHistoryService.GetAllMembershipHistorys();

                Assert.True(!Extenstions.IsNullOrEmpty(MembershipHistorys), "MembershipHistory has records.");
            }

        }

        [Fact]
        public async void MembershipHistory_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var MembershipHistoryService = scope.ServiceProvider.GetService<IMembershipHistoryService>();
                MembershipHistoryModel model = TestDataGenerator.GetMembershipHistoryModel();
                var MembershipHistory = await MembershipHistoryService.CreateMembershipHistory(model);

                var newMembershipHistory = await MembershipHistoryService.GetMembershipHistoryById(MembershipHistory.MembershipHistoryId);

                Assert.True(newMembershipHistory.MembershipHistoryId == MembershipHistory.MembershipHistoryId, "MembershipHistory returns selected Id.");
            }

        }

        [Fact]
        public async void MembershipHistory_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var MembershipHistoryService = scope.ServiceProvider.GetService<IMembershipHistoryService>();
                MembershipHistoryModel model = TestDataGenerator.GetMembershipHistoryModel();
                var MembershipHistory = await MembershipHistoryService.CreateMembershipHistory(model);


                model.MembershipHistoryId = MembershipHistory.MembershipHistoryId;
                model.Reason = "Changed Reason";
                model.MembershipId = 99;

                await MembershipHistoryService.UpdateMembershipHistory(model);

                var updatedrecord = await MembershipHistoryService.GetMembershipHistoryById(MembershipHistory.MembershipHistoryId);

                Assert.True(MembershipHistory.Reason == "Changed Reason", "MembershipHistory updated.");
                Assert.True(MembershipHistory.MembershipId == 99, "MembershipHistory updated.");


            }

        }

        [Fact]
        public async void MembershipHistory_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var MembershipHistoryService = scope.ServiceProvider.GetService<IMembershipHistoryService>();
                MembershipHistoryModel model = TestDataGenerator.GetMembershipHistoryModel();
                var MembershipHistory = await MembershipHistoryService.CreateMembershipHistory(model);
                

                await MembershipHistoryService.DeleteMembershipHistory(MembershipHistory.MembershipHistoryId);

                var deletedrecord = await MembershipHistoryService.GetMembershipHistoryById(MembershipHistory.MembershipHistoryId);

                Assert.True(deletedrecord == null, "MembershipHistory Deleted.");
                
            }

        }        

    }
}