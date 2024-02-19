using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using System;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GlAccountController : ControllerBase
    {

        private readonly ILogger<GlAccountController> _logger;
        private readonly IGlAccountService _glAccountService;

        public GlAccountController(ILogger<GlAccountController> logger, IGlAccountService GlAccountService)
        {
            _logger = logger;
            _glAccountService = GlAccountService;
        }

        [HttpGet("GetAllGlaccounts")]
        public async Task<ActionResult<IEnumerable<Glaccount>>> GetAllGlaccounts()
        {
            var glAccounts = await _glAccountService.GetAllGlaccounts();
            return Ok(glAccounts);
        }
        [HttpGet("GetSelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetSelectList()
        {
            var glAccounts = await _glAccountService.GetSelectList();
            return Ok(glAccounts);
        }

        [HttpGet("GetGlAccountSelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetGlAccountSelectList()
        {
            var glAccounts = await _glAccountService.GetGlAccountSelectList();
            return Ok(glAccounts);
        }

        [HttpGet("GetAllGLAccountsSelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetAllGLAccountsSelectList()
        {
            var glAccounts = await _glAccountService.GetAllGLAccountsSelectList();
            return Ok(glAccounts);
        }

        [HttpPost("CreateGLAccount")]
        public async Task<ActionResult<Glaccount>> CreateGLAccount(GlAccountModel model)
        {
            Glaccount glAccount = new Glaccount();

            try
            {
                glAccount = await _glAccountService.CreateGlAccount(model);

                if (glAccount.GlAccountId==0)
                {
                    return BadRequest(new { message = "Failed to create GL Account" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(glAccount);
        }

        [HttpPost("UpdateGlAccount")]
        public async Task<ActionResult<Glaccount>> UpdateGlAccount(GlAccountModel model)
        {
            bool response = false;
            try
            {
                response = await _glAccountService.UpdateGlAccount(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update GL Account" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("DeleteGlAccount")]
        public async Task<ActionResult<bool>> DeleteGlAccount(GlAccountModel model)
        {
            bool response = false;
            try
            {
                response = await _glAccountService.DeleteGlAccount(model.GlAccountId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete GL Account" });
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
