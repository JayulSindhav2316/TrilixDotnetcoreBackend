using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using AutoMapper;
using System.Linq;
using static Max.Core.Constants;
using Max.Core.Helpers;
using Serilog;
using static Twilio.Rest.Proxy.V1.Service.SessionResource;
using System.ComponentModel;
using System.Reflection;

namespace Max.Services
{
    public class ShoppingCartService : IShoppingCartService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly ITransactionService _transactionService;
        private readonly IEntityService _entityService;
        private readonly IStaffUserService _staffUserService;
        static readonly ILogger _logger = Serilog.Log.ForContext<ShoppingCartService>();
        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper,
                                    IPaymentTransactionService paymentTransactionService,
                                    IEntityService entityService,
                                    ITransactionService transactionService, IStaffUserService staffUserService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._paymentTransactionService = paymentTransactionService;
            this._transactionService = transactionService;
            this._entityService = entityService;
            this._staffUserService = staffUserService;
        }

        public async Task<IEnumerable<ShoppingCartModel>> GetAllShoppingCarts()
        {
            var carts = await _unitOfWork.ShoppingCarts
                         .GetAllShoppingCartsAsync();
            return _mapper.Map<List<ShoppingCartModel>>(carts);
        }

        public async Task<ShoppingCartModel> GetShoppingCartById(int id)
        {
            var cart = await _unitOfWork.ShoppingCarts
                            .GetShoppingCartByIdAsync(id);
            var shoppingCartModel = _mapper.Map<ShoppingCartModel>(cart);
            if (shoppingCartModel != null)
            {
                if (shoppingCartModel.PromoCodeId > 0)
                {
                    var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByIdAsync(shoppingCartModel.PromoCodeId);
                    if (promoCode != null)
                    {
                        shoppingCartModel.PromoCode = promoCode.Code;
                    }
                }
            }
            return shoppingCartModel;
        }

        public async Task<ShoppingCartModel> GetShoppingCartByUserId(int id)
        {
            var cart = await _unitOfWork.ShoppingCarts
                            .GetShoppingCartByUserIdAsync(id);



            if (cart != null)
            {
                //Check if billable member has Credit Balance
                cart.CreditBalance = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(cart.EntityId ?? 0);

                var shoppingCartModel = _mapper.Map<ShoppingCartModel>(cart);

                if (shoppingCartModel.PromoCodeId > 0)
                {
                    var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByIdAsync(shoppingCartModel.PromoCodeId);
                    if (promoCode != null)
                    {
                        shoppingCartModel.PromoCode = promoCode.Code;
                    }
                }

                if (cart.EntityId > 0)
                {
                    var entity = await _unitOfWork.Entities.GetByIdAsync(cart.EntityId ?? 0);
                    if (entity != null)
                    {
                        shoppingCartModel.BillableEntityName = entity.Name;
                    }
                }
                return shoppingCartModel;
            }
            return new ShoppingCartModel();
        }

        public async Task<ShoppingCartModel> GetMemberPortalShoppingCartByEntityId(int entityId, string userName)
        {
            var entity = await _entityService.GetEntityByWebLogin(userName);

            if (entity != null)
            {
                var cart = await _unitOfWork.ShoppingCarts.GetMemberPortalShoppingCartByEntityIdAsync(entityId, userName);
                var shoppingCartModel = _mapper.Map<ShoppingCartModel>(cart);
                if (shoppingCartModel != null)
                {
                    if (shoppingCartModel.PromoCodeId > 0)
                    {
                        var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByIdAsync(shoppingCartModel.PromoCodeId);
                        if (promoCode != null)
                        {
                            shoppingCartModel.PromoCode = promoCode.Code;
                        }
                    }
                }
                return shoppingCartModel;
            }
            throw new Exception("Shopping cart not found.");
        }
        public async Task<ShoppingCartModel> CreateShoppingCart(ShoppingCartModel model)
        {
            var isValid = ValidShoppingCart(model);
            if (isValid)
            {
                var shoppingCart = _mapper.Map<Shoppingcart>(model);

                await _unitOfWork.ShoppingCarts.AddAsync(shoppingCart);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<ShoppingCartModel>(shoppingCart);
            }
            return new ShoppingCartModel();
        }

        public async Task<bool> DeleteShoppingCart(int shoppingCartId)
        {
            var cart = await _unitOfWork.ShoppingCarts
                            .GetShoppingCartByIdAsync(shoppingCartId);
            if (cart != null && cart.PaymentStatus != (int)PaymentTransactionStatus.Approved)
            {

                _unitOfWork.ShoppingCartDetails.RemoveRange(cart.Shoppingcartdetails);
                _unitOfWork.ShoppingCarts.Remove(cart);
                try
                {
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Shopping Cart can not be deleted. There could be pending payments on them.");
                }
            }
            else
            {
                throw new InvalidOperationException("Shopping Cart can not be deleted. There could be pending payments on them.");
            }
            return true;
        }

        private bool ValidShoppingCart(ShoppingCartModel model)
        {
            return true;
        }

        public Task<ShoppingCartModel> AddItemToShoppingCart(ShoppingCartDetailModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateShoppingCartPaymentStatus(int userId, int cartId, int status, decimal creditUsed, string paymentMode)
        {
            var shoppingCart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(cartId);

            //check if CartId is valid

            if (shoppingCart.ShoppingCartId == cartId && shoppingCart.UserId == userId)
            {
                shoppingCart.PaymentStatus = status;
                shoppingCart.CreditBalance = creditUsed;
                shoppingCart.PaymentMode = paymentMode;
                try
                {
                    _unitOfWork.ShoppingCarts.Update(shoppingCart);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return true;
            }
            return false;
        }

        public async Task<ShoppingCartModel> AddInvoiceToShoppingCart(int userId, int invoiceId)
        {
            ShoppingCartModel model = new ShoppingCartModel();
            var hasItems = true;

            var invoice = await _unitOfWork.Invoices.GetInvoicePaymentsByIdAsync(invoiceId);
            //var invoiceModel = _mapper.Map<InvoiceModel>(invoice);

            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByUserIdAsync(userId);

            if (cart == null)
            {
                //Add Cart
                cart = new Shoppingcart();

                cart.UserId = userId;
                cart.TransactionDate = DateTime.Now;
                cart.PaymentStatus = 0;
                cart.EntityId = invoice.BillableEntityId;
                hasItems = false;
            }
            else
            {
                if (invoice.BillableEntityId != cart.EntityId)
                {
                    throw new InvalidOperationException("Billable member must be same for all items in cart!!!.");
                }
            }


            foreach (var invoiceItem in invoice.Invoicedetails)
            {
                if (!cart.Shoppingcartdetails.Any(x => x.ItemId == invoiceItem.InvoiceDetailId))
                {
                    Shoppingcartdetail item = new Shoppingcartdetail();
                    item.ShoppingCartId = cart.ShoppingCartId;
                    item.ItemId = invoiceItem.InvoiceDetailId;
                    item.Description = invoiceItem.Description;
                    item.ItemType = invoiceItem.ItemType;
                    item.Price = invoiceItem.Price;
                    item.Quantity = invoiceItem.Quantity;
                    if (item.Quantity == 0)
                    {
                        item.Quantity = 1;
                    }
                    item.Discount = invoiceItem.Discount;
                    decimal amount = 0;
                    amount = (invoiceItem.Price * item.Quantity) - invoiceItem.Discount;
                    var paid = invoiceItem.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created && x.Status != (int)ReceiptStatus.Void).Sum(x => x.Amount);
                    var writeOff = invoiceItem.Writeoffs.Sum(x => x.Amount) ?? 0;
                    var balance = amount - paid - writeOff;
                    item.Amount = balance;
                    if (invoice.InvoiceItemType != (int)InvoiceItemType.Marketplace)
                    {
                        if (invoiceItem.ItemType == (int)InvoiceItemType.Membership)
                        {
                            item.ItemGroup = "Membership Payment";
                            item.ItemGroupDescription = $"{invoice.Membership.MembershipType.Code}:{invoice.Membership.MembershipType.Name}";
                        }
                        else if (invoiceItem.ItemType == (int)InvoiceItemType.Event)
                        {
                            item.ItemGroup = "Event Registration Payment";
                            item.ItemGroupDescription = $"{invoice.Event.Name} - {(invoice.Event.EventTypeId == 1 ? "In-Person" : invoice.Event.EventTypeId == 2 ? "Virtual" : "Pre Recorded")}";
                        }
                    }
                    else
                    {
                        item.ItemGroup = EnumUtil.GetDescription(((InvoiceItemType)invoiceItem.ItemType));
                        item.ItemGroupDescription = EnumUtil.GetDescription(((InvoiceItemType)invoiceItem.ItemType));
                    }
                    if (balance > 0)
                    {
                        cart.Shoppingcartdetails.Add(item);
                    }
                }

            }
            if (hasItems)
            {
                _unitOfWork.ShoppingCarts.Update(cart);
            }
            else
            {
                await _unitOfWork.ShoppingCarts.AddAsync(cart);
            }
            await _unitOfWork.CommitAsync();
            model = _mapper.Map<ShoppingCartModel>(cart);
            return model;
        }

        public async Task<ShoppingCartModel> AddReceiptToShoppingCart(int userId, int useCreditBalance)
        {
            // Get Shopping Cart by User Id. We create receipt when User selectes make Payment from checkout
            //   So cart  should already be there.
            decimal creditBalance = 0;
            var user = await _unitOfWork.Staffusers.GetByIdAsync(userId);
            var organization = user?.Organization;

            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByUserIdAsync(userId);

            if (useCreditBalance > 0)
            {
                creditBalance = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(cart.EntityId ?? 0);
            }
            if (cart != null)
            {
                //check if cart already has a receipt
                if (cart.ReceiptId > 0)
                {
                    var cartReceipt = await _unitOfWork.ReceiptHeaders.GetReceiptItemDetailById(cart.ReceiptId ?? 0);

                    if (cartReceipt != null)
                    {
                        //User might have alreday created/tried the payment
                        // Check if status is not Approved

                        if (cart.Receipt.Status == (int)ReceiptStatus.Active)
                        {
                            return new ShoppingCartModel();
                        }
                        //Inactive receipt could allready have items added so check
                        foreach (var cartItem in cart.Shoppingcartdetails)
                        {
                            if (cartReceipt.Receiptdetails.Any(x => x.InvoiceDetailId == cartItem.ItemId))
                            {
                                continue;
                            }

                            //New Item so add to the receipt
                            var receiptItem = new Receiptdetail();
                            receiptItem.InvoiceDetailId = cartItem.ItemId;
                            receiptItem.Description = cartItem.Description;
                            receiptItem.ItemType = cartItem.ItemType;
                            receiptItem.Rate = cartItem.Price;
                            receiptItem.Quantity = cartItem.Quantity;
                            receiptItem.Tax = cartItem.Tax;
                            receiptItem.Discount = cartItem.Discount;
                            receiptItem.Amount = cartItem.Amount;
                            receiptItem.Status = cartReceipt.Status;
                            receiptItem.EntityId = cart.EntityId;
                            cartReceipt.Receiptdetails.Add(receiptItem);
                        }
                        try
                        {
                            if (cart.UseCreditBalance != useCreditBalance)
                            {
                                cart.UseCreditBalance = useCreditBalance;
                            }
                            if (useCreditBalance > 0 & creditBalance >= cart.Shoppingcartdetails.Sum(x => x.Amount))
                            {
                                await _paymentTransactionService.ProcessCreditPaymentTransaction(cart.ShoppingCartId);
                                cart.PaymentStatus = (int)PaymentTransactionStatus.Approved;
                                cart.CreditBalance = cart.Shoppingcartdetails.Sum(x => x.Amount);
                            }

                            cartReceipt.PromoCodeId = cart.PromoCodeId;

                            _unitOfWork.ShoppingCarts.Update(cart);
                            _unitOfWork.ReceiptHeaders.Update(cartReceipt);
                            await _unitOfWork.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException("Failed to add new Items to the cart.");
                        }

                        if (cart.PaymentStatus != (int)PaymentTransactionStatus.Approved)
                        {
                            //Get updated cart
                            cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByUserIdAsync(userId);
                        }

                    }

                    var model = new ShoppingCartModel();
                    model = _mapper.Map<ShoppingCartModel>(cart);
                    return model;
                }
                else
                {
                    //Add Receipt to Cart
                    var receipt = new Receiptheader();
                    int receiptStatus = (int)ReceiptStatus.Created;
                    receipt.Date = DateTime.Now;
                    receipt.CheckNo = String.Empty;
                    receipt.PaymentMode = String.Empty;
                    receipt.StaffId = cart.UserId;
                    receipt.OrganizationId = cart.User.OrganizationId;
                    receipt.Status = receiptStatus;
                    receipt.PromoCodeId = cart.PromoCodeId;
                    receipt.BillableEntityId = cart.EntityId;

                    receipt.Notes = organization?.PrintMessage ?? null;

                    if (user.UserName == "memberportal")
                    {
                        receipt.Portal = (int)Portal.MemberPortal;
                    }
                    // Add item details to receipt
                    foreach (var item in cart.Shoppingcartdetails)
                    {
                        var receiptDetail = new Receiptdetail();
                        receiptDetail.InvoiceDetailId = item.ItemId;
                        receiptDetail.Description = item.Description;
                        receiptDetail.ItemType = item.ItemType;
                        receiptDetail.Rate = item.Price;
                        receiptDetail.Quantity = item.Quantity;
                        receiptDetail.Tax = item.Tax;
                        receiptDetail.Amount = item.Amount;
                        receiptDetail.Discount = item.Discount;
                        receiptDetail.Status = receiptStatus;
                        receiptDetail.EntityId = cart.EntityId;
                        receipt.Receiptdetails.Add(receiptDetail);
                    }
                    try
                    {
                        await _unitOfWork.ReceiptHeaders.AddAsync(receipt);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to add Items to the cart.");
                    }

                    //Add Receipt to Cart
                    cart.Receipt = receipt;
                    cart.UseCreditBalance = useCreditBalance;
                    _unitOfWork.ShoppingCarts.Update(cart);
                    await _unitOfWork.CommitAsync();

                    if (useCreditBalance > 0 & creditBalance >= cart.Shoppingcartdetails.Sum(x => x.Amount))
                    {
                        await _paymentTransactionService.ProcessCreditPaymentTransaction(cart.ShoppingCartId);
                        cart.PaymentStatus = (int)PaymentTransactionStatus.Approved;
                        cart.CreditBalance = cart.Shoppingcartdetails.Sum(x => x.Amount);
                        _unitOfWork.ShoppingCarts.Update(cart);
                        await _unitOfWork.CommitAsync();
                    }

                    var model = new ShoppingCartModel();
                    model = _mapper.Map<ShoppingCartModel>(cart);
                    return model;
                }
            }
            return new ShoppingCartModel();
        }

        public async Task<bool> ProcessCheckPayment(CheckPaymentModel model)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(model.CartId);

            decimal creditBalance = 0;
            decimal paymentAmount = cart.Shoppingcartdetails.Sum(x => x.Amount);
            if (cart.UseCreditBalance > 0)
            {
                creditBalance = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(model.EntityId);
                paymentAmount = paymentAmount - creditBalance;
            }

            // Create PaymentRecord

            PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
            paymentTransaction.IsOfflinePayment = false;
            paymentTransaction.TransactionDate = model.TransactionDate;
            paymentTransaction.ReceiptId = cart.ReceiptId;
            paymentTransaction.EntityId = cart.EntityId;
            paymentTransaction.ShoppingCartId = cart.ShoppingCartId;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
            paymentTransaction.Amount = paymentAmount;
            paymentTransaction.CreditBalanceUsed = creditBalance;
            paymentTransaction.PaymentType = PaymentType.CHECK;
            paymentTransaction.TransactionId = string.Empty;
            paymentTransaction.ResponseDetails = string.Empty;
            paymentTransaction.MessageDetails = string.Empty;
            paymentTransaction.ResponseCode = string.Empty;
            paymentTransaction.AuthCode = string.Empty;
            paymentTransaction.AccountNumber = model.CheckNumber;
            paymentTransaction.CardType = string.Empty;
            paymentTransaction.AccountHolderName = model.AccountHolderName;
            paymentTransaction.AccountNumber = model.AccountNumber;
            paymentTransaction.AccountType = model.AccountType;
            paymentTransaction.BankName = model.BankName;
            paymentTransaction.Result = 1;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;

            var paymentModel = await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);

            cart.PaymentStatus = (int)PaymentTransactionStatus.Approved;
            cart.CreditBalance = creditBalance;
            cart.PaymentMode = PaymentType.CHECK;
            try
            {
                _unitOfWork.ShoppingCarts.Update(cart);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var result = await _transactionService.UpdateTransactionStatus(paymentTransaction);

            return result;
        }

        public async Task<bool> ProcessOffLinePayment(OffLinePaymentModel model)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(model.CartId);

            decimal creditBalance = 0;
            decimal paymentAmount = cart.Shoppingcartdetails.Sum(x => x.Amount);

            if (cart.UseCreditBalance > 0)
            {
                throw new InvalidOperationException("You can not use credits in offline payment.");
            }

            if (cart.PromoCodeId > 0)
            {
                throw new InvalidOperationException("You can not use discounts in offline payment.");
            }

            // Create PaymentRecord

            PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
            paymentTransaction.IsOfflinePayment = true;
            paymentTransaction.TransactionDate = model.TransactionDate;
            paymentTransaction.ReceiptId = cart.ReceiptId;
            paymentTransaction.EntityId = cart.EntityId;
            paymentTransaction.ShoppingCartId = cart.ShoppingCartId;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
            paymentTransaction.Amount = paymentAmount;
            paymentTransaction.CreditBalanceUsed = creditBalance;
            paymentTransaction.PaymentType = PaymentType.OFFLINE;
            paymentTransaction.TransactionId = string.Empty;
            paymentTransaction.ResponseDetails = string.Empty;
            paymentTransaction.MessageDetails = string.Empty;
            paymentTransaction.ResponseCode = string.Empty;
            paymentTransaction.AuthCode = string.Empty;
            paymentTransaction.AccountNumber = model.ReferenceNumber;
            paymentTransaction.CardType = string.Empty;
            paymentTransaction.AccountHolderName = model.PayerName;
            paymentTransaction.AccountType = string.Empty;
            paymentTransaction.BankName = string.Empty;
            paymentTransaction.Result = 1;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
            paymentTransaction.OfflinePaymentType = model.PaymentType;

            var paymentModel = await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);


            cart.PaymentStatus = (int)PaymentTransactionStatus.Approved;
            cart.CreditBalance = creditBalance;
            cart.PaymentMode = PaymentType.OFFLINE;
            try
            {
                _unitOfWork.ShoppingCarts.Update(cart);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var result = await _transactionService.UpdateTransactionStatus(paymentTransaction);

            return result;
        }
        public async Task<ShoppingCartModel> ApplyPromoCode(int id, string code, decimal superDiscount)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(id);

            if (cart != null)
            {
                if (cart.PromoCodeId > 0)
                {
                    throw new InvalidOperationException("Promo Code has already been applied.");
                }
            }
            var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByCodeAsync(code);

            if (promoCode == null)
            {
                throw new InvalidOperationException("Invalid Promo Code.");
            }
            if (promoCode.Status != (int)Status.Active)
            {
                throw new InvalidOperationException("This Promo code is no longer Active.");
            }
            if (promoCode.ExpirationDate < DateTime.Now.Date)
            {
                throw new InvalidOperationException("This Promo code has expired.");
            }

            var promoCodeModel = _mapper.Map<PromoCodeModel>(promoCode);

            if (cart != null)
            {
                cart.PromoCodeId = promoCode.PromoCodeId;
                decimal discountApplied = 0;
                foreach (var cartItem in cart.Shoppingcartdetails)
                {
                    promoCodeModel.DiscountApplied = discountApplied;
                    cartItem.Discount = GetPromoCodeDiscount(promoCodeModel, cartItem.Amount, superDiscount);
                    //cartItem.Amount = (cartItem.Price * cartItem.Quantity) -cartItem.Discount;
                    cartItem.Amount = cartItem.Amount - cartItem.Discount;
                    discountApplied += cartItem.Discount;
                }
                //Update Receipt
                if (cart.ReceiptId > 0)
                {
                    var cartReceipt = await _unitOfWork.ReceiptHeaders.GetReceiptItemDetailById(cart.ReceiptId ?? 0);

                    if (cartReceipt != null)
                    {
                        discountApplied = 0;
                        foreach (var cartReceiptItem in cartReceipt.Receiptdetails)
                        {
                            promoCodeModel.DiscountApplied = discountApplied;
                            cartReceiptItem.Discount = GetPromoCodeDiscount(promoCodeModel, cartReceiptItem.Amount, superDiscount);
                            cartReceiptItem.Amount = (cartReceiptItem.Rate * cartReceiptItem.Quantity ?? 0) - cartReceiptItem.Discount;
                            discountApplied += cartReceiptItem.Discount;
                        }
                        cartReceipt.PromoCodeId = cart.PromoCodeId;
                        _unitOfWork.ReceiptHeaders.Update(cartReceipt);
                    }
                }
                //Update Invoice Details
                //int invoiceId = 0;
                //discountApplied = 0;
                //foreach (var cartItem in cart.Shoppingcartdetails)
                //{
                //    var invoiceDetail = await _unitOfWork.InvoiceDetails.GetByIdAsync(cartItem.ItemId??0);
                //    if(invoiceDetail != null)
                //    {
                //        promoCodeModel.DiscountApplied = discountApplied;
                //        invoiceDetail.Discount = GetPromoCodeDiscount(promoCodeModel, invoiceDetail.Amount, superDiscount);
                //        invoiceDetail.Amount = (invoiceDetail.Price * invoiceDetail.Quantity) - invoiceDetail.Discount;
                //        discountApplied += invoiceDetail.Discount;
                //        _unitOfWork.InvoiceDetails.Update(invoiceDetail);

                //        if(invoiceId != invoiceDetail.InvoiceId)
                //        {
                //            invoiceId = invoiceDetail.InvoiceId;

                //            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
                //            if (invoice != null)
                //            {
                //                invoice.PromoCodeId = cart.PromoCodeId;
                //                _unitOfWork.Invoices.Update(invoice);
                //            }
                //        }
                //    }
                //}

                try
                {
                    _unitOfWork.ShoppingCarts.Update(cart);
                    await _unitOfWork.CommitAsync();
                    var shoppongCart = _mapper.Map<ShoppingCartModel>(cart);
                    //Check if billable member has Credit Balance
                    shoppongCart.CreditBalance = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(cart.EntityId ?? 0);
                    shoppongCart.PromoCode = promoCode.Code;
                    return shoppongCart;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new ShoppingCartModel();
        }

        public async Task<ShoppingCartModel> DeletePromoCode(int id, string code)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(id);

            if (cart != null)
            {
                if (cart.PromoCodeId == 0)
                {
                    throw new InvalidOperationException("Promo Code has note been applied.");
                }
                cart.PromoCodeId = 0;
            }

            if (cart != null)
            {

                foreach (var cartItem in cart.Shoppingcartdetails)
                {
                    cartItem.Discount = 0;
                    cartItem.Amount = cartItem.Price * cartItem.Quantity;
                }
                //Update Receipt
                if (cart.ReceiptId > 0)
                {
                    var cartReceipt = await _unitOfWork.ReceiptHeaders.GetReceiptItemDetailById(cart.ReceiptId ?? 0);

                    if (cartReceipt != null)
                    {
                        foreach (var cartReceiptItem in cartReceipt.Receiptdetails)
                        {
                            cartReceiptItem.Discount = 0;
                            cartReceiptItem.Amount = cartReceiptItem.Rate * cartReceiptItem.Quantity ?? 0;
                        }
                        cartReceipt.PromoCodeId = cart.PromoCodeId;
                        _unitOfWork.ReceiptHeaders.Update(cartReceipt);
                    }
                }
                //Update Invoice Details
                //int invoiceId = 0;
                //foreach (var cartItem in cart.Shoppingcartdetails)
                //{
                //    var invoiceDetail = await _unitOfWork.InvoiceDetails.GetByIdAsync(cartItem.ItemId ?? 0);
                //    if (invoiceDetail != null)
                //    {

                //        invoiceDetail.Discount = 0;
                //        invoiceDetail.Amount = invoiceDetail.Price * invoiceDetail.Quantity;
                //        _unitOfWork.InvoiceDetails.Update(invoiceDetail);

                //        if (invoiceId != invoiceDetail.InvoiceId)
                //        {
                //            invoiceId = invoiceDetail.InvoiceId;

                //            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
                //            if (invoice != null)
                //            {
                //                invoice.PromoCodeId = cart.PromoCodeId;
                //                _unitOfWork.Invoices.Update(invoice);
                //            }
                //        }
                //    }
                //}

                try
                {
                    _unitOfWork.ShoppingCarts.Update(cart);
                    await _unitOfWork.CommitAsync();
                    var shoppongCart = _mapper.Map<ShoppingCartModel>(cart);
                    shoppongCart.PromoCode = string.Empty;
                    return shoppongCart;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new ShoppingCartModel();
        }

        public async Task<ShoppingCartModel> AddMemberPortalInvoiceToShoppingCart(int invoiceId)
        {
            ShoppingCartModel model = new ShoppingCartModel();

            var invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(invoiceId);
            var invoiceModel = _mapper.Map<InvoiceModel>(invoice);
            var billingEmail = await _unitOfWork.BillingEmails.GetBillingEmailByInvoiceIdAsync(invoiceId);
            var billingUser = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync("BillingService");
            var itemTypes = await _unitOfWork.ItemTypes.GetAllItemTypesAsync();
            if (billingUser == null)
            {
                throw new Exception("An internal error has occurred. Please contact support team.");
            }
            //Add Cart
            var cart = new Shoppingcart();

            cart.UserId = billingUser.UserId;
            cart.TransactionDate = DateTime.Now;
            cart.PaymentStatus = 0;
            cart.EntityId = invoice.BillableEntityId;
            cart.UseCreditBalance = 0;
            foreach (var invoiceItem in invoiceModel.InvoiceDetails)
            {
                //Check if payment has been made

                var balance = _unitOfWork.InvoiceDetails.GetInvoiceItemBalanceById(invoiceItem.InvoiceDetailId);

                if (balance > 0)
                {
                    if (!cart.Shoppingcartdetails.Any(x => x.ItemId == invoiceItem.InvoiceDetailId))
                    {
                        Shoppingcartdetail item = new Shoppingcartdetail();
                        item.ShoppingCartId = cart.ShoppingCartId;
                        item.ItemId = invoiceItem.InvoiceDetailId;
                        item.Description = invoiceItem.Description;
                        item.ItemType = invoiceItem.ItemType;
                        item.Price = invoiceItem.Price;
                        item.Quantity = invoiceItem.Quantity;
                        item.Discount = invoiceItem.Discount;
                        item.Amount = balance;

                        if (invoice.InvoiceItemType != (int)InvoiceItemType.Marketplace)
                        {
                            if (invoiceItem.ItemType == (int)InvoiceItemType.Membership)
                            {
                                item.ItemGroup = "Membership Payment";
                                item.ItemGroupDescription = $"{invoice.Membership.MembershipType.Code}:{invoice.Membership.MembershipType.Name}";
                            }
                        }
                        else
                        {
                            item.ItemGroup = EnumUtil.GetDescription(((InvoiceItemType)invoiceItem.ItemType));
                            item.ItemGroupDescription = EnumUtil.GetDescription(((InvoiceItemType)invoiceItem.ItemType));
                        }

                        cart.Shoppingcartdetails.Add(item);
                    }
                }

            }

            await _unitOfWork.ShoppingCarts.AddAsync(cart);

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to add member portal invoice to the cart.");
            }

            billingEmail.CartId = cart.ShoppingCartId;
            _unitOfWork.BillingEmails.Update(billingEmail);

            await _unitOfWork.CommitAsync();
            model = _mapper.Map<ShoppingCartModel>(cart);
            return model;
        }
        public async Task<SelfPaymentResponseModel> AddMemberPortalReceiptToShoppingCart(int cartId, int useCreditBalance)
        {
            decimal creditBalance = 0;
            var paymentResponse = new SelfPaymentResponseModel();

            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(cartId);
            var user = await _staffUserService.GetStaffUserByName("memberportal");

            if (useCreditBalance > 0)
            {
                creditBalance = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(cart.EntityId ?? 0);
            }
            if (cart != null)
            {
                //check if cart already has a receipt
                if (cart.ReceiptId > 0)
                {
                    var cartReceipt = await _unitOfWork.ReceiptHeaders.GetReceiptItemDetailById(cart.ReceiptId ?? 0);

                    if (cartReceipt != null)
                    {
                        //User might have alreday created/tried the payment
                        // Check if status is not Approved

                        if (cart.Receipt.Status == (int)ReceiptStatus.Active)
                        {
                            return new SelfPaymentResponseModel();
                        }
                        //Inactive receipt could allready have items added so check
                        foreach (var cartItem in cart.Shoppingcartdetails)
                        {
                            if (cartReceipt.Receiptdetails.Any(x => x.InvoiceDetailId == cartItem.ItemId))
                            {
                                continue;
                            }

                            //New Item so add to the receipt
                            var receiptItem = new Receiptdetail();
                            receiptItem.InvoiceDetailId = cartItem.ItemId;
                            receiptItem.Description = cartItem.Description;
                            receiptItem.ItemType = cartItem.ItemType;
                            receiptItem.Rate = cartItem.Price;
                            receiptItem.Quantity = cartItem.Quantity;
                            receiptItem.Tax = cartItem.Tax;
                            receiptItem.Discount = cartItem.Discount;
                            receiptItem.Amount = cartItem.Amount;
                            receiptItem.Status = cartReceipt.Status;
                            receiptItem.EntityId = cart.EntityId;
                            cartReceipt.Receiptdetails.Add(receiptItem);
                        }
                        try
                        {
                            if (cart.UseCreditBalance != useCreditBalance)
                            {
                                cart.UseCreditBalance = useCreditBalance;
                            }
                            if (useCreditBalance > 0 & creditBalance >= cart.Shoppingcartdetails.Sum(x => x.Amount))
                            {
                                await _paymentTransactionService.ProcessCreditPaymentTransaction(cart.ShoppingCartId);
                                cart.PaymentStatus = (int)PaymentTransactionStatus.Approved;
                                cart.CreditBalance = cart.Shoppingcartdetails.Sum(x => x.Amount);
                            }

                            cartReceipt.PromoCodeId = cart.PromoCodeId;

                            _unitOfWork.ShoppingCarts.Update(cart);
                            _unitOfWork.ReceiptHeaders.Update(cartReceipt);
                            await _unitOfWork.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException("Failed to add new Items to the cart.");
                        }


                        //Get updated cart
                        cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(cartId);

                    }


                    paymentResponse.ShoppingCart = _mapper.Map<ShoppingCartModel>(cart);
                    var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(cart.Receipt.OrganizationId ?? 0);
                    var billingAddress = await _entityService.GetBillingAddressByEntityId(cart.EntityId ?? 0);
                    paymentResponse.Organization = _mapper.Map<OrganizationModel>(organization);
                    paymentResponse.BillingAddress = billingAddress;
                    return paymentResponse;
                }
                else
                {
                    //Add Receipt to Cart
                    var receipt = new Receiptheader();
                    int receiptStatus = (int)ReceiptStatus.Created;
                    receipt.Date = DateTime.Now;
                    receipt.CheckNo = String.Empty;
                    receipt.PaymentMode = String.Empty;
                    receipt.StaffId = cart.UserId;
                    receipt.OrganizationId = user.OrganizationId;
                    receipt.Status = receiptStatus;
                    receipt.PromoCodeId = cart.PromoCodeId;
                    receipt.BillableEntityId = cart.EntityId;
                    receipt.Portal = (int)Portal.MemberPortal;
                    // Add item details to receipt
                    foreach (var item in cart.Shoppingcartdetails)
                    {
                        var receiptDetail = new Receiptdetail();
                        receiptDetail.InvoiceDetailId = item.ItemId;
                        receiptDetail.Description = item.Description;
                        receiptDetail.ItemType = item.ItemType;
                        receiptDetail.Rate = item.Price;
                        receiptDetail.Quantity = item.Quantity;
                        receiptDetail.Tax = item.Tax;
                        receiptDetail.Amount = item.Amount;
                        receiptDetail.Discount = item.Discount;
                        receiptDetail.Status = receiptStatus;
                        receiptDetail.EntityId = cart.EntityId;
                        receipt.Receiptdetails.Add(receiptDetail);
                    }
                    try
                    {
                        await _unitOfWork.ReceiptHeaders.AddAsync(receipt);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to add Items to the cart.");
                    }

                    //Add Receipt to Cart
                    cart.Receipt = receipt;
                    cart.UseCreditBalance = useCreditBalance;
                    _unitOfWork.ShoppingCarts.Update(cart);
                    await _unitOfWork.CommitAsync();

                    if (useCreditBalance > 0 & creditBalance >= cart.Shoppingcartdetails.Sum(x => x.Amount))
                    {
                        await _paymentTransactionService.ProcessCreditPaymentTransaction(cart.ShoppingCartId);
                        cart.PaymentStatus = (int)PaymentTransactionStatus.Approved;
                        cart.CreditBalance = cart.Shoppingcartdetails.Sum(x => x.Amount);
                        _unitOfWork.ShoppingCarts.Update(cart);
                        await _unitOfWork.CommitAsync();
                    }

                    paymentResponse.ShoppingCart = _mapper.Map<ShoppingCartModel>(cart);
                    var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(cart.Receipt.OrganizationId ?? 0);
                    var billingAddress = await _entityService.GetBillingAddressByEntityId(cart.EntityId ?? 0);
                    paymentResponse.Organization = _mapper.Map<OrganizationModel>(organization);
                    paymentResponse.BillingAddress = billingAddress;
                    return paymentResponse;
                }
            }
            return new SelfPaymentResponseModel();
        }


        private decimal GetPromoCodeDiscount(PromoCodeModel model, decimal amount, decimal superDiscount)
        {
            if (model.DiscountType == (int)DiscountType.Amount)
            {
                if (model.DiscountApplied < model.Discount)
                {
                    decimal discountLeft = model.Discount - model.DiscountApplied;
                    if (amount > discountLeft)
                    {
                        return discountLeft;
                    }
                    else
                    {
                        return amount;
                    }
                }
                return 0;
            }
            else if (model.DiscountType == (int)DiscountType.Percentage)
            {
                var discount = (amount * model.Discount) / 100;
                return discount.RoundOff();
            }
            else
            {
                var discount = (amount * superDiscount) / 100;
                return discount.RoundOff();
            }
        }

        public async Task<ShoppingCartModel> AddInvoicesToMemberPortalShoppingCart(string userName, string invoiceIds)
        {
            ShoppingCartModel model = new ShoppingCartModel();
            Invoice invoice = new Invoice();
            Shoppingcart cart = new Shoppingcart();
            bool newCart = false;
            //Get the current Cart if it exists for the current user

            cart = await _unitOfWork.ShoppingCarts.GetMemberPortalShoppingCartByUserNameAsync(userName);
            //Add Cart
            if (cart == null)
            {
                cart = new Shoppingcart();
                newCart = true;
            }

            //Validate MemberPortalUser

            var user = await _entityService.GetEntityByWebLogin(userName);
            if (user == null)
            {
                _logger.Error($"Cant find a valid user with login:{userName}");
                throw new Exception("Invalid user Name");
            }
            var memberPortalUser = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync("memberportal");
            if (invoiceIds.Length > 0)
            {
                string[] arrInvoiceIds = invoiceIds.Split(',');

                foreach (var invoiceId in arrInvoiceIds)
                {
                    invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(Convert.ToInt32(invoiceId));

                    var invoiceModel = _mapper.Map<InvoiceModel>(invoice);

                    cart.UserId = memberPortalUser.UserId;
                    cart.TransactionDate = DateTime.Now;
                    cart.PaymentStatus = 0;
                    cart.MemberPortalUser = userName;
                    cart.EntityId = invoice.BillableEntityId;


                    foreach (var invoiceItem in invoiceModel.InvoiceDetails)
                    {
                        if (!cart.Shoppingcartdetails.Any(x => x.ItemId == invoiceItem.InvoiceDetailId))
                        {
                            Shoppingcartdetail item = new Shoppingcartdetail();
                            item.ShoppingCartId = cart.ShoppingCartId;
                            item.ItemId = invoiceItem.InvoiceDetailId;
                            item.Description = invoiceItem.Description;
                            item.ItemType = invoiceItem.ItemType;
                            item.Price = invoiceItem.Price;
                            item.Quantity = invoiceItem.Quantity;
                            item.Discount = invoiceItem.Discount;



                            item.Amount = await GetInvoiceItemBalance(invoiceItem.InvoiceDetailId);

                            if (invoice.InvoiceItemType != (int)InvoiceItemType.Marketplace)
                            {
                                if (invoiceItem.ItemType == (int)InvoiceItemType.Membership)
                                {
                                    item.ItemGroup = "Membership Payment";
                                    item.ItemGroupDescription = $"{invoice.Membership.MembershipType.Code}:{invoice.Membership.MembershipType.Name}";
                                }
                            }
                            else
                            {
                                item.ItemGroup = EnumUtil.GetDescription(((InvoiceItemType)invoiceItem.ItemType));
                                item.ItemGroupDescription = EnumUtil.GetDescription(((InvoiceItemType)invoiceItem.ItemType));
                            }
                            if (item.Amount > 0)
                            {
                                cart.Shoppingcartdetails.Add(item);
                            }

                        }

                    }
                }
                try
                {
                    if (newCart)
                    {
                        await _unitOfWork.ShoppingCarts.AddAsync(cart);
                    }
                    else
                    {
                        _unitOfWork.ShoppingCarts.Update(cart);
                    }
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to create cart with login:{userName} {ex.Message}");
                }
            }
            _logger.Information($"Added cart with login:{userName} InvoiceIds:{invoiceIds}");
            model = _mapper.Map<ShoppingCartModel>(cart);
            return model;
        }
        public async Task<ShoppingCartDetailModel> UpdateShoppingCartItem(ShoppingCartDetailModel model)
        {
            var shoppingCartDetailItem = await _unitOfWork.ShoppingCartDetails.GetShoppingCartDetailByIdAsync(model.ShoppingCartDetailId);

            //check the balance

            var invoiceDetail = await _unitOfWork.InvoiceDetails.GetInvoiceDetailByIdAsync(shoppingCartDetailItem.ItemId ?? 0);

            var balance = (invoiceDetail.Price * invoiceDetail.Quantity) - invoiceDetail.Writeoffs.Sum(x => x.Amount) - invoiceDetail.Receiptdetails.Where(x => x.Status == (int)Status.Active).Sum(x => x.Amount);

            if (model.Amount > balance)
            {
                throw new InvalidOperationException("You can not make payments for more than due amount.");
            }

            if (shoppingCartDetailItem.ShoppingCart.ReceiptId > 0)
            {
                //Need to update the receipt amount
                var receciptDetails = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByReceiptIdAsync(shoppingCartDetailItem.ShoppingCart.ReceiptId ?? 0);

                var receiptDetailItem = receciptDetails.Where(x => x.InvoiceDetailId == shoppingCartDetailItem.ItemId).FirstOrDefault();

                if (receiptDetailItem != null)
                {
                    try
                    {
                        receiptDetailItem.Amount = model.Amount;
                        _unitOfWork.ReceiptDetails.Update(receiptDetailItem);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to update the changed amount.");
                    }
                }

            }

            try
            {
                shoppingCartDetailItem.Amount = model.Amount;
                _unitOfWork.ShoppingCartDetails.Update(shoppingCartDetailItem);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<ShoppingCartDetailModel>(shoppingCartDetailItem);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update the changed amount.");
            }

        }

        public async Task<ShoppingCartModel> DeleteShoppingCartItem(ShoppingCartDetailModel model)
        {
            var shoppingCart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(model.ShoppingCartId ?? 0);

            try
            {
                var shoppingCartDetailItem = shoppingCart.Shoppingcartdetails.Where(x => x.ShoppingCartDetailId == model.ShoppingCartDetailId).FirstOrDefault();

                if (shoppingCartDetailItem != null)
                {
                    shoppingCart.Shoppingcartdetails.Remove(shoppingCartDetailItem);
                    if (shoppingCart.Shoppingcartdetails.Count == 0)
                    {
                        _unitOfWork.ShoppingCarts.Remove(shoppingCart);
                    }
                    else
                    {
                        _unitOfWork.ShoppingCarts.Update(shoppingCart);
                    }
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete the cart Item.");
            }
            return _mapper.Map<ShoppingCartModel>(shoppingCart);
        }
        public async Task<bool> UpdateInvoiceDetails(int id, string code, decimal superDiscount)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(id);
            if (cart == null)
            {
                return false;
            }
            if (code == null)
            {
                return false;
            }
            try
            {
                var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByCodeAsync(code);
                if (promoCode == null)
                {
                    return false;
                }
                var promoCodeModel = _mapper.Map<PromoCodeModel>(promoCode);
                //Update Invoice Details
                int invoiceId = 0;
                decimal discountApplied = 0;
                foreach (var cartItem in cart.Shoppingcartdetails)
                {
                    var invoiceDetail = await _unitOfWork.InvoiceDetails.GetByIdAsync(cartItem.ItemId ?? 0);
                    if (invoiceDetail != null)
                    {
                        promoCodeModel.DiscountApplied = discountApplied;
                        invoiceDetail.Discount = GetPromoCodeDiscount(promoCodeModel, invoiceDetail.Amount, superDiscount);
                        invoiceDetail.Amount = invoiceDetail.Amount - invoiceDetail.Discount;
                        discountApplied += invoiceDetail.Discount;
                        _unitOfWork.InvoiceDetails.Update(invoiceDetail);

                        if (invoiceId != invoiceDetail.InvoiceId)
                        {
                            invoiceId = invoiceDetail.InvoiceId;

                            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
                            if (invoice != null)
                            {
                                invoice.PromoCodeId = cart.PromoCodeId;
                                _unitOfWork.Invoices.Update(invoice);
                            }
                        }

                    }
                }
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update invoice details.");
            }
        }

        public async Task<decimal> GetInvoiceItemBalance(int invoiceDetailId)
        {
            var balance = 0.0M;

            var invoiceDetail = await _unitOfWork.InvoiceDetails.GetInvoiceDetailByIdAsync(invoiceDetailId);

            if (invoiceDetail != null)
            {
                var amount = invoiceDetail.Price * invoiceDetail.Quantity - invoiceDetail.Discount;
                var paid = invoiceDetail.Receiptdetails.Where(x => x.Status == (int)ReceiptStatus.Active || x.Status == (int)ReceiptStatus.Cancelled).Sum(x => x.Amount);
                var refund = invoiceDetail.Receiptdetails.Select(x => x.Refunddetails.Sum(x => x.Amount)).Sum();
                var writeOff = invoiceDetail.Writeoffs.Select(x => x.Amount ?? 0).Sum();
                balance = amount + refund - paid - writeOff;
            }

            return balance;
        }

        public List<string> GetOfflinePaymentTypes()
        {
            List<string> offlinePaymentTypes = new List<string>();
            var offlinePaymentTypesEnum = Enum.GetValues(typeof(OfflinePaymentType))
                                    .Cast<OfflinePaymentType>()
                                    .Select(v => v)
                                    .ToList();
            foreach(var item in offlinePaymentTypesEnum)
            {
                var offlinePaymentType = GetEnumDescription(item);
                offlinePaymentTypes.Add(offlinePaymentType);
            }
            return offlinePaymentTypes;
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo != null)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }

            return value.ToString();
        }
    }
}
