using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Max.Core.Constants;

namespace Max.Services
{
    public class VoidService : IVoidService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthNetService _authNetService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        public VoidService(IUnitOfWork unitOfWork,
                                IMapper mapper,
                                IPaymentTransactionService paymentTransactionService,
                                IAuthNetService authNetService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._authNetService = authNetService;
            this._paymentTransactionService = paymentTransactionService;
        }

        public async Task<VoidResponseModel> ProcessVoid(VoidRequestModel model)
        {
            VoidResponseModel response = new VoidResponseModel();

            response.ReceiptId = model.ReceiptId;
            response.UserId = model.UserId;

            //Get Reference Transaction
            var referenceTransactions = await _unitOfWork.PaymentTransactions.GetPaymentTransactionsByReceiptIdAsync(model.ReceiptId);
            if (referenceTransactions == null)
            {
                throw new Exception("Failed to get reference transaction details.");
            }
            var receipt = await _unitOfWork.ReceiptHeaders.GetReceiptDetailById(model.ReceiptId);

            if (receipt.PaymentMode == "CreditCard" || receipt.PaymentMode == "eCheck")
            {
                var resultModel = new AuthNetVoidModel();
                var voidTransaction = referenceTransactions.Where(x => x.Status == (int)PaymentTransactionStatus.Approved).FirstOrDefault();

                var creditCardVoid = new CreditCardVoidModel();

                creditCardVoid.OrganizationId = receipt.OrganizationId ?? 0;
                creditCardVoid.ReceiptId = model.ReceiptId;
                creditCardVoid.ReferenceTransactionId = voidTransaction.TransactionId;
                creditCardVoid.EntityId = receipt.BillableEntityId??0;
                creditCardVoid.VoidAmount = voidTransaction.Amount;
                creditCardVoid.PaymentMode = receipt.PaymentMode;
                try
                {
                    resultModel = await _authNetService.ProcessCreditCardVoid(creditCardVoid);
                    model.VoidPaymentTransactionId = resultModel.PaymentTransactionId;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to process Void.");
                }

                var paymentTransaction = await _unitOfWork.PaymentTransactions.GetPaymentTransactionByIdAsync(model.VoidPaymentTransactionId);

                if (paymentTransaction != null)
                {
                    response.Status = paymentTransaction.Status ?? 0;
                    response.ResponseCode = paymentTransaction.ResponseCode;
                    response.ResponseMessage = paymentTransaction.MessageDetails;
                    if (paymentTransaction.Status != (int)PaymentTransactionStatus.Approved)
                    {
                        dynamic responseObject = JObject.Parse(paymentTransaction.ResponseDetails);
                        var error = responseObject.transactionResponse.errors[0];
                        response.ResponseMessage = $"{error.errorCode}:{error.errorText}";
                        return response;
                    }
                    else if (resultModel.IsPaymentAlreadyVoided)
                    {
                        response.IsAlreadyVoided= true;
                        response.ResponseMessage = resultModel.PaymentVoidMessage;
                    }
                }
                else
                {
                    response.ResponseMessage = "Void could not be processed";
                    return response;
                }

            }
            else if (receipt.PaymentMode == "Check")
            {

                var voidTransaction = referenceTransactions.Where(x => x.Status == (int)PaymentTransactionStatus.Approved).FirstOrDefault();

                PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
                paymentTransaction.TransactionDate = DateTime.Now;
                paymentTransaction.ReceiptId = model.ReceiptId;
                paymentTransaction.EntityId = voidTransaction.EntityId;
                paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
                paymentTransaction.Amount = voidTransaction.Amount;
                paymentTransaction.PaymentType = PaymentType.CHECK;
                paymentTransaction.TransactionType = (int)PaymentTransactionType.Void;
                paymentTransaction.Result = (int)ReceiptStatus.Active;
                var voidPaymentTransaction = await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);
                paymentTransaction.PaymentTransactionId = voidPaymentTransaction.PaymentTransactionId;
                
                if (paymentTransaction != null)
                {
                    response.Status = voidPaymentTransaction.Status ?? 0;
                    response.ResponseCode = string.Empty;
                    response.ResponseCode = "1";
                    response.ResponseMessage = "Approved";
                }
                else
                {
                    response.ResponseMessage = "Void could not be processed";
                    return response;
                }

            }
            else
            {
                response.Status = (int)PaymentTransactionStatus.Approved;
                response.ResponseCode = "1"; 
                response.ResponseMessage = "";
            }
            var voidDetail = await CreateVoid(model);

