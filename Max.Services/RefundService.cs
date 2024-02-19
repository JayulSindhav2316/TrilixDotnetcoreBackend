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
using System.Linq;
using AutoMapper;
using static Max.Core.Constants;
using Newtonsoft.Json.Linq;

namespace Max.Services
{
    public class RefundService : IRefundService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthNetService _authNetService;
        public RefundService(IUnitOfWork unitOfWork, 
                                IMapper mapper,
                                IAuthNetService authNetService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._authNetService = authNetService;
        }

        public async Task<RefundResponseModel> ProcessRefund(RefundRequestModel model)
        {
            RefundResponseModel response = new RefundResponseModel();

            response.ReceiptId = model.ReceiptId;
            response.ReceiptDetailId = model.ReceiptDetailId;
            response.InvoiceDetailId = model.InvoiceDetailId;
            response.UserId = model.UserId;

            //Get Reference Transaction
            var referenceTransactions = await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByReceiptIdAsync(model.ReceiptId);
            if (referenceTransactions == null)
            {
                throw new Exception("Failed to get reference transaction details.");
            }
            var receiptDetail = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByReceiptIdAsync(model.ReceiptId);

            if (model.RefundMode == (int)RefundMode.CreditCard)
            {

                var refundTransaction = referenceTransactions.Where(x => x.Status == (int)PaymentTransactionStatus.Approved).FirstOrDefault();

                var creditCardRefund = new CreditCardRefundModel();

                creditCardRefund.OrganizationId = receiptDetail.Select(x => x.ReceiptHeader.OrganizationId ?? 0).FirstOrDefault();
                creditCardRefund.ReceiptId = model.ReceiptId;
                creditCardRefund.ReceiptDetailId = model.ReceiptDetailId;
                creditCardRefund.RefundAmount = model.RefundAmount;
                creditCardRefund.RefundTransactionId = refundTransaction.TransactionId;
                creditCardRefund.CreditCardNumber = refundTransaction.AccountNumber;
                creditCardRefund.EntityId = receiptDetail.Where(x => x.ReceiptDetailId == model.ReceiptDetailId).Select(x => x.EntityId ?? 0).FirstOrDefault();
                try
                {
                    var paymentTransactionId = await _authNetService.ProcessCreditCardRefund(creditCardRefund);
                    model.RefundPaymentTransactionId = paymentTransactionId;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to process Refund.");
                }

                var paymentTransaction = await _unitOfWork.PaymentTransactions.GetPaymentTransactionByIdAsync(model.RefundPaymentTransactionId);
                
                if (paymentTransaction != null)
                {
                    response.Status = paymentTransaction.Status??0;
                    response.ResponseCode = paymentTransaction.ResponseCode;
                    response.ResponseMessage = paymentTransaction.MessageDetails;
                    if(paymentTransaction.Status != (int)PaymentTransactionStatus.Approved)
                    {
                        dynamic responseObject = JObject.Parse(paymentTransaction.ResponseDetails);
                        var error = responseObject.transactionResponse.errors[0];
                        response.ResponseMessage = $"{error.errorCode}:{error.errorText}";
                        return response;
                    }
                }
                else
                {
                    response.ResponseMessage = "Refund could not be processed";
                    return response;
                }
               
            }
            else
            {
                response.Status = (int)PaymentTransactionStatus.Approved;
                response.ResponseCode = "1"; // Online Credit/ Offline
                response.ResponseMessage = "";
            }
            var refundDetail = await CreateRefund(model);

            if(refundDetail != null)
            {
                response.RefundDetailId = refundDetail.RefundId;
            }
            return response;
        }

