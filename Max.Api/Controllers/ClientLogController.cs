using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System.Linq;

namespace Max.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientLogController : ControllerBase
    {

        private readonly ILogger<ClientLogController> _logger;
        private readonly IClientLogService _clientLogService;

        public ClientLogController(ILogger<ClientLogController> logger, IClientLogService clientLogService)
        {
            _logger = logger;
            _clientLogService = clientLogService;
        }

        [HttpGet("GetClientLog")]
        public async Task<ActionResult<IEnumerable<ClientLogModel>>> GetClientLog()
        {
            var items = await _clientLogService.GetCleintLogs();
            return Ok(items);
        }

        [HttpPost("CreateLog")]
        public async Task<ActionResult<Clientlog>> CreateLog([FromBody] ClientLogModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                _logger.LogError($"Invalid Model state. {messages}");
            }
            try
            {
                var response = await _clientLogService.CreateLog(model);
                if (response.ClientLogId == 0)
                {
                    return BadRequest(new { message = "Failed to create Log entry" });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
