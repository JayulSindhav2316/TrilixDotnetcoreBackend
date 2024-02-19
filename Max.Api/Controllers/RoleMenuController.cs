using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services;
using Max.Services.Interfaces;
using Max.Core.Models;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
  public class RoleMenuController : ControllerBase
  {

    private readonly ILogger<RoleMenuController> _logger;
    private readonly IRoleMenuService _roleMenuService;
    private readonly IMenuService _menuService;

    public RoleMenuController(ILogger<RoleMenuController> logger, IRoleMenuService roleMenuService, IMenuService menuService)
    {
      _logger = logger;
      _roleMenuService = roleMenuService;
      _menuService = menuService;
    }


    [HttpGet("GetMenuByRoleId")]
    public async Task<ActionResult<IEnumerable<RoleMenuModel>>> GetMenuByRoleId(int roleId)
    {
      var listRoleMenuModel = await _roleMenuService.GetMenuByRoleId(roleId);
      return Ok(listRoleMenuModel);
    }

    [HttpPost("UpdateRoleMenubyRoleId")]
    public async Task<ActionResult<bool>> UpdateRoleMenubyRoleId(dynamic requestObject)
    {
      var response = false;

      try
      {
        
        response = await _roleMenuService.UpdateRoleMenubyRoleId(requestObject);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }

      return true;

    }

  }
}
