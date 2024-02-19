using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;
using AutoMapper;
using Max.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Max.Core.Constants;
using Microsoft.Extensions.Logging;

namespace Max.Services
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<PaymentTransactionService> _logger;

        public PaymentTransactionService(IUnitOfWork unitOfWork, IMapper mapper, ITransactionService transactionService, ILogger<PaymentTransactionService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._transactionService = transactionService;
            this._logger = logger;
        }
        public async Task<PaymentTransactionModel> CreatePaymentTransaction(PaymentTransactionModel model)
        {
            Paymenttransaction paymenttransaction = _mapper.Map<Paymenttransaction>(model);
            try
            {
                await _unitOfWork.PaymentTransactions.AddAsync(paymenttransaction);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<PaymentTransactionModel>(paymenttransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                             ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                throw ex;
            }

        }

        public async Task<bool> UpdatePaymentTransaction(PaymentTransactionModel paymentTransactionModel)
        {
            var paymenttransaction = _mapper.Map<Paymenttransaction>(paymentTransactionModel);

            if (paymenttransaction != null)
            {
                try
                {
                    _unitOfWork.PaymentTransactions.Update(paymenttransaction);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw ex;
                }
               
            }
            return false;
        }
        public async Task<IList<CreditCardReportModel>> GetCreditCardReport(string cardType, string searchBy, DateTime fromDate, DateTime toDate)
        {
            // Check searchby Parametr and set the search criteria

            if (searchBy.IsNullOrEmpty())
            {
                throw new ArgumentNullException("Invalid serach criteria.");
            }
            if (DateTime.Compare(fromDate, toDate) > 0)
            {
                throw new ArgumentNullException("Invalid dates in search criteria.");
            }
            if (searchBy.ToUpper() == "DAY")
            {
                toDate = fromDate;
            }
            if (searchBy.ToUpper() == "MONTH")
            {
                fromDate = Extenstions.FirstDayOfMonth(fromDate);
                toDate = Extenstions.LastDayOfMonth(fromDate);
            }
            var cardPayments = await _unitOfWork.PaymentTransactions.GetCreditCardPaymentsByDateRangeAsync(cardType, fromDate, toDate);

            var result = cardPayments.Select(x => new CreditCardReportModel()
            {
                TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
                ReceiptId = x.Receipt.Receiptid,
                BillableName = x.Entity.Name,
                CreditCardNumber = x.AccountNumber?? " ",
                CardType = x.CardType?? " ",
                AuthCode = x.AuthCode?? " ",
                PaymentStatus = Enum.GetName(typeof(PaymentTransactionStatus), x.Status),
                TransactionType = x.TransactionType == (int)PaymentTransactionType.Sale ? "Payment" : x.TransactionType == (int)PaymentTransactionType.Refund? "Refund": "Void",
                Amount = x.TransactionType == (int)PaymentTransactionType.Sale? x.Amount: x.Amount * -1
            }).ToList();

            return result;
        }

        public async Task<IList<DepositReportModel>> GetDepositReport(string paymentType, string searchBy, DateTime fromDate, DateTime toDate, int summary, string portal)
        {
            // Check searchby Parametr and set the search criteria

            if (searchBy.IsNullOrEmpty())
            {
                throw new ArgumentNullException("Invalid serach criteria.");
            }
            if (DateTime.Compare(fromDate, toDate) > 0)
            {
                throw new ArgumentNullException("Invalid dates in search criteria.");
            }
            if (searchBy.ToUpper() == "DAY")
            {
                toDate = fromDate;
            }
            if (searchBy.ToUpper() == "MONTH")
            {
                fromDate = Extenstions.FirstDayOfMonth(fromDate);
                toDate = Extenstions.LastDayOfMonth(fromDate);
            }
            var deposits = await _unitOfWork.PaymentTransactions.GetDepositsByDateRangeAsync(paymentType, fromDate, toDate, portal);

            if(summary == (int)Status.Active)
            {
                var result = deposits.GroupBy(x => new { x.TransactionDate, x.PaymentType, x.TransactionType })
                            .Select(y => new DepositReportModel()
                            {
                                TransactionDate = y.Key.TransactionDate.ToString("MM/dd/yyyy"),
                                PaymentMode = y.Key.PaymentType,
                                TotalCash  = y.Where(x => y.Key.PaymentType == PaymentType.CASH && y.Key.TransactionType == (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum() - y.Where(x => y.Key.PaymentType == PaymentType.CASH && y.Key.TransactionType != (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum(),
                                TotalCheck = y.Where(x => y.Key.PaymentType == PaymentType.CHECK && y.Key.TransactionType == (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum() - y.Where(x => y.Key.PaymentType == PaymentType.CHECK && y.Key.TransactionType != (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum(),
                                TotalCreditCard = y.Where(x => y.Key.PaymentType == PaymentType.CREDITCARD && y.Key.TransactionType == (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum() - y.Where(x => y.Key.PaymentType == PaymentType.CREDITCARD && y.Key.TransactionType != (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum(),
                                TotalECheck = y.Where(x => y.Key.PaymentType == PaymentType.ECHECK && y.Key.TransactionType == (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum() - y.Where(x => y.Key.PaymentType == PaymentType.ECHECK && y.Key.TransactionType != (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum(),
                                TotalOffline = y.Where(x => y.Key.PaymentType == PaymentType.OFFLINE && y.Key.TransactionType == (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum() - y.Where(x => y.Key.PaymentType == PaymentType.OFFLINE && y.Key.TransactionType != (int)PaymentTransactionType.Sale).Select(x => x.Amount).Sum(),
                            }).ToList();
                //Now make the items in a row
                var sortedResult = new List<DepositReportModel>();
                var currentDate = result.Select(x => x.TransactionDate).FirstOrDefault();
                var row = new DepositReportModel();
                row.TransactionDate = currentDate;
                foreach (var item in result)
                {
                    if (currentDate != item.TransactionDate)
                    {
                        sortedResult.Add(row);
                        currentDate = item.TransactionDate;
                        row = new DepositReportModel();
                        row.TransactionDate= item.TransactionDate;
                        switch (item.PaymentMode)
                        {
                            case "Cash":
                                row.TotalCash += item.TotalCash;
                                break;
                            case "Check":
                                row.TotalCheck += item.TotalCheck;
                                break;
                            case "eCheck":
                                row.TotalECheck += item.TotalECheck;
                                break;
                            case "CreditCard":
                                row.TotalCreditCard += item.TotalCreditCard;
                                break;
                            case "Off Line":
                                row.TotalOffline += item.TotalOffline;
                                break;
                        }
                    }
                    else
                    {
                        switch(item.PaymentMode)
                        {
                            case "Cash":
                                row.TotalCash += item.TotalCash;
                                break;
                            case "Check":
                                row.TotalCheck += item.TotalCheck;
                                break;
                            case "eCheck":
                                row.TotalECheck += item.TotalECheck;
                                break;
                            case "CreditCard":
                                row.TotalCreditCard += item.TotalCreditCard;
                                break;
                            case "Off Line":
                                row.TotalOffline += item.TotalOffline;
                                break;
                        }
                    }
                }
                //Add Last row
                if(row.TransactionDate != null)
                {
                    sortedResult.Add(row);
                }
                return sortedResult;
            }
            else 
            {
                var result = deposits.Select(x => new DepositReportModel()
                {
                    TransactionDate = x.TransactionDate.ToString("MM/dd/yyyy"),
                    ReceiptId = x.Receipt.Receiptid,
                    BillableName = x.Entity.Name,
                    CreditCardNumber = x.AccountNumber ?? " ",
                    CheckNumber = x.AccountNumber ?? " ",
                    BankName = x.BankName ?? " ",
                    PaymentMode = x.PaymentType ?? " ",
                    UserName = $"{x.Receipt.Staff.FirstName} {x.Receipt.Staff.LastName}",
                    Amount = x.TransactionType == (int) PaymentTransactionType.Sale?x.Amount: x.Amount * -1,
                    TransactionReference = x.PaymentType == "CreditCard" ? $"{x.CardType}-{x.AccountNumber}" : $"{ x.BankName}-{x.AccountNumber}",
                    Portal = x.Receipt.Portal == 0 ? "Staff" : "Member"
                }).ToList();
               
                return result;
            }
        }

        public async Task<bool> ProcessCreditPaymentTransaction(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(cartId);

            if (cart != null)
            {
                //check if cart already has a receipt
                if (cart.ReceiptId > 0)
                {
                    var cartReceipt = await _unitOfWork.ReceiptHeaders.GetReceiptItemDetailById(cart.ReceiptId ?? 0);

                    if (cartReceipt != null)
                    {
                        //User might have alreday created/tried the payment
                        //Check if status is not Approved

                        if (cart.Receipt.Status == (int)ReceiptStatus.Active)
                        {
                            return false;
                        }

                        // Create PaymentRecord
                        PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
                        paymentTransaction.TransactionDate = DateTime.Now;
                        paymentTransaction.ReceiptId = cart.ReceiptId;
                        paymentTransaction.EntityId = cart.EntityId;
                        paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
                        paymentTransaction.Amount = cartReceipt.Receiptdetails.Sum(x => x.Amount);
                        paymentTransaction.PaymentType = PaymentType.MAXCREDIT;
                        paymentTransaction.TransactionType = (int)PaymentTransactionType.Sale;
                        paymentTransaction.Result = (int)ReceiptStatus.Active;
                        paymentTransaction.CreditBalanceUsed = cartReceipt.Receiptdetails.Sum(x => x.Amount);

                        await CreatePaymentTransaction(paymentTransaction);

                        var result = await _transactionService.UpdateTransactionStatus(paymentTransaction);

                        return result;
                    }
                }
            }
            return false;
        }
        public async Task<Paymenttransaction> GetPaymentTransactionById(int paymentTransactionId)
        {
            return await _unitOfWork.PaymentTransactions.GetPaymentTransactionByIdAsync(paymentTransactionId);
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByDate(DateTime transactionDate)
        {
            return await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByDateAsync(transactionDate);
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByEntityIdAsync(int id)
        {
            return await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByEntityIdAsync(id);
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByReceiptId(int receiptId)
        {
            return await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByReceiptIdAsync(receiptId);
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByTransactionType(int transactionType) 
        {
            return await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByTransactionTypeAsync(transactionType);
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByPaymentType(string paymentType)
        {
            return await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByPaymentTypeAsync(paymentType);
        }
    }
}
