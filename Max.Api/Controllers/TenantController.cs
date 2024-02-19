using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("GetTenantByHostName")]
        public async Task<ActionResult<Tenant>> GetTenantByHostName(string hostName)
        {
            var tenant = await _tenantService.GetTenantByOrganizationName(hostName);
            return Ok(tenant);
        }

        [HttpGet("GetTenantByOrganizationName")]
        public async Task<ActionResult<Tenant>> GetTenantByOrganizationName(string organizationName)
        {
            var tenant = await _tenantService.GetTenantByOrganizationName(organizationName);
            return Ok(tenant);
        }
    }
}
