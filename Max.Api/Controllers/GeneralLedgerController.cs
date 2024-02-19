using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using System;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GeneralLedgerController : ControllerBase
    {

        private readonly ILogger<GeneralLedgerController> _logger;
        private readonly IGeneralLedgerService _generalLedgerService;

        public GeneralLedgerController(ILogger<GeneralLedgerController> logger, IGeneralLedgerService generalLedgerService)
        {
            _logger = logger;
            _generalLedgerService = generalLedgerService;
        }

        [HttpGet("GetGeneralLedger")]
        public async Task<ActionResult<IEnumerable<GeneralLedgerModel>>> GetGeneralLedger(string glAccount,string searchBy, DateTime fromDate, DateTime toDate)
        {
            var generalLedger = await _generalLedgerService.GetGeneralLedger( glAccount,  searchBy, fromDate, toDate);
            return Ok(generalLedger);
        }
    }
}
