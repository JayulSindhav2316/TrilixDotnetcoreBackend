using AutoMapper;
using Max.Core;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;
using static Max.Core.Constants;

namespace Max.Services
{

    public class AutoBillingService : IAutoBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        static readonly ILogger _logger = Serilog.Log.ForContext<AutoBillingService>();
        private readonly IMapper _mapper;
        public AutoBillingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }
        /// <summary>
        /// Checks if AutoBilling Job is Due
        /// Job is due if:
        ///     1. AutoBilling Is Enabled
        ///     2. No Job has been created for todays date
        ///                 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAutoBillingJobDue()
        {

            _logger.Information("Checking Job Due Status");
            try
            {
                var settings = await _unitOfWork.AutoBillingSettings.SingleOrDefaultAsync(a => a.AutoBillingsettingsId > 0);

                if (settings.EnableAutomatedBillingForMembership == 0)
                {
                    _logger.Information("AutoBilling is not enabled.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"IsAutoBillingJobDue failed:{ex.Message}");
            }

            try
            {
                var job = await _unitOfWork.AutoBillingJobs.GetAutoBillingJobByDateAsync(DateTime.Now);
                if (job != null)
                {
                    _logger.Information($"AutoBilling Job is already created.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"IsAutoBillingJobDue failed:{ex.Message}");
            }
            return false;
        }

        public async Task<AutoBillingJobModel> CreatAutoBillingJob()
        {
            _logger.Information("Checking Job Due Status");

            var job = await _unitOfWork.AutoBillingJobs.GetAutoBillingJobByDateAsync(DateTime.Now);

            if (job != null)
            {
                _logger.Information("AutoBilling job has already been created for the date.");
                throw new Exception("AutoBilling job has already been created for the date.");
            }
            _logger.Information("Get Billing Date configuration");
            var billingDateSetup = await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByBillingTypeAsync("Membership");
            string invoiceType = Constants.InvoiceType.CREDITCARD;
            AutoBillingJobModel model = new AutoBillingJobModel();
            model.InvoiceType = invoiceType;
            model.ThroughDate = billingDateSetup.ThroughDate;
            model.BillingType = "Membership";
            model.Status = (int)BillingJobStatus.Created;
            model.Create = DateTime.Now;
            model.JobType = AutoBillingDraftType.Preliminary;
            model.AbpdId = billingDateSetup.AutoBillingProcessingDatesId;
            if (billingDateSetup.EffectiveDate.Date == DateTime.Now.Date)
            {
                model.JobType = AutoBillingDraftType.Finalized;
            }

            var autbillingJob = _mapper.Map<Autobillingjob>(model);
            _logger.Information("Creating Job");
            try
            {
                await _unitOfWork.AutoBillingJobs.AddAsync(autbillingJob);
                await _unitOfWork.CommitAsync();
                _logger.Information("Autobilling Jobcreated.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Autobilling creation failed:{ex.Message}");
            }

            return _mapper.Map<AutoBillingJobModel>(autbillingJob);

        }

        public async Task<AutoBillingJobModel> GetNextAutoBillingJob()
        {
            try
            {
                var job = await _unitOfWork.AutoBillingJobs.GetNextAutoBillingJobByDateAsync(DateTime.Now);

                if (job != null)
                {
                    return _mapper.Map<AutoBillingJobModel>(job);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Autobilling GetNextAutoBillingJob failed:{ex.Message}");
            }
            return new AutoBillingJobModel();
        }

        public async Task<bool> UpdateJobStatus(int jobId, int status)
        {
            if (jobId <= 0)
            {
                throw new InvalidOperationException($"Invalid Job Id.");
            }
            var job = await _unitOfWork.AutoBillingJobs.GetByIdAsync(jobId);
            if (job == null)
            {
                throw new NullReferenceException($"Job not found.");
            }
            job.Status = status;

            _unitOfWork.AutoBillingJobs.Update(job);
            await _unitOfWork.CommitAsync();

            return true;
        }
        public async Task<bool> RegenrateAutoBillingDraft(int billingDocumentId)
        {
            if (billingDocumentId <= 0)
            {
                throw new InvalidOperationException($"Invalid Document Id.");
            }
            var document = await _unitOfWork.BillingDocuments.GetByIdAsync(billingDocumentId);
            if (document == null)
            {
                throw new NullReferenceException($"Document not found.");
            }
            if(document.IsFinalized ==(int)BillingStatus.Created && document.Status == (int)Status.Active)
            {
                //Get the relared job
                var job = await _unitOfWork.AutoBillingJobs.GetAutoBillingJobByDateAsync(document.CreatedDate);
                if(job != null)
                {
                    _unitOfWork.AutoBillingJobs.Remove(job);
                }
                //Remove autobilling Drafts
                var drafts = await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsByBillingDocumentIdAsync(document.BillingDocumentId);
                if(drafts != null)
                {
                    _unitOfWork.AutoBillingDrafts.RemoveRange(drafts);
                }
                _unitOfWork.BillingDocuments.Remove(document);
            }

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Autobilling Job could not be regenerated. Please contact support");
            }

            return true;
        }
        

    }
}