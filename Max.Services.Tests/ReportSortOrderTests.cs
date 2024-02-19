using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class ReportSortOrderTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public ReportSortOrderTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateReportSortOrder_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var ReportSortOrderService = scope.ServiceProvider.GetService<IReportSortOrderService>();
                ReportSortOrderModel model = TestDataGenerator.GetReportSortorderModel();
                var reportsortorder = await ReportSortOrderService.CreateReportSortorder(model);
                
                Assert.True(reportsortorder.ReportSortOrderId>0 , "Report sorting record created.");
            }

        }
    }
}