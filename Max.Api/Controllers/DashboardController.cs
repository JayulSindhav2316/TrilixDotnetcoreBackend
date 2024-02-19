using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Max.Services;
using Microsoft.AspNetCore.Http;
using Max.Core.Models;
using System.Linq;
using System;
using Microsoft.Extensions.Hosting;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {

        private readonly ILogger<DashboardController> _logger;
        private readonly IMembershipService _membershipService;
        private readonly IDocumentContainerService _documentContainerService;
        private readonly IReceiptHeaderService _receiptHeaderService;

         public DashboardController(ILogger<DashboardController> logger, 
                                    IMembershipService membershipService,
                                    IReceiptHeaderService receiptHeaderService,
                                    IDocumentContainerService documentContainerService)
         {
            _logger = logger;
            _membershipService = membershipService;
            _receiptHeaderService = receiptHeaderService;
            _documentContainerService = documentContainerService;
         }

        [HttpGet("GetMembershipsByType")]
        public async Task<ActionResult<IEnumerable<PieChartModel>>> GetMembershipsByType()
        {
            var documents = await _membershipService.GetMembershipsByType();
            return Ok(documents);
        }
        [HttpGet("GetDailySalesByMonth")]
        public async Task<ActionResult<IEnumerable<LineChartModel>>> GetDailySalesByMonth()
        {
            var chartData = await _receiptHeaderService.GetDailySalesByMonth();
            return Ok(chartData);
        }

        [HttpGet("GetMembershipExpirationDetails")]
        public async Task<ActionResult<BarChartModel>> GetMembershipExpirationDetails()
        {
            var chartData =  await _membershipService.GetMembershipExpirationData();
            return Ok(chartData);
        }

        [HttpGet("GetMembershipTerminationsByType")]
        public async Task<ActionResult<DoughnutChartModel>> GetMembershipTerminationsByType()
        {
            var chartData = await _membershipService.GetMembershipTerminationsByType();
            return Ok(chartData);
        }

        [HttpGet("GetDocumentSearchByDocument")]
        public async Task<ActionResult<IEnumerable<BarChartModel>>> GetDocumentSearchByDocument()
        {
            var documents = await _documentContainerService.GetSearchStatistics();
            return Ok(documents);
        }
        [HttpGet("GetMemberPortalActiveUsers")]
        public async Task<ActionResult<IEnumerable<BarChartModel>>> GetMemberPortalActiveUsers()
        {
            var documents = await _documentContainerService.GetMemberPortalActiveUsers();
            return Ok(documents);
        }

    }
}
