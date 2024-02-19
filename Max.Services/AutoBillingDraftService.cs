using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;
using System.Linq;
using Max.Core;
using Serilog;
using Max.Services.Helpers;

namespace Max.Services
{
    public class AutoBillingDraftService : IAutoBillingDraftService
    {
        private readonly IUnitOfWork _unitOfWork;
        static readonly ILogger _logger = Serilog.Log.ForContext<AutoBillingDraftService>();
        public AutoBillingDraftService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<Autobillingdraft> CreateAutoBillingDraft(AutoBillingDraftModel autoBillingDraftModel)
        {
            Autobillingdraft autobillingdraft = new Autobillingdraft();

            autobillingdraft.BillingDocumentId = autoBillingDraftModel.BillingDocumentId;
            autobillingdraft.Name = autoBillingDraftModel.Name;
            autobillingdraft.EntityId = autoBillingDraftModel.EntityId??0;
            autobillingdraft.Amount = autoBillingDraftModel.Amount;
            autobillingdraft.NextDueDate = autoBillingDraftModel.NextDueDate;
            autobillingdraft.IsProcessed = autoBillingDraftModel.IsProcessed;
            autobillingdraft.ProfileId = autoBillingDraftModel.ProfileId;
            autobillingdraft.PaymentProfileId = autoBillingDraftModel.PaymentProfileId;

            try
            {
                await _unitOfWork.AutoBillingDrafts.AddAsync(autobillingdraft);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
            }
           
            return autobillingdraft;
        }
        public async Task<bool> UpdateAutoBillingDraft(AutoBillingDraftModel autoBillingDraftModel)
        {
            Autobillingdraft autobillingdraft = await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftByIdAsync(autoBillingDraftModel.AutoBillingDraftId);

            if (autobillingdraft != null)
            {
                autobillingdraft.BillingDocumentId = autoBillingDraftModel.BillingDocumentId;
                autobillingdraft.Name = autoBillingDraftModel.Name;
                autobillingdraft.EntityId = autoBillingDraftModel.EntityId??0;
                autobillingdraft.Amount = autoBillingDraftModel.Amount;
                autobillingdraft.NextDueDate = autoBillingDraftModel.NextDueDate;
                autobillingdraft.IsProcessed = autoBillingDraftModel.IsProcessed;
                autobillingdraft.ProfileId = autoBillingDraftModel.ProfileId;
                autobillingdraft.PaymentProfileId = autoBillingDraftModel.PaymentProfileId;

                try
                {
                    _unitOfWork.AutoBillingDrafts.Update(autobillingdraft);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message.ToString());
                }
            }
            return false;
        }
        public async Task<Autobillingdraft> CreateAutoBillingCreditCardDraft(BillingDocumentModel document, InvoiceModel invoice)
        {
            Autobillingdraft autobillingdraft = new Autobillingdraft();
           
            var membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(invoice.MembershipId ?? 0);

            //Checking if billing is onHold
            if(membership.BillingOnHold  == (int)AutoPayOnHoldStatus.OnHoldActive)
            {
                var onHold = await _unitOfWork.AutoBillingOnHolds.GetAutoBillingOnHoldByMembershipIdAsync(membership.MembershipId);
                if(onHold.ReviewDate?.Date > DateTime.Now.Date)
                {
                    _logger.Information($"Skipping autobilling for Membership Id:{membership.MembershipId} as it is on Hold");
                    return autobillingdraft;
                }
            }

            // Entity details
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(invoice.BillableEntityId ?? 0);
            var paymentProfileId = membership.PaymentProfileId;

            var paymentProfiles = await _unitOfWork.PaymentProfiles.GetPaymentProfileByEntityIdAsync(invoice.BillableEntityId ?? 0);
            var profile = paymentProfiles.Where(x => x.AuthNetPaymentProfileId == paymentProfileId).FirstOrDefault();

            if(profile != null)
            {
                autobillingdraft.BillingDocumentId = document.BillingDocumentId;
                autobillingdraft.Name = entity.Name;
                autobillingdraft.EntityId = invoice.BillableEntityId ?? 0;
                autobillingdraft.Amount = invoice.InvoiceDetails.Sum(x => x.Price);
                autobillingdraft.NextDueDate = invoice.DueDate;
                autobillingdraft.IsProcessed = 0;
                autobillingdraft.ProfileId = profile.ProfileId;
                autobillingdraft.PaymentProfileId = profile.AuthNetPaymentProfileId;
                autobillingdraft.CardNumber = profile.CardNumber;
                autobillingdraft.CardType = profile.CardType;
                autobillingdraft.ExpirationDate = profile.ExpirationDate;
                autobillingdraft.MembershipId = invoice.MembershipId;
                autobillingdraft.InvoiceId = invoice.InvoiceId;
                await _unitOfWork.AutoBillingDrafts.AddAsync(autobillingdraft);
                await _unitOfWork.CommitAsync();
            }

            return autobillingdraft;
        }
        public async Task<Autobillingdraft> GetAutobillingDraftById(int autoBillingDraftId)
        {
            return await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftByIdAsync(autoBillingDraftId);
        }
        public async Task<List<AutoBillingDraftModel>> GetAutobillingCurrentDraft()
        {
            var billingDocumentId = await _unitOfWork.BillingDocuments.GetCurrentBillingDocumentIdAsync();
            return await GetAutobillingDraftsByBillingDocumentId(billingDocumentId);
        }
        public async Task<IEnumerable<Autobillingdraft>> GetAllAutobillingDrafts()
        {
            return await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsAsync();
        }
        public async Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByPersonId(int personId)
        {
            return await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsByPersonIdAsync(personId);
        }
      
