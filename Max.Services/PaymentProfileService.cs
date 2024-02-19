
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
using AutoMapper;
using System.Linq;

namespace Max.Services
{
    public class PaymentProfileService : IPaymentProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PaymentProfileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<Paymentprofile> CreatePaymentProfile(PaymentProfileModel PaymentProfileModel)
        {
            Paymentprofile paymentProfile = new Paymentprofile();
            var isPrefferedProfile = false;
            if (PaymentProfileModel.ProfileId != null)
            {
                //Map Model Data
                paymentProfile.EntityId = PaymentProfileModel.EntityId;
                paymentProfile.ProfileId = PaymentProfileModel.ProfileId;

                //check if there is any preferred payment profile
                var prefferedPaymentProfiles = await _unitOfWork.PaymentProfiles.GetPreferredPaymentProfileByEntityIdAsync(PaymentProfileModel.EntityId);

                if(prefferedPaymentProfiles == null)
                {
                    isPrefferedProfile = true;
                }
                if (PaymentProfileModel.CreditCards.Count > 0)
                {
                    paymentProfile.AuthNetPaymentProfileId = PaymentProfileModel.CreditCards[0].AuthNetPaymentProfileId;
                    paymentProfile.CardNumber = PaymentProfileModel.CreditCards[0].CardNumber;
                    paymentProfile.CardType = PaymentProfileModel.CreditCards[0].CardType;
                    paymentProfile.ExpirationDate = PaymentProfileModel.CreditCards[0].ExpirationDate;
                    paymentProfile.PreferredPaymentMethod = isPrefferedProfile ? 1 : 0;
                    paymentProfile.Status = (int)Status.Active;
                    paymentProfile.CardHolderName = PaymentProfileModel.CreditCards[0].CardHolderName;
                }
                else
                {
                    paymentProfile.AuthNetPaymentProfileId = PaymentProfileModel.BankAccounts[0].AuthNetPaymentProfileId;
                    paymentProfile.AccountNumber = PaymentProfileModel.BankAccounts[0].AccountNumber;
                    paymentProfile.AccountType = PaymentProfileModel.BankAccounts[0].AccountType;
                    paymentProfile.NameOnAccount = PaymentProfileModel.BankAccounts[0].NameOnAccount;
                    paymentProfile.NickName = PaymentProfileModel.BankAccounts[0].NickName;
                    paymentProfile.PreferredPaymentMethod = isPrefferedProfile ? 1 : 0;
                    paymentProfile.Status = (int)Status.Active;
                }

                await _unitOfWork.PaymentProfiles.AddAsync(paymentProfile);
                await _unitOfWork.CommitAsync();
            }
            return paymentProfile;
        }

