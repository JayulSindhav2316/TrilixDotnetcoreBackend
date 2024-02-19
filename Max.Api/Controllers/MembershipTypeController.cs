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
    [Authorize]
    [Route("api/[controller]")]
    public class MembershipTypeController : ControllerBase
    {

        private readonly ILogger<MembershipTypeController> _logger;
        private readonly IMembershipTypeService _membershipTypeService;

        public MembershipTypeController(ILogger<MembershipTypeController> logger, IMembershipTypeService membershipTypeService)
        {
            _logger = logger;
            _membershipTypeService = membershipTypeService;
        }

        [HttpGet("GetAllMembershipTypes")]
        public async Task<ActionResult<IEnumerable<MembershipTypeModel>>> GetAllMembershipTypes()
        {
            var membershipTypes = await _membershipTypeService.GetAllMembershipTypes();
            return Ok(membershipTypes);
        }

        [HttpGet("GetMembershipTypeById")]
        public async Task<ActionResult<MembershipTypeModel>> GetMembershipTypeById(int membershipTypeId)
        {

            var membershipTypes = await _membershipTypeService.GetMembershipTypeById(membershipTypeId);
            return Ok(membershipTypes);
        }

        [HttpGet("GetMembershipTypesByCategories")]
        public async Task<ActionResult<IEnumerable<Membershiptype>>> GetMembershipTypesByCategories(string selectedCategories)
        {
            if (String.IsNullOrEmpty(selectedCategories)) return BadRequest(new { message = "Invalid Membership Category ID." });

            var membershipTypes = await _membershipTypeService.GetMembershipTypesByCategoryIds(selectedCategories);
            return Ok(membershipTypes);
        }
        [HttpGet("GetMembershipTypeSelectListByCategories")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetMembershipTypeSelectListByCategories(string selectedCategories)
        {
            if (String.IsNullOrEmpty(selectedCategories)) return BadRequest(new { message = "Invalid Membership Category ID." });

            var membershipTypes = await _membershipTypeService.GetMembershipTypeSelectListByCategoryIds(selectedCategories);
            return Ok(membershipTypes);
        }
        [HttpPost("CreateMembershipType")]
        public async Task<ActionResult<Membershiptype>> CreateMembershipType([FromBody] MembershipTypeModel model)
        {
            Membershiptype response = new Membershiptype();
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                _logger.LogError($"Invalid Model state. {messages}");
            }
            try
            {
                response = await _membershipTypeService.CreateMembershipType(model);
                if (response.MembershipTypeId == 0)
                {
                    return BadRequest(new { message = "Failed to create Membership" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateMembershipType")]
        public async Task<ActionResult<Membershiptype>> UpdateMembershipType([FromBody] MembershipTypeModel model)
        {
            Membershiptype response = new Membershiptype();

            try
            {
                response = await _membershipTypeService.UpdateMembershipType(model);
                if (response.MembershipTypeId == 0)
                {
                    _logger.LogError("$Could not find reccord with MembershipTypeId={model.MembershipTypeId}");
                    return BadRequest(new { message = "Failed to create Membership" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteMembershipType")]
        public async Task<ActionResult<Membershiptype>> DeleteMembershipType([FromBody] MembershipTypeModel model)
        {
            
            try
            {
                await  _membershipTypeService.DeleteMembershipType(model.MembershipTypeId);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }
    }
}
