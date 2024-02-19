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
    [Route("api/[controller]")]
    public class StaffUserController : ControllerBase
    {

        private readonly ILogger<StaffUserController> _logger;
        private readonly IStaffUserService _staffUserService;

        public StaffUserController(ILogger<StaffUserController> logger, IStaffUserService staffUserService )
        {
            _logger = logger;
            _staffUserService=staffUserService;
        }

        [HttpGet("GetAllStaffUsers")]
        public async Task<ActionResult<IEnumerable<Staffuser>>> GetAllStaffUsers()
        {
            var staffUsers = await _staffUserService.GetAllStaffUsers();
            return Ok(staffUsers);
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
            try
            {
                response = await _staffUserService.UpdateStaffUser(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpDelete("DeleteStaffUser")]
        public async Task<ActionResult<Staffuser>> DeleteStaffUser(int staffUserId)
        {
            bool response = false;
            try
            {
                response = await _staffUserService.DeleteStaffUser(staffUserId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Staff User" });
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
