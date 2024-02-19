using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Api.Helpers;
using System.Linq;
using Max.Core;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BatchRenewalController : ControllerBase
    {

        private readonly ILogger<BillingController> _logger;
        private readonly IPaperInvoiceService _paperInvoiceService;
        private readonly IBillingService _billingService;
        private readonly IBillingFeeService _billingFeeService;
        public BatchRenewalController(ILogger<BillingController> logger,
                                        IPaperInvoiceService paperInvoiceService,
                                        IBillingService billingService,
                                        IBillingFeeService billingFeeService
                                    )
        {
            _logger = logger;
            _paperInvoiceService = paperInvoiceService;
            _billingService = billingService;
            _billingFeeService = billingFeeService;
        }

        [HttpPost("CreateBatchRenewalCycle")]
        public async Task<ActionResult<Billingcycle>> CreateBatchRenewalCycle(BillingCycleModel model)
        {
            Billingcycle response = new Billingcycle();
            try
            {
                model.CycleType = (int)BillingCycleType.Renewal;
                response = await _billingService.CreateBillingCycle(model);
                if (response.BillingCycleId > 0)
                {
                    //Create Job
                    await _billingService.CreateBillingJob(response.BillingCycleId);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("FinalizeBillingCycle")]
        public async Task<ActionResult<bool>> FinalizeBillingCycle(BillingCycleModel model)
        {
            try
            {
                var result = await _billingService.FinzalizeBillingCycle(model.BillingCycleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("GetPreliminaryPaperInvoices")]
        public async Task<ActionResult<PaperInvoiceModel>> GetPreliminaryPaperInvoices()
        {

            try
            {
                var response = await _paperInvoiceService.GetPaperPrliminaryInvoices();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }

        [HttpGet("GetRenewalPaperInvoiceByCycleId")]
        public async Task<ActionResult<PaperInvoiceModel>> GetRenewalPaperInvoiceByCycleId(int billingCycleId)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                _logger.LogError($"Invalid Model state. {messages}");
            }
            try
            {
                var response = await _paperInvoiceService.GetRenewalPaperInvoicesByCycleId(billingCycleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetBatchRenewalCycles")]
        public async Task<ActionResult<BillingCycleModel>> GetBatchRenewalCycles()
        {

            try
            {
                var response = await _billingService.GetBillingCycles((int)BillingCycleType.Renewal);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }
        [HttpPost("DeleteCycle")]
        public async Task<ActionResult<bool>> DeleteCycle(BillingCycleModel model)
        {
            bool response = false;
            try
            {

                response = await _billingService.DeleteBillingCycle(model.BillingCycleId);
                if (!response)
                {
                    return BadRequest(new { error = "Failed to delete cycle." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("RegenrateCycle")]
        public async Task<ActionResult<bool>> RegenrateCycle(BillingCycleModel model)
        {
            bool response = false;
            try
            {

                response = await _billingService.RegenrateBillingCycle(model.BillingCycleId);
                if (!response)
                {
                    return BadRequest(new { error = "Failed to delete cycle." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetLastManualBillingDrafts")]
        public async Task<ActionResult<Paperinvoice>> GetLastManualBillingDrafts(int billingCycleId)
        {
            try
            {
                var response = await _paperInvoiceService.GetLastManualBillingDrafts(billingCycleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("UpdateBillingFee")]
        public async Task<ActionResult<Billingfee>> UpdateBillingFee([FromBody] BillingFeeModel model)
        {
            try
            {
                var response = await _billingFeeService.UpdateBillingFee(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

    }
}
