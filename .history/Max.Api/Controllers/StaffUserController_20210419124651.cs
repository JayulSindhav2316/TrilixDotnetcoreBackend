using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services;
using Max.Services.Interfaces;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staffuser>>> GetAllStaffUsers()
        {
            var staffUsers = await _staffUserService.GetAllStaffUsers();
            return Ok(staffUsers);
        }
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(string userName,  string password)
        {
            var response = await _staffUserService.Authenticate(userName,password);

            if (!response)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
          
            return Ok(response);
        }
       
    }
}