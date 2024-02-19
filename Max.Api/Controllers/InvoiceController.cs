using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Microsoft.Extensions.Hosting;
using Max.Core;
using Max.Core.Helpers;
using System.Linq;
using Max.Api.Helpers;
using Microsoft.AspNetCore.Cors;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<InvoiceController> _logger;
        private readonly IInvoiceService _invoiceService;
        private readonly IEmailService _emailService;
        private readonly IDocumentService _documentService;
        private readonly IPaperInvoiceService _paperInvoiceService;
        private readonly IWriteOffService _writeOffService;
        [System.Obsolete]
        private readonly IHostEnvironment _appEnvironment;

        [Obsolete]
        public InvoiceController(ILogger<InvoiceController> logger, 
                                    IInvoiceService InvoiceService, 
                                    IEmailService emailService, 
                                    IHostEnvironment appEnvironment,
                                    IDocumentService documentService,
                                    IPaperInvoiceService paperInvoiceService,
                                    IWriteOffService writeOffService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _invoiceService = InvoiceService;
            _emailService = emailService;
            _appEnvironment = appEnvironment;
            _documentService = documentService;
            _paperInvoiceService = paperInvoiceService;
            _writeOffService = writeOffService;
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("GetAllInvoices")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoices()
        {
            var Invoices = await _invoiceService.GetAllInvoices();
            return Ok(Invoices);
        }
        [HttpGet("GetInvoicesByEntityId")]
        public async Task<ActionResult<IEnumerable<InvoicePaymentModel>>> GetInvoicesByEntityId(int entityId, string sortOrder="", string paymentStatus="")
        {
            var Invoices = await _invoiceService.GetAllInvoicesByEntityId(entityId, sortOrder, paymentStatus);
            return Ok(Invoices);
        }
        [HttpGet("GetMembershipDueByEntityId")]
        public ActionResult<decimal> GetMembershipDueByEntityId(int entityId)
        {
            var due =  _invoiceService.GetBalanceByEntityId(entityId);
            return Ok(due);
        }
        [HttpGet("GetMembershipBillingHistory")]
        public async Task<ActionResult<IEnumerable<Person>>> GetMembershipBillingHistory(int entityId)
        {
            var history = await _invoiceService.GetInvoicePaymentsByEntityId(entityId);
            return Ok(history);
        }


        [HttpGet("GetInvoicesBySearchCondition")]
        public async Task<ActionResult<IEnumerable<InvoicePaymentModel>>> GetInvoicesBySearchCondition(int entityId, string serachBy, string itemDescription, DateTime startDate, DateTime endDate)
        {
            var Invoices = await _invoiceService.GetInvoicesBySearchCondition(entityId,  serachBy,  itemDescription, startDate, endDate);
            return Ok(Invoices);
        }

        [HttpGet("GetInvoiceDetailsByInvoiceId")]
        public async Task<ActionResult<InvoiceModel>> GetInvoiceDetailsByInvoiceId(int invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceDetailsByInvoiceId(invoiceId);
            return Ok(invoice);
        }

        [HttpGet("GetMembershipInvoiceDues")]
        public async Task<ActionResult<MembershipDuesModel>> GetMembershipInvoicedues()
        {
            var invoiceDues = await _invoiceService.GetMembershipInvoiceDues();
            return Ok(invoiceDues);
        }

        [HttpPost("EmailInvoice")]
        public async Task<ActionResult<bool>> EmailInvoice(EmailMessageModel model)
        {
            var result = await _invoiceService.SendHtmlInvoice(model);
            return Ok(result);
        }

        [HttpPost("CreateInvoice")]
        public async Task<ActionResult<InvoiceModel>> CreateInvoice([FromBody] InvoiceModel model)
        {
            InvoiceModel response = new InvoiceModel();

            try
            {
                response = await _invoiceService.CreateInvoice(model);
                if (response.InvoiceId == 0)
                {
                    return BadRequest(new { message = "Failed to create Invoice" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("CreateItemInvoice")]
        public async Task<ActionResult<InvoiceModel>> CreateItemInvoice([FromBody] GeneralInvoiceModel model)
        {
            InvoiceModel response = new InvoiceModel();
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                _logger.LogError($"Invalid Model state. {messages}");
            }
            try
            {
                response = await _invoiceService.CreateItemInvoice(model);
                if (response.InvoiceId == 0)
                {
                    return BadRequest(new { message = "Failed to create Invoice" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateItemInvoice")]
        public async Task<ActionResult<Invoice>> UpdateItemInvoice([FromBody] GeneralInvoiceModel model)
        {
            Invoice response = new Invoice();

            try
            {
                response = await _invoiceService.UpdateItemInvoice(model);
                if (response.InvoiceId == 0)
                {
                    _logger.LogError("$Could not find reccord with InvoiceId={model.InvoiceId}");
                    return BadRequest(new { message = "Failed to create Invoice" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateInvoice")]
        public async Task<ActionResult<Invoice>> UpdateInvoice([FromBody] InvoiceModel model)
        {
            Invoice response = new Invoice();

            try
            {
                response = await _invoiceService.UpdateInvoice(model);
                if (response.InvoiceId == 0)
                {
                    _logger.LogError("$Could not find reccord with InvoiceId={model.InvoiceId}");
                    return BadRequest(new { message = "Failed to create Invoice" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteInvoice")]
        public async Task<ActionResult<bool>> DeleteInvoice([FromBody] InvoiceModel model)
        {

            try
            {
                await _invoiceService.DeleteInvoice(model.InvoiceId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }
       
        [HttpGet("GetAllOutstandingReceivables")]
        public async Task<ActionResult<ReceivablesReportMembershipDueModel>> GetAllOutstandingReceivables()
        {
            var invoiceDues = await _invoiceService.GetAllOutstandingReceivables();
            return Ok(invoiceDues);
        }
        [HttpGet("GetRefundModes")]
        public ActionResult<IEnumerable<EnumOptionListModel>> GetRefundModes(string paymentMode)
        {
            List<EnumOptionListModel> list = new List<EnumOptionListModel>();
            foreach (int value in Enum.GetValues(typeof(RefundMode)))
            {
                if (paymentMode == "Check" && ( EnumUtil.GetDescription(((RefundMode)value)) == "Credit Card" || EnumUtil.GetDescription(((RefundMode)value)) == "ACH / eCheck"))
                    continue;
                if (paymentMode == "CreditCard" && EnumUtil.GetDescription(((RefundMode)value)) == "ACH / eCheck")
                    continue;
                if (paymentMode == "eCheck" && EnumUtil.GetDescription(((RefundMode)value)) == "Credit Card")
                    continue;
                if (paymentMode == "Off Line" && (EnumUtil.GetDescription(((RefundMode)value)) == "ACH / eCheck" || EnumUtil.GetDescription(((RefundMode)value)) == "Credit Card"))
                    continue;
                list.Add(new EnumOptionListModel { Name = EnumUtil.GetDescription(((RefundMode)value)), Code = value });
            }
            return Ok(list);
        }

        [HttpGet("GetInvoicesWithBalanceByEntityId")]
        public async Task<ActionResult<IEnumerable<InvoicePaymentModel>>> GetInvoicesWithBalanceByEntityId(int entityId)
        {
            var invoiceDues = await _invoiceService.GetInvoicesWithBalanceByEntityId(entityId);
            return Ok(invoiceDues);
        }

        [HttpPost("UpdatePaperInvoice")]
        public async Task<ActionResult<Invoice>> UpdatePaperInvoice(PaperInvoiceModel model)
        {
            bool response = false;

            try
            {
                response = await _paperInvoiceService.UpdatePaperInvoice(model);
                if (!response)
                {
                    return BadRequest(new ApiResponse(304, "Record could not be updated."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [EnableCors("_allowSpecificOrigins")]
        [HttpDelete("DeletePaperInvoice/{paperInvoiceId}")]
        public async Task<ActionResult<bool>> DeletePaperInvoice(int paperInvoiceId)
        {
            bool response = false;
            try
            {
                response = await _paperInvoiceService.DeletePaperInvoice(paperInvoiceId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete paper invoice" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("CreateWriteOff")]
        public async Task<ActionResult<WriteOffModel>> CreateWriteOff([FromBody] WriteOffModel model)
        {
            Writeoff response = new Writeoff();

            try
            {
                response = await _writeOffService.CreateWriteOff(model);
                if (response.WriteOffId == 0)
                {
                    return BadRequest(new { message = "Failed to create Write Off entry" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpGet("CheckInvoiceIsInCart/{invoiceId}")]
        public async Task<bool> GetShoppingCartByUserId(int invoiceId)
        {
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            var shoppingCart = await _shoppingCartService.GetShoppingCartByUserId(user.UserId);
            List<int> InvoiceIds = new List<int>();
            foreach (var shoppingCartItem in shoppingCart.ShoppingCartDetails)
            {
                var invoicedetail = await _invoiceService.GetInvoiceDetail(shoppingCartItem.ItemId??0);
                InvoiceIds.Add(invoicedetail.InvoiceId);
            }
            if (InvoiceIds.Any(x => x == invoiceId))
            {
                return true;
            }
            return false;
        }
    }
}
