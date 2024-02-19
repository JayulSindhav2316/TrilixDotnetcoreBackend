using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CustomFieldController : ControllerBase
    {
        private readonly ILogger<CustomFieldController> _logger;
        private readonly ICustomFieldService _customFieldService;
        public CustomFieldController(ILogger<CustomFieldController> logger, ICustomFieldService customFieldService)
        {
            _logger = logger;
            _customFieldService = customFieldService;
        }
        [HttpGet("GetCustomFieldList")]
        public async Task<ActionResult<List<Fieldtype>>> GetCustomFieldList()
        {
            try
            {
                var list = await _customFieldService.GetCustomFields(1);
                return Ok(list);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("GetCustomFieldsOptionsByFieldId")]
        public async Task<ActionResult<List<SelectListModel>>> GetCustomFieldsOptions(int FieldId)
        {
            try
            {
                var list = await _customFieldService.GetCustomFieldsOptions(FieldId);
                return Ok(list);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("GetCustomFieldTypes")]
        public async Task<ActionResult<List<Fieldtype>>> GetCustomFieldTypes()
        {
            try
            {
                var fieldtypes = await _customFieldService.GetCustomFieldTypes();
                return Ok(fieldtypes);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("AddCustomField")]
        public async Task<ActionResult> AddCustomField(CustomFieldModel Model)
        {
            try
            {
                var res = await _customFieldService.SaveCustomField(Model);
                if(res==null)
                {
                    ModelState.AddModelError("Label", "Already Exist");
                }
                return Ok(res);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }

        [HttpPost("DeleteCustomField")]
        public async Task<ActionResult<bool>> DeleteCustomField(CustomFieldModel model)
        {
            //bool response = false;
            try
            {
              var response = await _customFieldService.DeleteCustomField(model.CustomFieldId);
                if(response==false)
                {
                    return BadRequest(new { message = "The field is in use" });
                }

                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete Field" });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }
        [HttpPost("UpdateCustomField")]
        public async Task<ActionResult<CustomFieldModel>> UpdateCustomField(CustomFieldModel model)
        {
            bool response = false;

            try
            {
                response = await _customFieldService.UpdateField(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Field" });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpGet("GetCustomFieldByModuleAndTab/{module}/{tab}/{entityId}/{currentAction}")]
        public async Task<ActionResult<List<Customfieldlookup>>> GetCustomFields(string module, string tab,int entityId,string currentAction)
        {
            try
            {
                var list = await _customFieldService.GetCustomFieldsByModuleAndTab(module, tab, entityId, currentAction);
                return Ok(list);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("SaveCustomFieldData")]
        public async Task<ActionResult> SaveCustomFieldData(List<Customfielddata> Model)
        {
            try
            {
                var res = await _customFieldService.SaveCustomFieldData(Model);
                return Ok("");
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("CheckCustomFieldData/{customFieldId}")]
        public async Task<ActionResult<bool>> CheckCustomFieldData(int customFieldId)
        {
            try
            {
                var result = await _customFieldService.CheckCustomFieldData(customFieldId);
                return result;
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("GetModuleInfo")]
        public async Task<ActionResult<List<Moduleinfo>>> GetModuleInfo()
        {
            try
            {
                var list = await _customFieldService.GetModuleList();
                return Ok(list);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("AddCustomFieldBlock")]
        public async Task<ActionResult> AddCustomFieldBlock(CustomfieldblockModel Model)
        {
            try
            {
                var res = await _customFieldService.AddCustomFieldBlock(Model);
                return Ok(res);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetBlockList/{module}/{tabinfo}/{blockfor}")]
        public async Task<ActionResult<List<Moduleinfo>>> GetBlockList(string module, string tabinfo, int blockfor)
        {
            try
            {
                var list = await _customFieldService.GetBlockList(module,tabinfo,blockfor);
                return Ok(list);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("UpdateCustomFieldBlock")]
        public async Task<ActionResult<CustomFieldModel>> UpdateCustomFieldBlock(Customfieldblock model)
        {
            bool response = false;
            try
            {
                response = await _customFieldService.UpdateBlock(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Block" });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpDelete("DeleteBlock/{blockId}")]
        public async Task<ActionResult<bool>> DeleteBlock(int blockId)
        {
            //bool response = false;
            try
            {
                var response = await _customFieldService.DeleteBlock(blockId);
                if (response == false)
                {
                    return BadRequest(new { message = "The field is in use" });
                }
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete Field" });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }
    }
}
