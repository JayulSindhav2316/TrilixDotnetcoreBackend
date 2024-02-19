using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StateController : ControllerBase
    {
        private readonly ILogger<StateController> _logger;
        private readonly IStateService _stateService;

        public StateController(ILogger<StateController> logger, IStateService stateService)
        {
            _logger = logger;
            _stateService = stateService;
        }

        [HttpGet("GetStatesByCountryId")]
        public async Task<ActionResult<IEnumerable<StateModel>>> GetAll(int countryId)
        {
            var states = await _stateService.GetStatesByCountryIdAsync(countryId);
            return Ok(states);
        }
    }
}
