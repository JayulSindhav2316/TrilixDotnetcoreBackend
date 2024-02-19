using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MembershipHistoryController : ControllerBase
    {

        private readonly ILogger<MembershipHistoryController> _logger;
        private readonly IMembershipHistoryService _MembershipHistoryService;

        public MembershipHistoryController(ILogger<MembershipHistoryController> logger, IMembershipHistoryService MembershipHistoryService)
        {
            _logger = logger;
            _MembershipHistoryService = MembershipHistoryService;
        }

        [HttpGet("GetAllMembershipHistorys")]
        public async Task<ActionResult<IEnumerable<Membershiphistory>>> GetAllMembershipHistoryies()
        {
            var MembershipHistorys = await _MembershipHistoryService.GetAllMembershipHistorys();
            return Ok(MembershipHistorys);
        }
        [HttpPost("CreateMembershipHistory")]
        public async Task<ActionResult<Membershiphistory>> CreateMembershipHistory([FromBody] MembershipHistoryModel model)
        {
            Membershiphistory response = new Membershiphistory();

            try
            {
                response = await _MembershipHistoryService.CreateMembershipHistory(model);
                if (response.MembershipHistoryId == 0)
                {
                    return BadRequest(new { message = "Failed to create MembershipHistory" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateMembershipHistory")]
        public async Task<ActionResult<Membershiphistory>> UpdateMembershipHistory([FromBody] MembershipHistoryModel model)
        {
            Membershiphistory response = new Membershiphistory();

            try
            {
                response = await _MembershipHistoryService.UpdateMembershipHistory(model);
                if (response.MembershipHistoryId == 0)
                {
                    _logger.LogError("$Could not find reccord with MembershipHistoryId={model.MembershipHistoryId}");
                    return BadRequest(new { message = "Failed to create MembershipHistory" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteMembershipHistory")]
        public async Task<ActionResult<bool>> DeleteMembershipHistory([FromBody] MembershipHistoryModel model)
        {

            try
            {
                await _MembershipHistoryService.DeleteMembershipHistory(model.MembershipHistoryId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }


    }
}
