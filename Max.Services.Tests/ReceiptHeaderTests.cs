using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{ 
    public class ReceiptHeaderTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public ReceiptHeaderTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateReceiptHeader_Add_New() 
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                Assert.True(receiptheader.Receiptid > 0, "ReceiptHeader Created.");
            }

        }

        [Fact]
        public async void UpdateReceiptHeader_Update() 
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                modelReceiptHeader.Receiptid = receiptheader.Receiptid;
                modelReceiptHeader.Status = 1;
                modelReceiptHeader.Notes = "Changed Notes";

                await ReceiptHeaderService.UpdateReceipt(modelReceiptHeader);

                var updatedReceiptHeader = await ReceiptHeaderService.GetReceiptById(receiptheader.Receiptid);

                Assert.True(updatedReceiptHeader.Notes== "Changed Notes", "ReceiptHeader Updated.");
                Assert.True(updatedReceiptHeader.Status == 1, "ReceiptHeader Updated.");

            }

        }

        [Fact]
        public async void GetReceipt_By_Id() 
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var newReceiptHeader = await ReceiptHeaderService.GetReceiptById(receiptheader.Receiptid);

                Assert.True(newReceiptHeader.Receiptid == receiptheader.Receiptid, "ReceiptHeader returns selected Id.");
            }

        }


        [Fact]
        public async void ReceiptHeader_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                //Add another

                modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var ReceiptHeader = await ReceiptHeaderService.GetAllReceipts(); 

                Assert.True(!Extenstions.IsNullOrEmpty(ReceiptHeader), "ReceiptHeader has records.");
            }

        }

        [Fact]
        public async void ReceiptHeader_Get_By_DateRange() 
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var newReceiptHeader = await ReceiptHeaderService.GetReceiptsByDateRange(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(30));

                Assert.True(newReceiptHeader.Where(x => x.Date >= DateTime.Now.AddDays(-1) && x.Date <= DateTime.Now.AddDays(30)).Count() > 0, "ReceiptHeader returns selected Date Range."); ;
            }

        }

        [Fact]
        public async void ReceiptDetail_Get_By_OrganizationId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var newReceiptDetail = await ReceiptHeaderService.GetReceiptsByOrganizationId((int)receiptheader.OrganizationId);

                Assert.True(newReceiptDetail.Where(x => x.OrganizationId == receiptheader.OrganizationId).Count() > 0, "ReceiptHeader returns selected OrganizationId Id.");
            }

        }

        [Fact]
        public async void ReceiptDetail_Get_By_StaffId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var newReceiptDetail = await ReceiptHeaderService.GetReceiptsByStaffId((int)receiptheader.StaffId);

                Assert.True(newReceiptDetail.Where(x=>x.StaffId == receiptheader.StaffId).Count() > 0, "ReceiptHeader returns selected StaffId Id.");
            }

        }

        
        [Fact]
        public async void GetReceiptDetail_By_Id() 
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();

                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();

                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                var newReceiptDetail = await ReceiptHeaderService.GetReceiptDetailById(receiptheader.Receiptid);

                Assert.True(newReceiptDetail.Receiptid == receiptheader.Receiptid, "ReceiptHeader returns selected Receiptid Id.");
            }

        }

    }
}