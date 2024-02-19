using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core.Helpers;
using Max.Core;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PromoCodeController : ControllerBase
    {

        private readonly ILogger<PromoCodeController> _logger;
        private readonly IPromoCodeService _promoCodeService;

        public PromoCodeController(ILogger<PromoCodeController> logger, IPromoCodeService promoCodeService)
        {
            _logger = logger;
            _promoCodeService = promoCodeService;
        }

        [HttpGet("GetAllPromoCodes")]
        public async Task<ActionResult<IEnumerable<PromoCodeModel>>> GetAllPromoCodes()
        {
            var promoCodes = await _promoCodeService.GetAllPromoCodes();
            return Ok(promoCodes);
        }

        [HttpGet("GetPromoCodeByCode")]
        public async Task<ActionResult<PromoCodeModel>> GetPromoCodeByCode(string promoCode)
        {
            var promoCodes = await _promoCodeService.GetPromoCodeByCode(promoCode);
            return Ok(promoCodes);
        }

        [HttpGet("GetNewPromoCode")]
        public async Task<ActionResult<PromoCodeModel>> GetNewPromoCode()
        {
            var model = await _promoCodeService.GenratePromoCode();
            return Ok(model);
        }

        [HttpGet("GetTransactionTypes")]
        public ActionResult<IEnumerable<EnumOptionListModel>> GetTransactionTypes()
        {
            List<EnumOptionListModel> list = new List<EnumOptionListModel>();
            foreach (int value in Enum.GetValues(typeof(TransactionType)))
            {
                list.Add(new EnumOptionListModel { Name = EnumUtil.GetDescription(((TransactionType)value)), Code = value });
            }
            return Ok(list);
        }

        [HttpGet("GetDiscountTypes")]
        public ActionResult<IEnumerable<EnumOptionListModel>> GetDiscountTypes()
        {
            List<EnumOptionListModel> list = new List<EnumOptionListModel>();
            foreach (int value in Enum.GetValues(typeof(DiscountType)))
            {
                list.Add(new EnumOptionListModel { Name = EnumUtil.GetDescription(((DiscountType)value)), Code = value });
            }
            return Ok(list);
        }

        [HttpPost("CreatePromoCode")]
        public async Task<ActionResult<PromoCodeModel>> CreatePromoCode(PromoCodeModel model)
        {
            Promocode promoCode = new Promocode();
            try
            {
                promoCode = await _promoCodeService.CreatePromoCode(model);
                if (promoCode.PromoCodeId == 0)
                {
                    return BadRequest(new { message = "Failed to create PromoCode" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(promoCode);
        }

        [HttpPost("UpdatePromoCode")]
        public async Task<ActionResult<PromoCodeModel>> UpdatePromoCode(PromoCodeModel model)
        {
            bool response = false;
            try
            {
                response = await _promoCodeService.UpdatePromoCode(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update PromoCode" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(response);
        }
        [HttpPost("DeletePromoCode")]
        public async Task<ActionResult<bool>> DeletePromoCode(PromoCodeModel model)
        {
            bool response = false;
            try
            {
                response = await _promoCodeService.DeletePromoCode(model.PromoCodeId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete PromoCode" });
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
