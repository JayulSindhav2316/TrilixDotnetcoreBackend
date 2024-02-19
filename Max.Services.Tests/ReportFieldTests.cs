using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class ReportFieldTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public ReportFieldTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateReportField_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var ReportFieldService = scope.ServiceProvider.GetService<IReportFieldService>();
                ReportFieldModel model = TestDataGenerator.GetReportFieldModel();
                var reportField = await ReportFieldService.CreateReportField(model);
                
                Assert.True(reportField.ReportFieldId>0 , "Report sorting record created.");
            }

        }
    }
}