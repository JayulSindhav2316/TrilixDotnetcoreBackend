using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {

        private readonly ILogger<ReceiptController> _logger;
        private readonly IReceiptHeaderService _receiptHeaderService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IReceiptDetailService _receiptDetailService;
        private readonly IRefundService _refundService;
        private readonly IVoidService _voidService;
        private readonly IAuthNetService _authNetService;
        private readonly IEmailService _emailService;
        public ReceiptController(ILogger<ReceiptController> logger, 
                                    IReceiptHeaderService receiptHeaderService, 
                                    IPaymentTransactionService paymentTransactionService,
                                    IReceiptDetailService receiptDetailService,
                                    IRefundService refundService,
                                     IVoidService voidService,
                                    IAuthNetService authNetService,
                                    IEmailService emailService
                                    )
        {
            _logger = logger;
            _receiptHeaderService = receiptHeaderService;
            _paymentTransactionService = paymentTransactionService;
            _receiptDetailService = receiptDetailService;
            _refundService = refundService;
            _voidService = voidService;
            _authNetService = authNetService;
            _emailService = emailService;
        }

        [HttpGet("GetAllReceipts")]
        public async Task<ActionResult<IEnumerable<ReceiptHeaderModel>>> GetAllReceipts()
        {
            var roles = await _receiptHeaderService.GetAllReceipts();
            return Ok(roles);
        }

        [HttpGet("GetReceiptDetailById")]
        public async Task<ActionResult<ReceiptModel>> GetReceiptDetailById(int id)
        {
            var receiptDetail = await _receiptHeaderService.GetReceiptDetailByReceiptId(id);
            return Ok(receiptDetail);
        }

        [HttpGet("GetCreditCardReport")]
        public async Task<ActionResult<CreditCardReportModel>> GetCreditCardReport(string cardType, string searchBy, DateTime fromDate, DateTime toDate)
        {
            var creditCardReport = await _paymentTransactionService.GetCreditCardReport(cardType, searchBy, fromDate, toDate);
            return Ok(creditCardReport);
        }
        [HttpGet("GetDepositReport")]
        public async Task<ActionResult<DepositReportModel>> GetDepositReport(string paymentType, string searchBy, DateTime fromDate, DateTime toDate, int summary, string portal )
        {
            var depositReport = await _paymentTransactionService.GetDepositReport(paymentType, searchBy, fromDate, toDate, summary, portal);
            return Ok(depositReport);
        }

        [HttpGet("GetReceiptDetailByCartId")]
        public async Task<ActionResult<ReceiptModel>> GetReceiptDetailByCartId(int cartId)
        {
            var receiptDetail = await _receiptHeaderService.GetReceiptDetailByCartId(cartId);
            return Ok(receiptDetail);
        }

        [HttpGet("GetReceiptDetailByReceiptId")]
        public async Task<ActionResult<ReceiptModel>> GetReceiptDetailByReceiptId(int cartId)
        {
            var receiptDetail = await _receiptHeaderService.GetReceiptDetailByCartId(cartId);
            return Ok(receiptDetail);
        }

        [HttpPost("CreateReceipt")]
        public async Task<ActionResult<Receiptheader>> CreateReceipt(ReceiptHeaderModel model)
        {
            try
            {
                var receipt = await _receiptHeaderService.CreateReceipt(model);
                if (receipt.Receiptid == 0)
                {
                    return BadRequest(new { message = "Failed to create Receipt" });
                }
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("RefundPayment")]
        public async Task<ActionResult<RefundResponseModel>> RefundPayment([FromBody] RefundRequestModel model)
        {
            RefundResponseModel response = new RefundResponseModel();
            try
            {
                response = await _refundService.ProcessRefund(model);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("VoidPayment")]
        public async Task<ActionResult<VoidResponseModel>> VoidPayment([FromBody] VoidRequestModel model)
        {
            VoidResponseModel response = new VoidResponseModel();
            try
            {
                response = await _voidService.ProcessVoid(model);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateReceipt")]
        public async Task<ActionResult<Receiptheader>> UpdateReceipt([FromBody] ReceiptHeaderModel model)
        {
            bool response = false;
            try
            {
                response = await _receiptHeaderService.UpdateReceipt(model);
                if (!response)
                {
                    return BadRequest(new  { message = "Receipt could not be updated." } );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("EmailReceipt")]
        public async Task<ActionResult<bool>> EmailReceipt(EmailMessageModel model)
        {
            var result = await _emailService.SendHtmlReceipt(model);
            return Ok(result);
        }
    }
}
