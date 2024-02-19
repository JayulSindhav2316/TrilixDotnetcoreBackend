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
using static Max.Core.Constants;

namespace Max.Services
{
    public class GeneralLedgerService : IGeneralLedgerService
    {

        private readonly IUnitOfWork _unitOfWork;
        public GeneralLedgerService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IList<GeneralLedgerModel>> GetGeneralLedger(string glAccount, string searchBy,  DateTime fromDate, DateTime toDate)
        {
            // Check searchby Parametr and set the search criteria

            if(searchBy.IsNullOrEmpty())
            {
                throw new ArgumentNullException("Invalid serach criteria.");
            }
            if (DateTime.Compare(fromDate, toDate) >0)
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
            var journalEntries = await _unitOfWork.JournalEntryDetails.GetReceiptsByDateRangeAsync( glAccount,  fromDate,  toDate);

            var result = journalEntries.Select(x => new GeneralLedgerModel()
            {
                TransactionDate = x.JournalEntryHeader.EntryDate.ToString("MM/dd/yyyy"),
                ReceiptId = x.JournalEntryHeader.ReceiptHeaderId??0,
                ItemDescription = x.Description,
                GlAccount = x.GlAccountCode,
                PaymentMode = x.EntryType == JournalEntryType.SALE ? x.JournalEntryHeader.ReceiptHeader.PaymentMode: x.EntryType == JournalEntryType.VOID ? x.JournalEntryHeader.ReceiptHeader.PaymentMode: "",
                TransactionType = x.JournalEntryHeader.TransactionType,
                EntryType = x.EntryType,
                Amount = x.Amount
            }).ToList();

            return result;
        }
    }
}