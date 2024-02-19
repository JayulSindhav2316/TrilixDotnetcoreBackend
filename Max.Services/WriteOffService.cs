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
    public class WriteOffService : IWriteOffService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
       
        public WriteOffService(IUnitOfWork unitOfWork,
                                IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<Writeoff> CreateWriteOff(WriteOffModel model)
        {
            var writeOff = new Writeoff();
           
            var accountingSetup = await _unitOfWork.AccountingSetups.GetAccountingSetupByOrganizationIdAsync(model.OrganizationId);
            //WriteOff GL not required for Cash accounting
            //var writeOffGlAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(accountingSetup.WriteOffGlAccount??0);
            var invoiceDetail = await _unitOfWork.InvoiceDetails.GetByIdAsync(model.InvoiceDetailId);

            writeOff.InvoiceDetailId = model.InvoiceDetailId;
            writeOff.Reason = model.Reason;
            writeOff.UserId = model.UserId;
            writeOff.Date = DateTime.Now;
            writeOff.Amount = model.Amount;

            //WriteOff GL not required for Cash accounting
            //Create Journal Entry
            /*
            var journalEntry = new Journalentryheader();

            //journalEntry.ReceiptHeaderId = 0;
            journalEntry.UserId = model.UserId;
            journalEntry.TransactionType = JournalTransactionType.MEMBERSHIP;
            journalEntry.EntryDate = DateTime.Now;

            //Create Journal Entry for Write Off 
           
            var journalEntryDetail = new Journalentrydetail();
            //journalEntryDetail.ReceiptDetailId = null;
            journalEntryDetail.GlAccountCode = invoiceDetail.GlAccount;
            journalEntryDetail.Description = "Write Off - " + invoiceDetail.Description;
            journalEntryDetail.Amount = -1 * model.Amount;
            journalEntryDetail.EntryType = JournalEntryType.WRITEOFF;
            journalEntry.Journalentrydetails.Add(journalEntryDetail);

            journalEntryDetail = new Journalentrydetail();
            //journalEntryDetail.ReceiptDetailId = 0;
            journalEntryDetail.GlAccountCode = writeOffGlAccount.Code;
            journalEntryDetail.Description = "Write Off - " + invoiceDetail.Description;
            journalEntryDetail.Amount =  model.Amount;
            journalEntryDetail.EntryType = JournalEntryType.WRITEOFF;
            journalEntry.Journalentrydetails.Add(journalEntryDetail);
            */
            try
            {
                //await _unitOfWork.JournalEntryHeaders.AddAsync(journalEntry);
                await _unitOfWork.WriteOffs.AddAsync(writeOff);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add Write Off entries.");
            }
            return writeOff;
        }

        async public Task<List<WriteOffModel>> GetWriteOffByInvoiceDetailId(int id)
        {
            List<WriteOffModel> writeOffs = new List<WriteOffModel>();

            var invoiceWriteOffs = await _unitOfWork.WriteOffs.GetWriteOffByInvoiceDetailIdAsync(id);

            if (invoiceWriteOffs != null)
            {
                foreach( var item in invoiceWriteOffs)
                {
                    var model = new WriteOffModel();

                    writeOffs.Add(_mapper.Map<WriteOffModel>(item));
                }
            }
            return writeOffs;
        }
    }
}