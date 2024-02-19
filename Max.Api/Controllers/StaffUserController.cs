using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Api.Helpers;

namespace Max.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StaffUserController : ControllerBase
    {

        private readonly ILogger<StaffUserController> _logger;
        private readonly IStaffUserService _staffUserService;
        private readonly IMenuService _menuService;

        public StaffUserController(ILogger<StaffUserController> logger, IStaffUserService staffUserService, IMenuService menuService )
        {
            _logger = logger;
            _staffUserService=staffUserService;
            _menuService = menuService;
        }

        [HttpGet("GetAllStaffUsers")]
        public async Task<ActionResult<IEnumerable<StaffUserModel>>> GetAllStaffUsers()
        {
            try
            {
                var staffUsers = await _staffUserService.GetAllStaffUsers();
                return Ok(staffUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. Please try again or contact support team if error persists.");
            }
           
        }

        [HttpPost("CreateStaffUser")]
        public async Task<ActionResult<Staffuser>> CreateStaffUser(StaffUserModel model)
        {
            Staffuser response = new Staffuser();
            try
            {
                response = await _staffUserService.CreateStaffUser(model);
                if (response.UserId ==  0)
                {
                    return BadRequest(new { message = "Failed to create Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }


        [HttpPost("UpdateStaffUser")]
        public async Task<ActionResult<Staffuser>> UpdateStaffUser(StaffUserModel model)
        {
            bool response = false ;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(406, "Invalid information."));
            }
            
            try
            {
                response = await _staffUserService.UpdateStaffUser(model);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304,"Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update Staff User: Failed to update: failed with error {ex.Message} {ex.StackTrace}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteStaffUser")]
        public async Task<ActionResult<bool>> DeleteStaffUser(StaffUserModel model)
        {
            bool response = false;
            try
            {
                response = await _staffUserService.DeleteStaffUser(model.UserId);
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
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<bool>> ResetPassword(StaffUserModel model)
        {
            bool response = false;
            try
            {
                response = await _staffUserService.ResetPassword(model.UserId,model.Password);
                if (!response)
                {
                    return BadRequest(new { message = $"Failed to update Password" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpGet("GetStaffMenu")]
        public async Task<ActionResult<object>> GetStaffMenu(int staffId)
        {
            try
            {
                var staffMenu = await _menuService.GetMenuByStaffId(staffId);
                var data = new { staffMenu = staffMenu.Item1, accessUrl=staffMenu.Item2 };
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unable to get menu list for the staff user.");
            }

        }

        [HttpGet("GetAllStaffUsersByNameAndEmail")]
        public async Task<ActionResult<IEnumerable<StaffUserModel>>> GetAllStaffUsersByNameAndEmail(string value)
        {
            try
            {
                var staffUsers = await _staffUserService.GetAllStaffUsersByNameAndUserNameAndEmail(value);
                return Ok(staffUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. Please try again or contact support team if error persists.");
            }

        }
        [HttpGet("GetStaffUserSelectList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetStaffUserSelectList()
        {
            try
            {
                var selectList = await _staffUserService.GetStaffUserSelectList();
                return Ok(selectList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

    }
}
