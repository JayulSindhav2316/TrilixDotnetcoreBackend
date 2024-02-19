using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{
    public class ReceiptDetailTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public ReceiptDetailTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateReceiptDetail_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptDetailService = scope.ServiceProvider.GetService<IReceiptDetailService>();

                ReceiptDetailModel modelReceiptDetail = TestDataGenerator.GetReceiptDetailModel();

                var receiptdetail = await ReceiptDetailService.CreateReceipt(modelReceiptDetail);

                Assert.True(receiptdetail.ReceiptDetailId > 0, "ReceiptDetail Created.");
            }

        }
         
        [Fact]
        public async void UpdateReceiptDetail_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var ReceiptDetailService = scope.ServiceProvider.GetService<IReceiptDetailService>();

                ReceiptDetailModel modelReceiptDetail = TestDataGenerator.GetReceiptDetailModel();

                var receiptdetail = await ReceiptDetailService.CreateReceipt(modelReceiptDetail);

                modelReceiptDetail.ReceiptDetailId = receiptdetail.ReceiptDetailId;
                modelReceiptDetail.Description = "Changed Description";

                await ReceiptDetailService.UpdateReceipt(modelReceiptDetail);

                var updatedReceiptDetail = await ReceiptDetailService.GetReceiptDetailsById(receiptdetail.ReceiptDetailId);

                Assert.True(updatedReceiptDetail.Description == "Changed Description", "ReceiptDetail Updated.");

            }

        }

        [Fact]
        public async void ReceiptDetail_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptDetailService = scope.ServiceProvider.GetService<IReceiptDetailService>();

                ReceiptDetailModel modelReceiptDetail = TestDataGenerator.GetReceiptDetailModel();

                var receiptdetail = await ReceiptDetailService.CreateReceipt(modelReceiptDetail);

                var newReceiptDetail = await ReceiptDetailService.GetReceiptDetailsById(receiptdetail.ReceiptDetailId);

                Assert.True(newReceiptDetail.ReceiptDetailId == receiptdetail.ReceiptDetailId, "ReceiptDetail returns selected Id.");
            }

        }

        [Fact]
        public async void ReceiptDetail_Get_By_ReceiptId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope()) 
            {
                //ReceiptHeaderService
                var ReceiptHeaderService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                ReceiptHeaderModel modelReceiptHeader = TestDataGenerator.GetReceiptHeaderModel();
                var receiptheader = await ReceiptHeaderService.CreateReceipt(modelReceiptHeader);

                //ReceiptDetailService
                var ReceiptDetailService = scope.ServiceProvider.GetService<IReceiptDetailService>();
                ReceiptDetailModel modelReceiptDetail = TestDataGenerator.GetReceiptDetailModel();
                var receiptdetail = await ReceiptDetailService.CreateReceipt(modelReceiptDetail);
                
                var newReceiptDetail = await ReceiptDetailService.GetReceiptDetailsByReceiptId(receiptheader.Receiptid);
                Assert.True(newReceiptDetail.Where(x=>x.ReceiptHeader.Receiptid == receiptdetail.ReceiptHeader.Receiptid).Count() > 0, "ReceiptDetail returns selected ReceiptId.");
            }

        }
     
        [Fact]
        public async void ReceiptDetail_Get_By_InvoiceDetailId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ReceiptDetailService = scope.ServiceProvider.GetService<IReceiptDetailService>();
                ReceiptDetailModel modelReceiptDetail = TestDataGenerator.GetReceiptDetailModel();
                var receiptdetail = await ReceiptDetailService.CreateReceipt(modelReceiptDetail);

                var newReceiptDetail = await ReceiptDetailService.GetReceiptDetailsByInvoiceDetailId((int)receiptdetail.InvoiceDetailId);

                Assert.True(newReceiptDetail.Where(x => x.InvoiceDetailId == receiptdetail.InvoiceDetailId).Count() > 0, "ReceiptDetail returns selected InvoiceDetail Id.");
            }

        }

    }
}