            if (voidDetail != null)
            {
                response.VoidDetailId = voidDetail.VoidId;
            }
            return response;
        }

        public async Task<Voiddetail> CreateVoid(VoidRequestModel model)
        {
            var voidDetail = new Voiddetail();

            var receipt = await _unitOfWork.ReceiptHeaders.GetByIdAsync(model.ReceiptId);
            var receiptDetail = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByReceiptIdAsync(model.ReceiptId);
            var accountingSetup = await _unitOfWork.AccountingSetups.GetAccountingSetupByOrganizationIdAsync(receipt.OrganizationId ?? 0);
            var salesReturnGlAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(accountingSetup.SalesReturnGlAccount);

            //Add Void request

            voidDetail.ReceiptId = model.ReceiptId;
            voidDetail.Reason = model.Reason;
            voidDetail.PaymentTransactionId = model.VoidPaymentTransactionId;
            voidDetail.UserId = model.UserId;
            voidDetail.VoidDate = DateTime.Now;
            voidDetail.VoidMode = receipt.PaymentMode;

            receipt.Status = (int)ReceiptStatus.Void;
            _unitOfWork.ReceiptHeaders.Update(receipt);

            foreach ( var receiptDetailItem in receiptDetail)
            {
                if(receiptDetailItem.Status == (int) ReceiptStatus.Active)
                {
                    receiptDetailItem.Status = (int)ReceiptStatus.Void;
                    _unitOfWork.ReceiptDetails.Update(receiptDetailItem);
                }
            }

            //Create Journal Entry

            var journalEntry = new Journalentryheader();

            journalEntry.ReceiptHeaderId = model.ReceiptId;
            journalEntry.UserId = model.UserId;
            journalEntry.TransactionType = JournalTransactionType.MEMBERSHIP;
            journalEntry.EntryDate = DateTime.Now;

            //Create Journal Entry for Void 

            foreach (var receiptDetailItem in receiptDetail)
            {
                var journalEntryDetail = new Journalentrydetail();
                journalEntryDetail.ReceiptDetailId = receiptDetailItem.ReceiptDetailId;
                journalEntryDetail.GlAccountCode = receiptDetailItem.InvoiceDetail.GlAccount;
                journalEntryDetail.Description = "Void - " + receiptDetailItem.InvoiceDetail.Description;
                journalEntryDetail.Amount = -1 * receiptDetailItem.Amount;
                journalEntryDetail.EntryType = JournalEntryType.VOID;
                journalEntry.Journalentrydetails.Add(journalEntryDetail);
            }

            //Revert Credits if used

            var credits = await _unitOfWork.CreditTransactions.GetCreditTransactionsByReceiptIdAsync(model.ReceiptId);

            if(credits != null)
            {
                foreach (var creditEntry in credits)
                {
                    var journalEntryDetail = new Journalentrydetail();
                    journalEntryDetail.ReceiptDetailId = creditEntry.ReceiptDetailId;
                    journalEntryDetail.GlAccountCode = creditEntry.DebitGlAccount;
                    journalEntryDetail.Description = "Void - " + creditEntry.Reason;
                    journalEntryDetail.Amount = -1 * creditEntry.Amount??0;
                    journalEntryDetail.EntryType = JournalEntryType.VOID;
                    journalEntry.Journalentrydetails.Add(journalEntryDetail);

                    journalEntryDetail = new Journalentrydetail();
                    journalEntryDetail.ReceiptDetailId = creditEntry.ReceiptDetailId;
                    journalEntryDetail.GlAccountCode = creditEntry.CreditGlAccount;
                    journalEntryDetail.Description = "Revert - " + creditEntry.Reason;
                    journalEntryDetail.Amount = 1 * creditEntry.Amount ?? 0;
                    journalEntryDetail.EntryType = JournalEntryType.ONLINE_CREDIT_CR;
                    journalEntry.Journalentrydetails.Add(journalEntryDetail);

                    Credittransaction creditTransaction = new Credittransaction();

                    creditTransaction.TransactionDate = DateTime.Now;
                    creditTransaction.ReceiptDetailId = creditEntry.ReceiptDetailId;
                    creditTransaction.EntryType = (int)CreditEntryType.CreditEntry;
                    creditTransaction.DebitGlAccount = creditEntry.DebitGlAccount;
                    creditTransaction.CreditGlAccount = creditEntry.CreditGlAccount;

                    creditTransaction.ExpirDate.AddYears(1);
                    creditTransaction.EntityId = creditEntry.EntityId;
                    creditTransaction.Reason = model.Reason;
                    creditTransaction.Status = (int)CreditStatus.Active;
                    creditTransaction.Amount = creditEntry.Amount;

                    await _unitOfWork.CreditTransactions.AddAsync(creditTransaction);
                }
            }

            try
            {
                await _unitOfWork.JournalEntryHeaders.AddAsync(journalEntry);
                await _unitOfWork.VoidDetails.AddAsync(voidDetail);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add Void details.");
            }
            return voidDetail;
        }
    }  
}