        public async Task<bool> DeletePaymentProfile(int id)
        {
            var PaymentProfile = await _unitOfWork.PaymentProfiles.GetByIdAsync(id);
            if (PaymentProfile != null)
            {
                PaymentProfile.Status = (int)Status.InActive;
                _unitOfWork.PaymentProfiles.Update(PaymentProfile);

                if(PaymentProfile.UseForAutobilling==(int)Status.Active)
                { 
                    var entity = await _unitOfWork.Entities.GetMembershipDetailByEntityId(PaymentProfile.EntityId);
                    var activeMemberships = entity.Memberships.Where(x => x.Status == (int)MembershipStatus.Active);
                    foreach (var activeMembership in activeMemberships)
                    {
                        if (activeMembership != null)
                        {
                            var billingFees = activeMembership.Billingfees.Select(x => x.MembershipFee.BillingFrequency);
                            if (billingFees.Contains((int)FeeBillingFrequency.Recurring))
                            {
                                activeMembership.AutoPayEnabled = (int)Status.InActive;
                                activeMembership.PaymentProfileId = null;
                                _unitOfWork.Memberships.Update(activeMembership);
                            }
                        }
                    }
                }
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;

        }

        public async Task<IEnumerable<Paymentprofile>> GetAllPaymentProfiles()
        {
            return await _unitOfWork.PaymentProfiles
                .GetAllPaymentProfilesAsync();
        }

        public async Task<PaymentProfileModel> GetPaymentProfileById(int id)
        {
            var profile =  await _unitOfWork.PaymentProfiles.GetPaymentProfileByIdAsync(id);
            return _mapper.Map<PaymentProfileModel>(profile);
        }
        public async Task<IEnumerable<Paymentprofile>> GetPaymentProfileByEntityId(int id)
        {
              return await _unitOfWork.PaymentProfiles
                    .GetPaymentProfileByEntityIdAsync(id);
        }

        public async Task<PaymentProfileModel> UpdatePaymentProfile(PaymentProfileModel model)
        {
             PaymentProfileModel paymentProfileModel = new PaymentProfileModel();
            if (model.ProfileId != null)
            {
                var paymentProfiles = await _unitOfWork.PaymentProfiles.GetActivePaymentProfileByEntityIdAsync(model.EntityId);

                if (!paymentProfiles.IsNullOrEmpty())
                {
                    if(model.CreditCards.Count > 0)
                    {
                        //check if this profile exists
                        foreach( var creditCardProfile in model.CreditCards)
                        {
                            var exitingProfile = paymentProfiles.Where(x => x.AuthNetPaymentProfileId == creditCardProfile.AuthNetPaymentProfileId).FirstOrDefault();

                            if(exitingProfile != null)
                            {
                                exitingProfile.CardHolderName = exitingProfile.CardHolderName??creditCardProfile.CardHolderName;
                                exitingProfile.CardNumber = creditCardProfile.CardNumber;
                                exitingProfile.ExpirationDate = creditCardProfile.ExpirationDate;
                                exitingProfile.CardType = creditCardProfile.CardType;
                                exitingProfile.Status = (int)Status.Active;
                                _unitOfWork.PaymentProfiles.Update(exitingProfile);
                            }
                            else
                            {
                                var paymentProfile = new Paymentprofile();

                                paymentProfile.EntityId = model.EntityId;
                                paymentProfile.ProfileId = model.ProfileId;
                                paymentProfile.CardHolderName = creditCardProfile.CardHolderName;
                                paymentProfile.CardNumber = creditCardProfile.CardNumber;
                                paymentProfile.ExpirationDate = creditCardProfile.ExpirationDate;
                                paymentProfile.CardType = creditCardProfile.CardType;
                                paymentProfile.Status = (int)Status.Active;
                                paymentProfile.AuthNetPaymentProfileId = creditCardProfile.AuthNetPaymentProfileId;
                                await _unitOfWork.PaymentProfiles.AddAsync(paymentProfile);
                            }

                        }
                    }
                    if(model.BankAccounts.Count > 0)
                    {

                        foreach (var bankAccountProfile in model.BankAccounts)
                        {
                            var exitingProfile = paymentProfiles.Where(x => x.AuthNetPaymentProfileId == bankAccountProfile.AuthNetPaymentProfileId).FirstOrDefault();

                            if (exitingProfile != null)
                            {
                                exitingProfile.AccountNumber = bankAccountProfile.AccountNumber;
                                exitingProfile.AccountType = bankAccountProfile.AccountType;
                                exitingProfile.NameOnAccount = bankAccountProfile.NameOnAccount;
                                exitingProfile.RoutingNumber = bankAccountProfile.RoutingNumber;
                                exitingProfile.Status = (int)Status.Active;
                                _unitOfWork.PaymentProfiles.Update(exitingProfile);
                            }
                            else
                            {
                                var paymentProfile = new Paymentprofile();

                                paymentProfile.EntityId = model.EntityId;
                                paymentProfile.ProfileId = model.ProfileId;
                                paymentProfile.AccountNumber = bankAccountProfile.AccountNumber;
                                paymentProfile.AccountType = bankAccountProfile.AccountType;
                                paymentProfile.NameOnAccount = bankAccountProfile.NameOnAccount;
                                paymentProfile.AuthNetPaymentProfileId = bankAccountProfile.AuthNetPaymentProfileId;
                                paymentProfile.RoutingNumber = bankAccountProfile.RoutingNumber;
                                paymentProfile.Status = (int)Status.Active;
                                await _unitOfWork.PaymentProfiles.AddAsync(paymentProfile);
                            }

                        }

                    }
                }
                await _unitOfWork.CommitAsync();

                // Now Map updated data to the view model

                paymentProfiles = await _unitOfWork.PaymentProfiles.GetActivePaymentProfileByEntityIdAsync(model.EntityId);

                if(!paymentProfiles.IsNullOrEmpty())
                {
                    paymentProfileModel.EntityId = model.EntityId;
                    paymentProfileModel.ProfileId = paymentProfiles.Select(x => x.ProfileId).FirstOrDefault(); // profileId -> Authorize.Net.CustomerProfileId

                    foreach(var item in paymentProfiles)
                    {
                        if(!item.CardNumber.IsNullOrEmpty())
                        {
                            CreditCardPaymentProfile creditCardProfile = new CreditCardPaymentProfile();
                            creditCardProfile.PaymentProfileId = item.PaymentProfileId;
                            creditCardProfile.AuthNetPaymentProfileId = item.AuthNetPaymentProfileId;
                            creditCardProfile.CardHolderName = item.CardHolderName;
                            creditCardProfile.CardNumber = item.CardNumber;
                            creditCardProfile.CardType = item.CardType;
                            creditCardProfile.ExpirationDate = item.ExpirationDate;
                            creditCardProfile.PaymentProfileId = item.PaymentProfileId;
                            creditCardProfile.PreferredPaymentMethod = item.PreferredPaymentMethod??0;
                            creditCardProfile.UseForAutoBilling = item.UseForAutobilling;
                            paymentProfileModel.CreditCards.Add(creditCardProfile);
                        }

                        if (!item.AccountNumber.IsNullOrEmpty())
                        {
                            BankAccountPaymentProfile bankAccountProfile = new BankAccountPaymentProfile();
                            bankAccountProfile.PaymentProfileId = item.PaymentProfileId;
                            bankAccountProfile.AuthNetPaymentProfileId = item.AuthNetPaymentProfileId;
                            bankAccountProfile.AccountNumber = item.AccountNumber;
                            bankAccountProfile.AccountType = item.AccountType;
                            bankAccountProfile.NameOnAccount = item.NameOnAccount;
                            bankAccountProfile.RoutingNumber = item.RoutingNumber;
                            bankAccountProfile.PreferredPaymentMethod = item.PreferredPaymentMethod ?? 0;
                            bankAccountProfile.UseForAutoBilling = item.UseForAutobilling;
                            bankAccountProfile.NickName = item.NickName;
                            paymentProfileModel.BankAccounts.Add(bankAccountProfile);
                        }
                    }
                }

                return paymentProfileModel;
            }
            return paymentProfileModel;
        }
        public async Task<bool>SetPreferredPaymentMethod(int entityId, int paymentProfileId)
        {
            var paymentProfiles = await _unitOfWork.PaymentProfiles.GetPaymentProfileByEntityIdAsync(entityId);
            if (!paymentProfiles.IsNullOrEmpty())
            {
                foreach (var currentProfile in paymentProfiles)
                {

                    currentProfile.PreferredPaymentMethod = (int)Status.InActive;
                    _unitOfWork.PaymentProfiles.Update(currentProfile);
                }
            }
            var paymentProfile = await _unitOfWork.PaymentProfiles.GetPaymentProfileByIdAsync(paymentProfileId);
            if(paymentProfile != null)
            {
                paymentProfile.PreferredPaymentMethod = (int)Status.Active;
                _unitOfWork.PaymentProfiles.Update(paymentProfile);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> SetAutoBillingPaymentMethod(int entityId, int paymentProfileId)
        {
            var paymentProfiles = await _unitOfWork.PaymentProfiles.GetPaymentProfileByEntityIdAsync(entityId);
            //Reset Active PaymentMethods
            if (!paymentProfiles.IsNullOrEmpty())
            {
                foreach (var currentProfile in paymentProfiles)
                {

                    currentProfile.UseForAutobilling = (int)Status.InActive;
                    _unitOfWork.PaymentProfiles.Update(currentProfile);
                }
            }
            var paymentProfile = await _unitOfWork.PaymentProfiles.GetPaymentProfileByIdAsync(paymentProfileId);
            if (paymentProfile != null)
            {
                paymentProfile.UseForAutobilling = (int)Status.Active;
                _unitOfWork.PaymentProfiles.Update(paymentProfile);
                var entity = await _unitOfWork.Entities.GetMembershipDetailByEntityId(entityId);
                var activeMemberships = entity.Memberships.Where(x => x.Status == (int)MembershipStatus.Active);

                foreach (var activeMembership in activeMemberships)
                {
                    if (activeMembership != null)
                    {
                        var billingFees = activeMembership.Billingfees.Select(x => x.MembershipFee.BillingFrequency);
                        if (billingFees.Contains((int)FeeBillingFrequency.Recurring))
                        {
                           activeMembership.AutoPayEnabled = (int)Status.Active;
                           activeMembership.PaymentProfileId = paymentProfile.ProfileId;
                            _unitOfWork.Memberships.Update(activeMembership);
                        }
                    }
                }
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        //TODO AKS: Should we delete or make it inactive
        public async Task<bool> DeletePaymentProfile(int entityId, int paymentProfileId)
        {
            var paymentProfile = await _unitOfWork.PaymentProfiles.GetPaymentProfileByIdAsync(paymentProfileId);
           
            if (paymentProfile != null)
            {
                paymentProfile.UseForAutobilling = (int)Status.InActive;
                _unitOfWork.PaymentProfiles.Update(paymentProfile);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

    }
   
}