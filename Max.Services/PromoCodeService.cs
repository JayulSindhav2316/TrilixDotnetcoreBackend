using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class PromoCodeService : IPromoCodeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;
        public PromoCodeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InvoiceService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }


        public async Task<IEnumerable<Promocode>> GetAllPromoCodes()
        {
            return await _unitOfWork.PromoCodes
                .GetAllPromoCodesAsync();
        }

        public async Task<Promocode> GetPromoCodeById(int id)
        {
            return await _unitOfWork.PromoCodes
                .GetPromoCodeByIdAsync(id);
        }
        public async Task<PromoCodeModel> GetPromoCodeByCode(string code)
        {
            var promoCode =  await _unitOfWork.PromoCodes
               .GetPromoCodeByCodeAsync(code);

            if (promoCode != null)
            {
                return _mapper.Map<PromoCodeModel>(promoCode);
            }
            return new PromoCodeModel();
        }
        public async Task<Promocode> CreatePromoCode(PromoCodeModel PromoCodeModel)
        {
            Promocode promoCode = new Promocode();

            var isValid = await ValidPromoCode(PromoCodeModel, true);
            if (isValid)
            {
                //Map Model Data
                promoCode.Code = PromoCodeModel.Code;
                promoCode.Description = PromoCodeModel.Description;
                promoCode.Status = PromoCodeModel.Status;
                promoCode.Discount = PromoCodeModel.Discount;
                promoCode.DiscountType = PromoCodeModel.DiscountType;
                promoCode.GlAccountId = PromoCodeModel.GlAccountId;
                promoCode.StartDate = PromoCodeModel.StartDate??DateTime.Now;
                promoCode.ExpirationDate = PromoCodeModel.ExpirationDate?? Constants.MySQL_MaxDate;
                promoCode.TransactionType = PromoCodeModel.TransactionType;

                try
                {
                    await _unitOfWork.PromoCodes.AddAsync(promoCode);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new Exception("Unable to add Promo Code.");
                }
            }
            return promoCode;
        }

        public async Task<bool> UpdatePromoCode(PromoCodeModel PromoCodeModel)
        {
            var isValidPromoCode = await ValidPromoCode(PromoCodeModel, false);
            if (isValidPromoCode)
            {
                var promoCode = await _unitOfWork.PromoCodes.GetByIdAsync(PromoCodeModel.PromoCodeId);

                if (promoCode != null)
                {
                    //Map Model Data
                    promoCode.Code = PromoCodeModel.Code;
                    promoCode.Description = PromoCodeModel.Description;
                    promoCode.Status = PromoCodeModel.Status;
                    promoCode.Discount = PromoCodeModel.Discount;
                    promoCode.DiscountType = PromoCodeModel.DiscountType;
                    promoCode.GlAccountId = PromoCodeModel.GlAccountId;
                    if(PromoCodeModel.StartDate != null)
                    {
                        promoCode.StartDate = PromoCodeModel.StartDate;
                    }
                    if (PromoCodeModel.ExpirationDate != null)
                    {
                        promoCode.ExpirationDate = PromoCodeModel.ExpirationDate;
                    }
                    promoCode.TransactionType = PromoCodeModel.TransactionType;
                }
                try
                {
                    _unitOfWork.PromoCodes.Update(promoCode);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new Exception("Unable to update Promo Code.");
                }
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePromoCode(int id)
        {

            var promoCode = await _unitOfWork.PromoCodes.GetByIdAsync(id);
            if (promoCode != null)
            {
                var receipts = await _unitOfWork.ReceiptHeaders.GetReceiptsByPromoCodeIdAsync(promoCode.PromoCodeId);
                if(receipts.IsNullOrEmpty() )
                {
                    _unitOfWork.PromoCodes.Remove(promoCode);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                throw new InvalidOperationException("This promo code can not be deleted as it has referenced transactions.");
            }
            return false;

        }

        public async Task<PromoCodeModel> GenratePromoCode()
        {
            PromoCodeModel model = new PromoCodeModel();

            for(int i=0; i < Constants.PromoCodeTrial; i++)
            {
                var promoCode = GetRandomPromoCode();

                //Check for duplicate code
                if(await _unitOfWork.PromoCodes.GetPromoCodeByCodeAsync(promoCode) == null)
                {
                    model.Code = promoCode;
                    return model;
                }
            }

            return model;
        }

        private string GetRandomPromoCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < Constants.PromoCodeLength; i++)
            {
               sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }
        private async Task<bool> ValidPromoCode(PromoCodeModel model, bool create)
        {
         
            //Validate  Code
            if (model.Code.IsNullOrEmpty())
            {
                throw new InvalidOperationException($"Promo Code can not be empty.");
            }

            if (model.Description.IsNullOrEmpty())
            {
                throw new NullReferenceException($"Promo Code Description can not be empty.");
            }
            // Validate Dates if Entered

            if (model.StartDate.HasValue)
            {
                if (create)
                {
                    if (model.StartDate < DateTime.Now.Date)
                    {
                        throw new InvalidOperationException($"Start date cant be a past date.");
                    }
                }
             
                if (model.ExpirationDate.HasValue)
                {
                    if (model.StartDate >= model.ExpirationDate)
                    {
                        throw new InvalidOperationException($"Start date cant be greater than expiration date.");
                    }
                }
               
            }

            if (model.ExpirationDate.HasValue)
            {
                if(model.Status == (int)Status.Active)
                {
                    if (model.ExpirationDate < DateTime.Now.Date)
                    {
                        throw new InvalidOperationException($"Expiration date cant be a past date.");
                    }
                }
              
            }

            //Check if PromoCode already exists

            var promoCode = await _unitOfWork.PromoCodes.GetPromoCodeByCodeAsync(model.Code);

            if (promoCode != null)
            {
                //check if code already exists
                if (promoCode.PromoCodeId != model.PromoCodeId)
                {
                    if (promoCode.Code == model.Code)
                    {
                        throw new InvalidOperationException($"Promo Code already exists.");
                    }
                }
            }

            return true;
        }


    }
}
