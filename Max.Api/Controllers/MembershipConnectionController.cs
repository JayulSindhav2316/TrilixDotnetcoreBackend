using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Services.Interfaces;
using Max.Data.DataModel;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembershipConnectionController : ControllerBase
    {
        private readonly ILogger<MembershipConnectionController> _logger;
        private readonly IMembershipConnectionService _membershipConnectionService;

        public MembershipConnectionController(ILogger<MembershipConnectionController> logger, IMembershipConnectionService membershipConnectionService)
        {
            _logger = logger;
            _membershipConnectionService = membershipConnectionService;
        }

        [HttpGet("GetMembershipConnectionsByMembershipId")]
        public async Task<ActionResult<IEnumerable<Membershipconnection>>> GetMembershipConnectionsByMembershipId(int membershipId)
        {
            var membershipconnection = await _membershipConnectionService.GetMembershipConnectionsByMembershipId(membershipId);
            return Ok(membershipconnection);
        }
    }
}
