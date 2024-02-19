using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Max.Services
{
    public class BillingDocumentsService : IBillingDocumentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BillingDocumentsService> _logger;
        public BillingDocumentsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BillingDocumentsService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }
        public async Task<List<BillingDocumentModel>> GetAllBillingDocumentDetails()
        {
            var documents = await _unitOfWork.BillingDocuments.GetAllAutoBillingDocumentDetailsAsync();

            return documents.Select(p => new BillingDocumentModel
            {
                BillingDocumentId = p.BillingDocumentId,
                CreatedDate = p.CreatedDate,
                InvoiceType = p.InvoiceType,
                BillingType = p.BillingType,
                IsFinalized = p.IsFinalized,
                Status = p.Status,
                ThroughDate = p.ThroughDate,
                TotalAmount = p.Autobillingdrafts.Select(x => x.Amount).Sum(),
                InvoiceCount = p.Autobillingdrafts.Count
            }).ToList();
        }
        public async Task<BillingDocumentModel> CreateAutoBillingDocument(BillingDocumentModel model)
        {
            var billingDocument = new Billingdocument();
            //Get current BillingDocumentId

            var billingDocumentId = await _unitOfWork.BillingDocuments.GetCurrentBillingDocumentIdAsync();

            if(billingDocumentId >0)
            {
                //Delete Draft
                var lastDraft = await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsByBillingDocumentIdAsync(billingDocumentId);

                if (lastDraft != null)
                {
                    _unitOfWork.AutoBillingDrafts.RemoveRange(lastDraft);
                }
                billingDocument = await _unitOfWork.BillingDocuments.GetByIdAsync(billingDocumentId);
                billingDocument.CreatedDate = DateTime.Now;
                billingDocument.ThroughDate = model.ThroughDate;
                billingDocument.Status = (int)BillingJobStatus.Created;
                billingDocument.IsFinalized = model.IsFinalized;
                _unitOfWork.BillingDocuments.Update(billingDocument);
            }
            else
            {
                billingDocument.AbpdId = model.AbpdId;
                billingDocument.BillingType = model.BillingType;
                billingDocument.CreatedDate = DateTime.Now;
                billingDocument.InvoiceType = model.InvoiceType;
                billingDocument.ThroughDate = model.ThroughDate;
                billingDocument.Status = (int)BillingJobStatus.Created;
                billingDocument.IsFinalized = model.IsFinalized;
                await _unitOfWork.BillingDocuments.AddAsync(billingDocument);
            }
            try
            {
               
                await _unitOfWork.CommitAsync();

                return _mapper.Map<BillingDocumentModel>(billingDocument);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                throw new InvalidOperationException("Failed to create billing document");
            }

        }

        public async Task<bool> UpdateBillingDocumentStatus(int documentId, int status)
        {
            var billingDocument = await _unitOfWork.BillingDocuments.GetByIdAsync(documentId);

            billingDocument.Status = status;
            _unitOfWork.BillingDocuments.Update(billingDocument);

            //Update next billing dates
            if(status == (int)BillingStatus.Finalized)
            {
                var billingDates = await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByBillingTypeAsync(billingDocument.BillingType);
                billingDates.EffectiveDate = billingDates.EffectiveDate.AddMonths(1);
                billingDates.ThroughDate = billingDates.ThroughDate.AddMonths(1);
                _unitOfWork.AutoBillingProcessingDates.Update(billingDates);
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
                throw new InvalidOperationException("Failed to update billing document");
            }
        }
    }
}