        public async Task<Refunddetail> CreateRefund(RefundRequestModel model)
        {
            var refunddetail = new Refunddetail();

            var receiptDetail = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByIdAsync(model.ReceiptDetailId);
            var accountingSetup = await _unitOfWork.AccountingSetups.GetAccountingSetupByOrganizationIdAsync(receiptDetail.ReceiptHeader.OrganizationId ?? 0);
            var processingFeeGlAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(accountingSetup.ProcessingFeeGlAccount);
            var onlineCreditGlAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(accountingSetup.OnlineCreditGlAccount);
            var offLinePaymentGlAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(accountingSetup.OffLinePaymentGlAccount);
            var salesReturnGlAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(accountingSetup.SalesReturnGlAccount);

            //Create Refund

            refunddetail.Amount = model.RefundAmount;
            refunddetail.ProcessingFee = model.ProcessingFee;
            refunddetail.Reason = model.Reason;
            refunddetail.ReceiptDetailId = model.ReceiptDetailId;
            refunddetail.EntityId = receiptDetail.EntityId ?? 0;
            refunddetail.Description = receiptDetail.InvoiceDetail.Description;
            refunddetail.RefundDate = DateTime.Now;
            refunddetail.RefundMode = model.RefundMode;
            refunddetail.UserId = model.UserId;
            refunddetail.PaymentTransactionId = model.RefundPaymentTransactionId;

            //Create Journal Entry

            var journalEntry = new Journalentryheader();

            journalEntry.ReceiptHeaderId = model.ReceiptId;
            journalEntry.UserId = model.UserId;
            journalEntry.TransactionType = JournalTransactionType.CANCELLATION;
            journalEntry.EntryDate = DateTime.Now;

            //Create Journal Entry for refund 

            var journalEntryDetail = new Journalentrydetail();
            journalEntryDetail.ReceiptDetailId = model.ReceiptDetailId;
            journalEntryDetail.GlAccountCode = receiptDetail.InvoiceDetail.GlAccount;
            journalEntryDetail.Description = "Refund - " + receiptDetail.InvoiceDetail.Description;
            journalEntryDetail.Amount = -1 * (model.ProcessingFee + model.RefundAmount);
            journalEntryDetail.EntryType = JournalEntryType.REFUND;
            journalEntry.Journalentrydetails.Add(journalEntryDetail);

            // Revert Discounts entries 
            if (receiptDetail.Discount > 0)
            {
                // Get Promo Code Account
                var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByIdAsync(receiptDetail.ReceiptHeader.PromoCodeId);
                if (promoCode != null)
                {
                    journalEntryDetail = new Journalentrydetail();
                    journalEntryDetail.ReceiptDetailId = model.ReceiptDetailId;
                    journalEntryDetail.GlAccountCode = promoCode.GlAccount.Code;
                    journalEntryDetail.Description = "Revert Discount - " + receiptDetail.InvoiceDetail.Description;
                    journalEntryDetail.Amount = receiptDetail.Discount;
                    journalEntryDetail.EntryType = JournalEntryType.DISCOUNT;
                    journalEntry.Journalentrydetails.Add(journalEntryDetail);
                }
            }

            if (model.ProcessingFee > 0)
            {
                //Create Journal Entry for Processing Fee 

                journalEntryDetail = new Journalentrydetail();
                journalEntryDetail.ReceiptDetailId = model.ReceiptDetailId;
                journalEntryDetail.GlAccountCode = processingFeeGlAccount.Code;
                journalEntryDetail.Description = "Processing Fee - " + receiptDetail.InvoiceDetail.Description;
                journalEntryDetail.Amount = model.ProcessingFee;
                journalEntryDetail.EntryType = JournalEntryType.ADJUSTMENT;
                journalEntry.Journalentrydetails.Add(journalEntryDetail);
            }

            if (model.RefundMode == (int)RefundMode.OnLineCredit)
            {
                Credittransaction creditTransaction = new Credittransaction();

                creditTransaction.TransactionDate = DateTime.Now;
                creditTransaction.ReceiptDetailId = model.ReceiptDetailId;
                creditTransaction.EntryType = (int)CreditEntryType.CreditEntry;
                creditTransaction.DebitGlAccount = receiptDetail.InvoiceDetail.GlAccount;
                creditTransaction.CreditGlAccount = onlineCreditGlAccount.Code;

                creditTransaction.ExpirDate.AddYears(1);
                creditTransaction.EntityId = receiptDetail.EntityId ?? 0;
                creditTransaction.Reason = model.Reason;
                creditTransaction.Status = (int)CreditStatus.Active;
                creditTransaction.Amount = model.RefundAmount;

                await _unitOfWork.CreditTransactions.AddAsync(creditTransaction);
                //Create Journal Entry for onlien credit

                journalEntryDetail = new Journalentrydetail();
                journalEntryDetail.ReceiptDetailId = model.ReceiptDetailId;
                journalEntryDetail.GlAccountCode = onlineCreditGlAccount.Code;
                journalEntryDetail.Description = "Credit - from " + receiptDetail.InvoiceDetail.Description;
                journalEntryDetail.Amount = model.RefundAmount;
                journalEntryDetail.EntryType = JournalEntryType.ONLINE_CREDIT_CR;
                journalEntry.Journalentrydetails.Add(journalEntryDetail);


            }
            else if (model.RefundMode == (int)RefundMode.OffLinePayment)
            {
                //Create Journal Entry for Offline Payment

                journalEntryDetail = new Journalentrydetail();
                journalEntryDetail.ReceiptDetailId = model.ReceiptDetailId;
                journalEntryDetail.GlAccountCode = offLinePaymentGlAccount.Code;
                journalEntryDetail.Description = "Off Line Payment - " + receiptDetail.InvoiceDetail.Description;
                journalEntryDetail.Amount = model.RefundAmount;
                journalEntryDetail.EntryType = JournalEntryType.REFUND;
                journalEntry.Journalentrydetails.Add(journalEntryDetail);

            }

            else if (model.RefundMode == (int)RefundMode.CreditCard || model.RefundMode == (int)RefundMode.ACH)
            {

               
                //Create Journal Entry for Credit Card??? Add to Sales Return.

                journalEntryDetail = new Journalentrydetail();
                journalEntryDetail.ReceiptDetailId = model.ReceiptDetailId;
                journalEntryDetail.GlAccountCode = salesReturnGlAccount.Code;
                journalEntryDetail.Description = "Refund - " + receiptDetail.InvoiceDetail.Description;
                journalEntryDetail.Amount = model.RefundAmount;
                journalEntryDetail.EntryType = JournalEntryType.REFUND;
                journalEntry.Journalentrydetails.Add(journalEntryDetail);

            }

            var refundReceiptDetail = receiptDetail;

            refundReceiptDetail.Status = (int)ReceiptStatus.Cancelled;

            try
            {
                await _unitOfWork.JournalEntryHeaders.AddAsync(journalEntry);
                await _unitOfWork.RefundDetails.AddAsync(refunddetail);
                _unitOfWork.ReceiptDetails.Update(refundReceiptDetail);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add Refund details.");
            }
            return refunddetail;
        }
    }
}