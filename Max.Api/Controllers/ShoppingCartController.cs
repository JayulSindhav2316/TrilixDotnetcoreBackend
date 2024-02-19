using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System;
using Serilog;
using System.Linq;
using Newtonsoft.Json;
using Max.Core;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {

        private readonly ILogger<ShoppingCartController> _logger;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAuthNetService _authNetService;
        private readonly IStaffUserService _staffUserService;
        private readonly IEntityService _entityService;

        public ShoppingCartController(ILogger<ShoppingCartController> logger, IShoppingCartService shoppingCartService, IAuthNetService authNetService, IStaffUserService staffUserService, IEntityService entityService)
        {
            _logger = logger;
            _shoppingCartService = shoppingCartService;
            _authNetService = authNetService;
            _staffUserService = staffUserService;
            _entityService = entityService;
        }

        [HttpGet("GetAllShoppingCarts")]
        public async Task<ActionResult<IEnumerable<ShoppingCartModel>>> GetAllShoppingCartServices()
        {
            try
            {
                var shoppingCart = await _shoppingCartService.GetAllShoppingCarts();
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = "Unable to process the request." });
            }

        }

        [HttpGet("GetShoppingCartById")]
        public async Task<ActionResult<ShoppingCartModel>> GetShoppingCartById(int cartId)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.GetShoppingCartById(cartId);
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = "Unable to process the request." });
            }

        }

        [HttpGet("GetShoppingCartByUserId")]
        public async Task<ActionResult<ShoppingCartModel>> GetShoppingCartByUserId(int userId)
        {
            var shoppingCart = await _shoppingCartService.GetShoppingCartByUserId(userId);
            if (shoppingCart == null)
            {
                return Ok(new ShoppingCartModel());
            }
            return Ok(shoppingCart);
        }

        [HttpGet("GetMemberPortalShoppingCartByEntityId")]
        public async Task<ActionResult<ShoppingCartModel>> GetMemberPortalShoppingCartByEntityId(int entityId, string userName)
        {
            var shoppingCart = await _shoppingCartService.GetMemberPortalShoppingCartByEntityId(entityId, userName);
            if (shoppingCart == null)
            {
                return Ok(new ShoppingCartModel());
            }
            return Ok(shoppingCart);
        }

        [HttpGet("DeleteShoppingCartById")]
        public async Task<ActionResult<ShoppingCartModel>> DeleteShoppingCartById(int cartId)
        {
            var shoppingCart = await _shoppingCartService.DeleteShoppingCart(cartId);
            return Ok(shoppingCart);
        }


        [HttpPost("AddInvoiceToShoppingCart")]
        public async Task<ActionResult<ShoppingCartModel>> AddInvoiceToShoppingCart([FromForm] int invoiceId)
        {
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            if (user != null)
            {
                int userId = user.UserId;
                var shoppingCart = await _shoppingCartService.AddInvoiceToShoppingCart(userId, invoiceId);
                return Ok(shoppingCart);
            }
            return BadRequest(new { message = "User not found." });
        }

        [HttpPost("AddReceiptToShoppingCart")]
        public async Task<ActionResult<ShoppingCartModel>> AddReceiptToShoppingCart([FromForm] CreateReceiptModel model)
        {

            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);

            if (cart == null) return BadRequest(new { message = "Cart not found." });
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            if (user != null)
            {
                int userId = user.UserId;
                try
                {
                    var shoppingCart = await _shoppingCartService.AddReceiptToShoppingCart(userId, model.UseCreditBalance);
                    return Ok(shoppingCart);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    return BadRequest(new { message = ex.Message.ToString() });
                }
            }
            return BadRequest(new { message = "User not found." });
        }

        [HttpPost("ProcessPayment")]
        public async Task<ActionResult<ShoppingCartModel>> ProcessPayment([FromForm] AuthNetSecureDataModel model)
        {
            _logger.LogInformation("Calling ProcessPayment");
            _logger.LogInformation("ProcessPayment request : " + JsonConvert.SerializeObject(model));
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                _logger.LogError("Shoppingcart:", messages);
            }
            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);
            if (cart == null)
            {
                _logger.LogError("Cart Not Found");
                return BadRequest(new { message = "Cart not found." });
            }
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            if (user != null)
            {
                try
                {
                    var shoppingCart = await _authNetService.ProcessAcceptPayment(model);
                    _logger.LogInformation("ProcessPayment response : " + JsonConvert.SerializeObject(shoppingCart));
                    return Ok(shoppingCart);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    return BadRequest(new { message = ex.Message.ToString() });
                }
            }
            return BadRequest(new { message = "User not found." });
        }
        [HttpPost("ProcessCheckPayment")]
        public async Task<ActionResult<ShoppingCartModel>> ProcessCheckPayment([FromForm] CheckPaymentModel model)
        {
            _logger.LogInformation("Calling ProcessCheckPayment");
            _logger.LogInformation("ProcessCheckPayment request : " + JsonConvert.SerializeObject(model));
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                _logger.LogError("Shoppingcart:", messages);
            }
            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);
            if (cart == null)
            {
                _logger.LogError("Cart Not Found");
                return BadRequest(new { message = "Cart not found." });
            }
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            if (user != null)
            {
                try
                {
                    var shoppingCart = await _shoppingCartService.ProcessCheckPayment(model);
                    _logger.LogInformation("ProcessCheckPayment response : " + JsonConvert.SerializeObject(shoppingCart));
                    return Ok(shoppingCart);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    return BadRequest(new { message = ex.Message.ToString() });
                }
            }
            return BadRequest(new { message = "User not found." });
        }

        [HttpPost("ProcessOfflinePayment")]
        public async Task<ActionResult<ShoppingCartModel>> ProcessOfflinePayment([FromForm] OffLinePaymentModel model)
        {
            _logger.LogInformation("Calling ProcessOfflinePayment");
            _logger.LogInformation("ProcessOfflinePayment request : " + JsonConvert.SerializeObject(model));
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                _logger.LogError("Shoppingcart:", messages);
            }
            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);
            if (cart == null)
            {
                _logger.LogError("Cart Not Found");
                return BadRequest(new { message = "Cart not found." });
            }
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            if (user != null)
            {
                try
                {
                    var shoppingCart = await _shoppingCartService.ProcessOffLinePayment(model);
                    _logger.LogInformation("ProcessOfflinePayment response : " + JsonConvert.SerializeObject(shoppingCart));
                    return Ok(shoppingCart);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    return BadRequest(new { message = ex.Message.ToString() });
                }
            }
            return BadRequest(new { message = "User not found." });
        }

        [HttpPost("ApplyPromoCode")]
        public async Task<ActionResult<ShoppingCartModel>> ApplyPromoCode([FromForm] CartPromoCodeModel model)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.ApplyPromoCode(model.CartId, model.PromoCode, model.Discount);
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpPost("DeletePromoCode")]
        public async Task<ActionResult<ShoppingCartModel>> DeletePromoCode([FromForm] CartPromoCodeModel model)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.DeletePromoCode(model.CartId, model.PromoCode);
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpPost("AddInvoicesToMemberPortalShoppingCart")]
        public async Task<ActionResult<ShoppingCartModel>> AddInvoicesToMemberPortalShoppingCart(MemberportalCartModel model)
        {
            _logger.LogInformation($"Adding MemberPortal Invoice: User Name: {model.MemberPortalUserId} InvoiceIds:{model.InvoiceIds}");
            //var user = await _staffUserService.GetStaffUserByName("memberportal");
            try
            {
                var shoppingCart = await _shoppingCartService.AddInvoicesToMemberPortalShoppingCart(model.MemberPortalUserId, model.InvoiceIds);
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpPost("ProcessMemberPortalPayment")]
        public async Task<ActionResult<ShoppingCartModel>> ProcessMemberPortalPayment(AuthNetSecureDataModel model)
        {
            _logger.LogInformation("Calling ProcessMemberPortalPayment");
            _logger.LogInformation("ProcessMemberPortalPayment request : " + JsonConvert.SerializeObject(model));
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                _logger.LogError("Shoppingcart:", messages);
            }
            //Check if cart exists 
            try
            {
                _logger.LogInformation("Shoppingcart Id:", model.CartId);
                var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);
                if (cart == null)
                {
                    _logger.LogError("Cart Not Found");
                    return BadRequest(new { message = "Cart not found." });
                }
                else
                {
                    //Generate receipt
                    var shoppingCartReceipt = await _shoppingCartService.AddMemberPortalReceiptToShoppingCart(cart.ShoppingCartId, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }


            try
            {
                var shoppingCart = await _authNetService.ProcessAcceptPayment(model);
                _logger.LogInformation("ProcessMemberPortalPayment response : " + JsonConvert.SerializeObject(shoppingCart));
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }

        }

        [HttpPost("AddMemberPortalReceiptToShoppingCart")]
        public async Task<ActionResult<ShoppingCartModel>> AddMemberPortalReceiptToShoppingCart(CreateReceiptModel model)
        {

            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);

            if (cart == null) return BadRequest(new { message = "Cart not found." });
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            if (user != null)
            {
                int userId = user.UserId;
                try
                {
                    var shoppingCart = await _shoppingCartService.AddReceiptToShoppingCart(userId, model.UseCreditBalance);
                    return Ok(shoppingCart);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    return BadRequest(new { message = ex.Message.ToString() });
                }
            }
            return BadRequest(new { message = "User not found." });
        }
        [HttpPost("UpdateShoppingCartItem")]
        public async Task<ActionResult<ShoppingCartDetailModel>> UpdateShoppingCartItem([FromForm] ShoppingCartDetailModel model)
        {

            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.ShoppingCartId ?? 0);

            if (cart == null) return BadRequest(new { message = "Cart not found." });
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            if (user != null)
            {
                int userId = user.UserId;
                try
                {
                    var shoppingCartItem = await _shoppingCartService.UpdateShoppingCartItem(model);
                    return Ok(shoppingCartItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    return BadRequest(new { message = ex.Message.ToString() });
                }
            }
            return BadRequest(new { message = "User not found." });
        }

        [HttpPost("DeleteMemberPortalShoppingCartItem")]
        public async Task<ActionResult<ShoppingCartModel>> DeleteMemberPortalShoppingCartItem(ShoppingCartDetailModel model)
        {

            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.ShoppingCartId ?? 0);

            if (cart == null) return BadRequest(new { message = "Cart not found." });

            if (cart.MemberPortalUser != null)
            {
                var memberPortaluser = await _entityService.GetEntityByWebLogin(cart.MemberPortalUser);

                if (memberPortaluser == null)
                {
                    return BadRequest(new { message = "Cart not found." });
                }

            }

            try
            {
                var shoppingCartItem = await _shoppingCartService.DeleteShoppingCartItem(model);
                return Ok(shoppingCartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpPost("DeleteShoppingCartItem")]
        public async Task<ActionResult<ShoppingCartModel>> DeleteShoppingCartItem([FromForm] ShoppingCartDetailModel model)
        {

            //Check if cart exists 
            var cart = await _shoppingCartService.GetShoppingCartById(model.ShoppingCartId ?? 0);

            if (cart == null) return BadRequest(new { message = "Cart not found." });

            if (cart.MemberPortalUser != null)
            {
                var memberPortaluser = _entityService.GetEntityByWebLogin(cart.MemberPortalUser);

                if (memberPortaluser == null)
                {
                    return BadRequest(new { message = "Cart not found." });
                }

            }
            else
            {
                var staffUser = (Staffuser)HttpContext.Items["StafffUser"];
                if (staffUser == null)
                {
                    return BadRequest(new { message = "Cart not found." });
                }
            }
            
            try
            {
                var shoppingCartItem = await _shoppingCartService.DeleteShoppingCartItem(model);
                return Ok(shoppingCartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpPost("ProcessPaymentProfilePayment")]
        public async Task<ActionResult<ShoppingCartModel>> ProcessPaymentProfilePayment([FromForm] AuthNetChargePaymentProfileRequestModel model)
        {
            _logger.LogInformation("Calling ProcessPaymentProfilePayment");
            _logger.LogInformation("ProcessPaymentProfilePayment request : " + JsonConvert.SerializeObject(model));
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                _logger.LogError("Shoppingcart:", messages);
            }
            //Check if cart exists 
            try
            {
                _logger.LogInformation("Shoppingcart Id:", model.CartId);
                var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);
                if (cart == null)
                {
                    _logger.LogError("Cart Not Found");
                    return BadRequest(new { message = "Cart not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }

            try
            {
                var shoppingCart = await _authNetService.ChargePaymentProfile(model);
                _logger.LogInformation("ProcessPaymentProfilePayment response : " + JsonConvert.SerializeObject(shoppingCart));
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }

        }
        [HttpPost("ProcessMemberPortalProfilePayment")]
        public async Task<ActionResult<ShoppingCartModel>> ProcessMemberPortalPaymentProfilePayment(AuthNetChargePaymentProfileRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                _logger.LogError("Shoppingcart:", messages);
            }
            //Check if cart exists 
            try
            {
                _logger.LogInformation("Shoppingcart Id:", model.CartId);
                var cart = await _shoppingCartService.GetShoppingCartById(model.CartId);
                if (cart == null)
                {
                    _logger.LogError("Cart Not Found");
                    return BadRequest(new { message = "Cart not found." });
                }
                else
                {
                    //Generate receipt
                    var shoppingCartReceipt = await _shoppingCartService.AddMemberPortalReceiptToShoppingCart(cart.ShoppingCartId, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }

            try
            {
                var shoppingCart = await _authNetService.ChargePaymentProfile(model);
                return Ok(shoppingCart);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message.ToString() });
            }

        }

        [HttpPost("UpdateInvoiceDetails")]
        public async Task<ActionResult<bool>> UpdateInvoiceDetails([FromForm] CartPromoCodeModel model)
        {
            try
            { 
                var result = await _shoppingCartService.UpdateInvoiceDetails(model.CartId, model.PromoCode, model.Discount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error", ex);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("GetOfflinePaymentTypes")]
        public ActionResult<List<string>> GetOfflinePaymentTypes()
        {
            try
            {
                var result = _shoppingCartService.GetOfflinePaymentTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error", ex);
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
    }
}

