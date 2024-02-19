using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MembershipPeriodController : ControllerBase
    {

        private readonly ILogger<MembershipPeriodController> _logger;
        private readonly IMembershipPeriodService _membershipPeriodService;

        public MembershipPeriodController(ILogger<MembershipPeriodController> logger, IMembershipPeriodService MembershipPeriodService)
        {
            _logger = logger;
            _membershipPeriodService = MembershipPeriodService;
        }

        [HttpGet("GetAllMembershipPeriods")]
        public async Task<ActionResult<IEnumerable<Membershipperiod>>> GetAllMembershipPeriods()
        {
            var MembershipPeriods = await _membershipPeriodService.GetAllMembershipPeriods();
            return Ok(MembershipPeriods);
        }
        [HttpGet("GetSelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetSelectList()
        {
            var MembershipPeriods = await _membershipPeriodService.GetSelectList();
            return Ok(MembershipPeriods);
        }
        [HttpGet("GetPaymentFrequencySelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetPaymentFrequencySelectList(int period)
        {
            var MembershipFrequencies = await _membershipPeriodService.GetFrequencySelectList(period);
            return Ok(MembershipFrequencies);
        }

    }
}
