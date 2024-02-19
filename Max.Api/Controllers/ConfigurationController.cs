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
    public class ConfigurationController : ControllerBase
    {

        private readonly ILogger<ConfigurationController> _logger;
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(ILogger<ConfigurationController> logger, IConfigurationService configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;
        }

        [HttpGet("GetConfigurationByOrganizationId")]
        public async Task<ActionResult<ConfigurationModel>> GetConfigurationByOrganizationId(int organizationId)
        {
            var configuration = await _configurationService.GetConfigurationByOrganizationId(organizationId);
            return Ok(configuration);
        }
    }
}