        public async Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByProcessStatus(int processStatus)
        {
            return await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsByProcessStatusAsync(processStatus);
        }
        public async Task<List<AutoBillingDraftModel>> GetAutobillingDraftsByBillingDocumentId(int billingDocumentId)
        {
            if (billingDocumentId == 0)
            {
                billingDocumentId = await _unitOfWork.BillingDocuments.GetLastBillingDocumentIdAsync();
            }

            var result = await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsByBillingDocumentIdAsync(billingDocumentId);

            return result.Select(x => new AutoBillingDraftModel()
            {
                BillingDocumentId = x.BillingDocumentId,
                AutoBillingDraftId = x.AutoBillingDraftId,
                BillableName = GetBillableEntity.GetBillableName(x.Entity),
                Name = GetBillableEntity.GetBillableName(x.Invoice.Entity),
                EntityId = x.EntityId,
                NextDueDate = x.NextDueDate,
                CardNumber = x.CardNumber,
                ExpirationDate = x.ExpirationDate,
                MembershipDescription = x.Membership?.MembershipType.Name,
                CardType = x.CardType,
                Amount = x.Amount,
                PaymentProfileId = x.PaymentProfileId,
                CreateDate = x.BillingDocument.CreatedDate,
                ProfileId = x.ProfileId,
                MembershipId = (int)x.MembershipId,
                InvoiceId = x.InvoiceId
            }).ToList();
        }
        public async Task<IEnumerable<CategoryRevenueModel>> GetAutobillingDraftsSummaryByCategoryId(int billingDocumentId)
        {
            if (billingDocumentId == 0)
            {
                billingDocumentId = await _unitOfWork.BillingDocuments.GetLastBillingDocumentIdAsync();
            }
            var list = await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsSummaryByCategoryIdAsync(billingDocumentId);

            List<CategoryRevenueModel> revenueList = new List<CategoryRevenueModel>();
            foreach(var item in list)
            {
                var revenue = new CategoryRevenueModel();

                revenue.CategoryName = item.Entity?.Memberships.Select(x => x.MembershipType.CategoryNavigation.Name).FirstOrDefault();
                revenue.Amount = item.Amount;
                revenueList.Add(revenue);
            }

            var result = revenueList.GroupBy(x => x.CategoryName).Select(p => new CategoryRevenueModel
            {
                CategoryName = p.Key,
                Amount = p.Sum(x => x.Amount)
            });
           return result;
        }
        public async Task<decimal?> GetLastAutobillingDraftsAmountCreated(int billingDocumentId)
        {
            if (billingDocumentId == 0)
            {
                billingDocumentId = await _unitOfWork.BillingDocuments.GetLastBillingDocumentIdAsync();
            }
            var autoBillingList = await _unitOfWork.AutoBillingDrafts.GetAutobillingDraftsByBillingDocumentIdAsync(billingDocumentId);
            decimal? autoBillingAmount = autoBillingList.Sum(x => x.Amount);
            return autoBillingAmount;
        }
        public async Task<decimal?> GetLastAutobillingDraftsAmountApproved(int billingDocumentId)
        {
            if (billingDocumentId == 0)
            {
                billingDocumentId = await _unitOfWork.BillingDocuments.GetLastBillingDocumentIdAsync();
            }
            var autoBillingList = await _unitOfWork.AutoBillingDrafts.GetLastAutoBillingDraftPaymentsAsync(billingDocumentId);
            decimal? autoBillingAmount = autoBillingList.Where(x => x.Autobillingpayments.Any(x => x.PaymentTransaction.Status == (int)PaymentTransactionStatus.Approved)).Sum(x => x.Amount);
            return autoBillingAmount;
        }

