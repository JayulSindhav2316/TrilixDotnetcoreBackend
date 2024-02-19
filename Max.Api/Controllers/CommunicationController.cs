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
    public class CommunicationController : ControllerBase
    {

        private readonly ILogger<CommunicationController> _logger;
        private readonly ICommunicationService _CommunicationService;

        public CommunicationController(ILogger<CommunicationController> logger, ICommunicationService CommunicationService)
        {
            _logger = logger;
            _CommunicationService = CommunicationService;
        }

        [HttpGet("GetAllCommunications")]
        public async Task<ActionResult<IEnumerable<Communication>>> GetAllCommunications()
        {
            var Communications = await _CommunicationService.GetAllCommunications();
            return Ok(Communications);
        }
        [HttpGet("GetAllCommunicationsByEntityId")]
        public async Task<ActionResult<IEnumerable<Communication>>> GetAllCommunicationsByEntityId(int entityId)
        {
            var Communications = await _CommunicationService.GetAllCommunicationsByEntityIdId(entityId);
            return Ok(Communications);
        }
        [HttpPost("CreateCommunication")]
        public async Task<ActionResult<Communication>> CreateCommunication([FromBody] CommunicationModel model)
        {
            Communication response = new Communication();

            try
            {
                response = await _CommunicationService.CreateCommunication(model);
                if (response.CommunicationId == 0)
                {
                    return BadRequest(new { message = "Failed to create Membership" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateCommunication")]
        public async Task<ActionResult<Communication>> UpdateCommunication([FromBody] CommunicationModel model)
        {
            Communication response = new Communication();

            try
            {
                response = await _CommunicationService.UpdateCommunication(model);
                if (response.CommunicationId == 0)
                {
                    _logger.LogError($"Could not find reccord with CommunicationId={model.CommunicationId}");
                    return BadRequest(new { message = "Failed to create Membership" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteCommunication")]
        public async Task<ActionResult<Communication>> DeleteCommunication([FromBody] CommunicationModel model)
        {

            try
            {
                await _CommunicationService.DeleteCommunication(model.CommunicationId);

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
