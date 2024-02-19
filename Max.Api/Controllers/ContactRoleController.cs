using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactRoleController : ControllerBase
    {
        private readonly ILogger<ContactRoleController> _logger;
        private readonly IContactRoleService _contactRoleService;

        public ContactRoleController(ILogger<ContactRoleController> logger, IContactRoleService contactRoleService)
        {
            _logger = logger;
            _contactRoleService = contactRoleService;
        }

        //TODO:AKS add more descriptive logging 
        [HttpPost("CreateContactRole")]
        public async Task<ActionResult<Contactrole>> CreateContactRole(ContactRoleModel model)
        {
            Contactrole role = new Contactrole();

            try
            {
                role = await _contactRoleService.CreateContactRole(model);
                if (role.ContactRoleId == 0)
                {
                    return BadRequest(new { message = "Failed to create contact Role" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(role);

        }
        //TODO:AKS add more descriptive logging 
        [HttpPost("UpdateContactRole")]
        public async Task<ActionResult<Contactrole>> UpdateContactRole(ContactRoleModel model)
        {
            bool success = false;

            try
            {
                success = await _contactRoleService.UpdateContactRole(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }
        //TODO:AKS add more descriptive logging 
        [HttpPost("DeleteContactRole")]
        public async Task<ActionResult<bool>> DeleteContactRole(ContactRoleModel model)
        {
            bool response = false;
            try
            {
                response = await _contactRoleService.DeleteContactRole(model.ContactRoleId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        //TODO:AKS add more descriptive logging 
        [HttpGet("GetAllContactRoles")]
        public async Task<ActionResult<List<Contactrole>>> GetAllContactRoles()
        {
            var groups = await _contactRoleService.GetAllContactRoles();
            return Ok(groups);
        }
        [HttpGet("GetContactRoleById")]
        public async Task<ActionResult<Contactrole>> GetContactRoleById(int roleId)
        {
            var role = await _contactRoleService.GetContactRoleById(roleId);
            return Ok(role);
        }
        [HttpGet("GetContactRoleSelectList")]
        public async Task<ActionResult<List<LinkGroupRoleModel>>> GetContactRoleSelectList()
        {
            var contactRoles = await _contactRoleService.GetContactRoleSelectList();
            return Ok(contactRoles);
        }
    }
}
