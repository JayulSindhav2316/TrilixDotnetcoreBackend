using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Microsoft.Extensions.Hosting;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {

        private readonly ILogger<DepartmentController> _logger;
        private readonly IDepartmentService _departmentService;

        public DepartmentController(ILogger<DepartmentController> logger, IDepartmentService DepartmentService, IHostEnvironment appEnvironment)
        {
            _logger = logger;
            _departmentService = DepartmentService;
        }

        [HttpGet("GetAllDepartments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments()
        {
            var Departments = await _departmentService.GetAllDepartments();
            return Ok(Departments);
        }
        [HttpGet("GetSelectList")]
        public async Task<ActionResult<List<Department>>> GetSelectList()
        {
            var Departments = await _departmentService.GetSelectList();
            return Ok(Departments);
        }
        [HttpPost("CreateDepartment")]
        public async Task<ActionResult<List<Department>>> CreateDepartment(DepartmentModel departmentModel)
        {
            Department department = new Department();
            try
            {
                department = await _departmentService.CreateDepartment( departmentModel);
                if (department.DepartmentId == 0)
                {
                    return BadRequest(new { message = "Failed to create Department" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(department);
        }

        [HttpPost("UpdateDepartment")]
        public async Task<ActionResult<DepartmentModel>> UpdateDepartment(DepartmentModel departmentModel)
        {
            bool response = false;
            try
            {
                response = await _departmentService.UpdateDepartment(departmentModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Department" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(response);
        }
        [HttpPost("DeleteDepartment")]
        public async Task<ActionResult<bool>> DeleteDepartment(DepartmentModel departmentmodel)
        {
            bool response = false;
            try
            {
                response = await _departmentService.DeleteDepartment(departmentmodel.DepartmentId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete Department" });
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
