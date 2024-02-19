using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GlAccountTypeController : ControllerBase
    {

        private readonly ILogger<GlAccountTypeController> _logger;
        private readonly IGlAccountTypeService _glAccountTypeService;

        public GlAccountTypeController(ILogger<GlAccountTypeController> logger, IGlAccountTypeService GlAccountTypeService)
        {
            _logger = logger;
            _glAccountTypeService = GlAccountTypeService;
        }

        [HttpGet("GetAllGlAccountTypes")]
        public async Task<ActionResult<IEnumerable<Glaccounttype>>> GetAllGlAccountTypes()
        {
            var glAccountTypes = await _glAccountTypeService.GetAllGlAccountTypes();
            return Ok(glAccountTypes);
        }
        [HttpGet("GetSelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetSelectList()
        {
            var GlAccountTypes = await _glAccountTypeService.GetSelectList();
            return Ok(GlAccountTypes);
        }


    }
}
