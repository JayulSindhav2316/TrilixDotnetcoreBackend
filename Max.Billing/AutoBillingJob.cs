using Max.Core.Models;
using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace Max.Billing
{
    public class AutoBillingJob
    {
        private readonly ILogger<AutoBillingJob> _logger;
        private readonly TelemetryClient _telemetryClient;
        public IServiceScopeFactory _serviceScopeFactory;

        public AutoBillingJob(ILogger<AutoBillingJob> logger,
        TelemetryClient telemetryClient,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public void CreateJob(Object obj)
        {
            AutoBillingJobModel model = new AutoBillingJobModel();
            _logger.LogInformation("CreateJob.Creating Job");
            try
            {
               
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
              
            }
        }
    }
}
