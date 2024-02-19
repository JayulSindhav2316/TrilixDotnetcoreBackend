using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;


namespace Max.Services
{
    public class AutoBillingProcessingDatesService : IAutoBillingProcessingDatesService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AutoBillingProcessingDatesService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<Autobillingprocessingdate> CreateAutoBillingProcessingDate(AutoBillingProcessingDateModel autoBillingProcessingDateModel)
        {
            Autobillingprocessingdate autobillingprocessingdate = new Autobillingprocessingdate();

            autobillingprocessingdate.BillingType = autoBillingProcessingDateModel.BillingType;
            autobillingprocessingdate.InvoiceType = autoBillingProcessingDateModel.InvoiceType;
            autobillingprocessingdate.ThroughDate = autoBillingProcessingDateModel.ThroughDate;
            autobillingprocessingdate.EffectiveDate = autoBillingProcessingDateModel.EffectiveDate;
            autobillingprocessingdate.Status = autoBillingProcessingDateModel.Status;
            autobillingprocessingdate.IsLastDayOfEffectiveDate = autoBillingProcessingDateModel.IsLastDayOfEffectiveDate;
            autobillingprocessingdate.IsLastDayOfThroughDate = autoBillingProcessingDateModel.IsLastDayOfThroughDate;
            autobillingprocessingdate.LastUpdated = autoBillingProcessingDateModel.LastUpdated;


            await _unitOfWork.AutoBillingProcessingDates.AddAsync(autobillingprocessingdate);
            await _unitOfWork.CommitAsync();
            return autobillingprocessingdate;
        }
        public async Task<bool> UpdateAutoBillingProcessingDate(AutoBillingProcessingDateModel autoBillingProcessingDateModel)
        {
            Autobillingprocessingdate autobillingprocessingdate = await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByABPDIdAsync(autoBillingProcessingDateModel.AutoBillingProcessingDatesId);

            if (autobillingprocessingdate != null)
            {
                autobillingprocessingdate.BillingType = autoBillingProcessingDateModel.BillingType;
                autobillingprocessingdate.InvoiceType = autoBillingProcessingDateModel.InvoiceType;
                autobillingprocessingdate.ThroughDate = autoBillingProcessingDateModel.ThroughDate;
                autobillingprocessingdate.EffectiveDate = autoBillingProcessingDateModel.EffectiveDate;
                autobillingprocessingdate.Status = autoBillingProcessingDateModel.Status;
                autobillingprocessingdate.IsLastDayOfEffectiveDate = autoBillingProcessingDateModel.IsLastDayOfEffectiveDate;
                autobillingprocessingdate.IsLastDayOfThroughDate = autoBillingProcessingDateModel.IsLastDayOfThroughDate;
                autobillingprocessingdate.LastUpdated = autoBillingProcessingDateModel.LastUpdated;

                _unitOfWork.AutoBillingProcessingDates.Update(autobillingprocessingdate);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByABPDId(int abpdId)
        {
            return await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByABPDIdAsync(abpdId);
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDates()
        {
            return await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesAsync();
        }
        public async Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByBillingType(string billingType)
        {
            return await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByBillingTypeAsync(billingType);
        }
        public async Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByInvoiceType(string invoiceType)
        {
            return await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByInvoiceTypeAsync(invoiceType);
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByThroughDate(DateTime throughDate)
        {
            return await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByThroughDateAsync(throughDate);
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByEffectiveDate(DateTime effectiveDate)
        {
            return await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByEffectiveDateAsync(effectiveDate);
        }
        public async Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByStatus(int status)
        {
            return await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByStatusAsync(status);
        }
    }
}
