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
    public class RoleController : ControllerBase
    {

        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<List<RoleModel>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRoles();
            return Ok(roles);
        }

        [HttpGet("GetActiveRoles")]
        public async Task<ActionResult<List<RoleModel>>> GetActiveRoles()
        {
            var roles = await _roleService.GetActiveRoles();
            return Ok(roles);
        }
        [HttpGet("GetRolesByCompanyId")]
        public async Task<ActionResult<List<RoleModel>>> GetRolesByCompanyId(int companyId)
        {
            var roles = await _roleService.GetRolesByCompanyId(companyId);
            return Ok(roles);
        }
        [HttpPost("CreateRole")]
        public async Task<ActionResult<Role>> CreateRole(RoleModel roleModel)
        {
            Role role = new Role();

            try
            {
                role = await _roleService.CreateRole(roleModel);
                if (role.RoleId == 0)
                {
                    return BadRequest(new { message = "Failed to create Role" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(role);

        }

        [HttpPost("DeleteRole")]
        public async Task<ActionResult<bool>> DeleteRole(RoleModel model)
        {
            bool response = false;
            try
            {
                response = await _roleService.DeleteRole(model.RoleId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateRole")]
        public async Task<ActionResult<RoleModel>> UpdateRole(RoleModel roleModel)
        {
            bool response = false;

            try
            {
                response = await _roleService.UpdateRole(roleModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Role" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

    }
}