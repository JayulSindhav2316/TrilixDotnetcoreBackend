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
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly ILogger<OrganizationController> _logger;
        private readonly IOrganizationService _organizationService;
        public OrganizationController(ILogger<OrganizationController> logger, IOrganizationService organizationService)
        {
            _logger = logger;
            _organizationService = organizationService;
        }

        [HttpGet("GetAllOrganizations")]
        public async Task<ActionResult<IEnumerable<Organization>>> GetAllOrganizations()
        {
            var organizations = await _organizationService.GetAllOrganizations();
            return Ok(organizations);
        }

        [HttpGet("GetOrganizationById")]
        public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizationById(int organizationId)
        {
            var organization = await _organizationService.GetOrganizationById(organizationId);
            return Ok(organization);
        }

        [HttpPost("CreateOrganization")]
        public async Task<ActionResult<IEnumerable<Organization>>> CreateOrganization(OrganizationModel organizationModel)
        {
            Organization organization = new Organization();
            if (ModelState.IsValid == false)
            {
                return BadRequest(new { message = "Invalid Model Value" });
            }
            try
            {
                organization = await _organizationService.CreateOrganization(organizationModel);

                if (organization.OrganizationId == 0)
                {
                    return BadRequest(new { message = "Failed to create Organization" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(organization);
        }

        [HttpPost("UpdateOrganization")]
        public async Task<ActionResult<Person>> UpdateOrganization(OrganizationModel organizationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update Person" });
            }
            bool response = false;
            if (organizationModel.OrganizationId <= 0) return BadRequest(new { message = "Invalid organization id supplied." });
            try
            {
                response = await _organizationService.UpdateOrganization(organizationModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update organization" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

    }
}