        public async Task<decimal?> GetLastAutoBillingDraftsAmountDeclined(int billingDocumentId)
        {
            if (billingDocumentId == 0)
            {
                billingDocumentId = await _unitOfWork.BillingDocuments.GetLastBillingDocumentIdAsync();
            }
            var autoBillingList = await _unitOfWork.AutoBillingDrafts.GetLastAutoBillingDraftPaymentsAsync(billingDocumentId);
            decimal? autoBillingAmount = autoBillingList.Where(x => x.Autobillingpayments.Any(x => x.PaymentTransaction.Status == (int)PaymentTransactionStatus.Declined)).Sum(x => x.Amount);
            return autoBillingAmount;
        }
        public async Task<dynamic> GetLastBillingChartInvoiceChartData()
        {
            var draftAmount = await GetLastAutobillingDraftsAmountCreated(0);
            var approved = await GetLastAutobillingDraftsAmountApproved(0);
            var result = new
            {
                AmountCreated = draftAmount,
                AmountApproved = approved,
                AmountDeclined = draftAmount - approved
            };
            return result;
        }

        public async Task<bool> SetAutoPayOnHold(AutoBillingHoldRequestModel model)
        {
            var membership = await _unitOfWork.Memberships.GetByIdAsync(model.MembershipId);
            if(membership != null)
            {
                membership.BillingOnHold = (int)AutoPayOnHoldStatus.OnHoldActive;

                var autoBillingOnHold = new Autobillingonhold();

                autoBillingOnHold.MembershipId = membership.MembershipId;
                autoBillingOnHold.Reason = model.Reason;
                if (model.ReviewDate != null)
                {
                    autoBillingOnHold.ReviewDate = model.ReviewDate;
                }
                autoBillingOnHold.UserId = model.UserId;

                _unitOfWork.Memberships.Update(membership);
                await _unitOfWork.AutoBillingOnHolds.AddAsync(autoBillingOnHold);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> ClearAutoPayOnHold(AutoBillingHoldRequestModel model)
        {
            var membership = await _unitOfWork.Memberships.GetByIdAsync(model.MembershipId);
            if (membership != null)
            {
                membership.BillingOnHold = (int)AutoPayOnHoldStatus.OnHoldInActive;

                var autoBillingOnHold = new Autobillingonhold();

                autoBillingOnHold.MembershipId = membership.MembershipId;
                autoBillingOnHold.Reason = "Auto Pay Re enabled.";
                autoBillingOnHold.ReviewDate = DateTime.Now;
                autoBillingOnHold.UserId = model.UserId;

                _unitOfWork.Memberships.Update(membership);
                await _unitOfWork.AutoBillingOnHolds.AddAsync(autoBillingOnHold);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

    }
}
