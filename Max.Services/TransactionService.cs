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
using static Max.Core.Constants;
using Microsoft.Extensions.Logging;

namespace Max.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;
        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TransactionService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }
        public async Task<bool> UpdateTransactionStatus(PaymentTransactionModel model)
        {
            Paymenttransaction paymenttransaction = _mapper.Map<Paymenttransaction>(model);

            Receiptheader receiptHeader = await _unitOfWork.ReceiptHeaders.GetReceiptByIdAsync(model.ReceiptId??0);

            var receiptDetails = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByReceiptIdAsync(model.ReceiptId ?? 0);

            var accountingSetup = await _unitOfWork.AccountingSetups.GetAccountingSetupByOrganizationIdAsync(receiptHeader.OrganizationId ?? 0);

            Glaccount onlineCreditGlAccount = new Glaccount();

            if (accountingSetup != null)
            {
                onlineCreditGlAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(accountingSetup.OnlineCreditGlAccount);
            }
            
            string promoCodeGlAccount = string.Empty;

            

            if (receiptHeader.PromoCodeId > 0)
            {
                var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByIdAsync(receiptHeader.PromoCodeId);
                promoCodeGlAccount = promoCode.GlAccount.Code;
            }

            decimal creditBalance = model.CreditBalanceUsed;

            //Update InvoceDetail

            var invoiceDetails = await _unitOfWork.InvoiceDetails.GetInvoiceDetailsByReceiptId(model.ReceiptId ?? 0);
           
            if (model.Status == (int)PaymentTransactionStatus.Approved)
            {
                bool updateInvoice = true;
                foreach (var item in invoiceDetails)
                {
                    if (updateInvoice)
                    {
                        item.Invoice.Status = (int)InvoiceStatus.FullyPaid;
                        if (model.AutoBillingDraftId > 0)
                        {
                            var period = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(item.Invoice.Membership.MembershipType.PaymentFrequency??0);
                            if(period.PeriodUnit=="Year")
                            {
                                item.Invoice.Membership.NextBillDate = item.Invoice.Membership.NextBillDate.AddYears(period.Duration);
                            }
                            else if (period.PeriodUnit == "Month")
                            {
                                item.Invoice.Membership.NextBillDate = item.Invoice.Membership.NextBillDate.AddMonths(period.Duration);
                            }
                            else
                            {
                                item.Invoice.Membership.NextBillDate = item.Invoice.Membership.NextBillDate.AddDays(period.Duration);
                            }
                        }
                        _unitOfWork.Invoices.Update(item.Invoice);
                        updateInvoice = false;
                    }
                    item.Status = (int)InvoiceStatus.FullyPaid;
                    _unitOfWork.InvoiceDetails.Update(item);

                }
                var journalEntry = new Journalentryheader();
                
                //if (!model.IsOfflinePayment)
                //{

                    journalEntry.ReceiptHeaderId = model.ReceiptId ?? 0;
                    journalEntry.UserId = receiptHeader.StaffId ?? 0;
                    journalEntry.TransactionType = BillingTypes.MEMBERSHIP;
                    journalEntry.EntryDate = model.TransactionDate;

                    //Create Journal Entry Details

                    foreach (var item in receiptDetails)
                    {
                        var journalEntryDetail = new Journalentrydetail();

                        if(item.Amount - creditBalance > 0)
                        {
                            journalEntryDetail.ReceiptDetailId = item.ReceiptDetailId;
                            journalEntryDetail.GlAccountCode = item.InvoiceDetail.GlAccount;
                            journalEntryDetail.Description = item.InvoiceDetail.Description;
                            journalEntryDetail.Amount = creditBalance > 0 ? item.Amount - creditBalance : item.Amount;
                            journalEntryDetail.EntryType = JournalEntryType.SALE;
                            journalEntry.Journalentrydetails.Add(journalEntryDetail);
                        }
                        
                        item.Status = (int)ReceiptStatus.Active;
                        _unitOfWork.ReceiptDetails.Update(item);

                        //Create Discount Entries if Discount > 0
                        if (item.Discount > 0)
                        {
                            journalEntryDetail = new Journalentrydetail();
                            journalEntryDetail.ReceiptDetailId = item.ReceiptDetailId;
                            journalEntryDetail.GlAccountCode = promoCodeGlAccount;
                            journalEntryDetail.Description = $"Discount: {item.InvoiceDetail.Description}";
                            journalEntryDetail.Amount = -1 * item.Discount;
                            journalEntryDetail.EntryType = JournalEntryType.DISCOUNT;

                            journalEntry.Journalentrydetails.Add(journalEntryDetail);
                        }

                        //Create Journal Entry for Crdeit Balance Used
                        if (creditBalance > 0)
                        {
                            decimal creditUsed = 0;

                            if (item.Amount >= creditBalance)
                            {
                                creditUsed = creditBalance;
                            }
                            else
                            {
                                creditUsed = item.Amount;
                            }

                            Credittransaction creditTransaction = new Credittransaction();

                            creditTransaction.TransactionDate = model.TransactionDate;
                            creditTransaction.ReceiptDetailId = item.ReceiptDetailId;
                            creditTransaction.EntryType = (int)CreditEntryType.DebitEntry;
                            creditTransaction.DebitGlAccount = onlineCreditGlAccount.Code;
                            creditTransaction.CreditGlAccount = item.InvoiceDetail.GlAccount;
                            creditTransaction.ExpirDate.AddYears(1);
                            creditTransaction.EntityId = item.EntityId ?? 0;
                            creditTransaction.Reason = $" Member Credit Used {item.InvoiceDetail.Description}";
                            creditTransaction.Status = (int)CreditStatus.Active;
                            creditTransaction.Amount = creditUsed;

                            await _unitOfWork.CreditTransactions.AddAsync(creditTransaction);
                            //Create Journal Entry for onlien credit

                            journalEntryDetail = new Journalentrydetail();
                            journalEntryDetail.ReceiptDetailId = item.ReceiptDetailId;
                            journalEntryDetail.GlAccountCode = item.InvoiceDetail.GlAccount;
                            journalEntryDetail.Description = $"Member Credit Used - {item.InvoiceDetail.Description}";
                            journalEntryDetail.Amount =  creditUsed;
                            journalEntryDetail.EntryType = JournalEntryType.ONLINE_CREDIT_USED;
                            item.Status = (int)ReceiptStatus.Active;                           
                            journalEntry.Journalentrydetails.Add(journalEntryDetail);

                            journalEntryDetail = new Journalentrydetail();
                            journalEntryDetail.ReceiptDetailId = item.ReceiptDetailId;
                            journalEntryDetail.GlAccountCode = onlineCreditGlAccount.Code;
                            journalEntryDetail.Description = $"Member Credit Used - {item.InvoiceDetail.Description}";
                            journalEntryDetail.Amount = -1 * creditUsed;
                            journalEntryDetail.EntryType = JournalEntryType.ONLINE_CREDIT_DB;
                            item.Status = (int)ReceiptStatus.Active;
                            creditBalance -= creditUsed;
                            journalEntry.Journalentrydetails.Add(journalEntryDetail);
                        }
                    }
                //}
                //else
                //{
                    //foreach (var item in receiptDetails)
                    //{
                    //    item.Status = (int)ReceiptStatus.Active;
                    //    _unitOfWork.ReceiptDetails.Update(item);
                    //}
                //}

                //Update Tables
                receiptHeader.Status = (int)ReceiptStatus.Active;
                if(model.IsOfflinePayment)
                {
                    receiptHeader.PaymentMode = "Off Line";
                }
                else
                {
                    receiptHeader.PaymentMode = model.PaymentType;
                }
                

                //Offline payment might have a past transaction date.
                receiptHeader.Date = model.TransactionDate;
                try
                {
                    _unitOfWork.ReceiptHeaders.Update(receiptHeader);
                    //if (!model.IsOfflinePayment)
                    //{
                        await _unitOfWork.JournalEntryHeaders.AddAsync(journalEntry);
                    //}
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new NullReferenceException("Failed to Create Journal Entry");
                }
            }
          
           
            //Update AutobillingDraft
            if (model.AutoBillingDraftId > 0)
            {
                var autoBillingDraftItem = await _unitOfWork.AutoBillingDrafts.GetByIdAsync(model.AutoBillingDraftId);

                if (autoBillingDraftItem != null)
                {
                    Autobillingpayment autoBillingPayment = new Autobillingpayment();
                    autoBillingPayment.AutoBillingDraftId = model.AutoBillingDraftId;
                    autoBillingPayment.PaymentTransactionId = model.PaymentTransactionId;
                    autoBillingPayment.ReceiptId = model.ReceiptId??0;
                    autoBillingPayment.Status = model.Status??0;
                    autoBillingDraftItem.IsProcessed = (int)ReceiptStatus.Active;

                    await _unitOfWork.AutoBillingPayments.AddAsync(autoBillingPayment);
                    _unitOfWork.AutoBillingDrafts.Update(autoBillingDraftItem);
                }
            }

            try
            {
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                throw new NullReferenceException("Failed to Update transaction STatus");
            }


        }
       
    }
}
