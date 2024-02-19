using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Max.Core;
using System.Linq;

namespace Max.Services.Tests
{
    public class StaffSearchHistoryTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public StaffSearchHistoryTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }
        //[Fact]
        //public async void SearchHistory_Get_By_UserId()
        //{
        //    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        //    using (var scope = serviceScopeFactory.CreateScope())
        //    {
        //        var historyService = scope.ServiceProvider.GetService<IStaffSearchHistoryService>();
        //        var model = TestDataGenerator.GetStaffUserSearchHistoryModel();

        //        await historyService.SaveSearchText(model.SearchText, model.Id);

        //        var res = await historyService.GetSearchHistory(model.Id);

        //        Assert.Equal(model.SearchText, res[0]);

        //    }
        //}
        //[Fact]
        //public async void SearchHistory_Save_Search_Record()
        //{
        //    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        //    using (var scope = serviceScopeFactory.CreateScope())
        //    {
        //        var historyService = scope.ServiceProvider.GetService<IStaffSearchHistoryService>();
        //        var model = TestDataGenerator.GetStaffUserSearchHistoryModel();

        //        var res = await historyService.SaveSearchText(model.SearchText, model.Id);

        //        Assert.True(res);
        //    }
        //}

        //[Fact]
        //public async void SearchHistory_Remove_Last_Record_Add_New_If_Count_Is_Ten()
        //{
        //    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        //    using (var scope = serviceScopeFactory.CreateScope())
        //    {
        //        var historyService = scope.ServiceProvider.GetService<IStaffSearchHistoryService>();
        //        int staffUserId = 456;
        //        for (int i = 0; i < 10; i++)
        //        {
        //            var model = TestDataGenerator.GetStaffUserSearchHistoryModel();
        //            await historyService.SaveSearchText(model.SearchText, staffUserId);
        //        }
        //        var newRecord = TestDataGenerator.GetStaffUserSearchHistoryModel();
        //        var res = await historyService.SaveSearchText(newRecord.SearchText, staffUserId);
        //        var records = await historyService.GetSearchHistory(staffUserId);

        //        Assert.Equal(10, records.Count);
        //        Assert.Contains(records, s => s.Contains(newRecord.SearchText));
        //    }
        //}
        //[Fact]
        //public async void SearchHistory_Dont_Save_If_Alreday_Exist()
        //{
        //    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        //    using (var scope = serviceScopeFactory.CreateScope())
        //    {
        //        var historyService = scope.ServiceProvider.GetService<IStaffSearchHistoryService>();

        //        var model = TestDataGenerator.GetStaffUserSearchHistoryModel();
        //        await historyService.SaveSearchText(model.SearchText, model.StaffUserId);

        //        await historyService.SaveSearchText(model.SearchText, model.StaffUserId);

        //        var records = await historyService.GetSearchHistory(model.StaffUserId);

        //        Assert.Single(records);

        //    }
        //}
    }
}