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

namespace Max.Services
{
    public class BillingFeeService : IBillingFeeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BillingFeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        } 
        public async Task<Billingfee> CreateBillingFee(BillingFeeModel model) 
        {
            Billingfee billingfee = new Billingfee();

            //billingfee.BillingFeeId = model.BillingFeeId;
            billingfee.MembershipId= model.MembershipId;
            billingfee.MembershipFeeId = model.MembershipFeeId;
            billingfee.Fee = model.Fee;
            billingfee.Status = model.Status;

            await _unitOfWork.BillingFees.AddAsync(billingfee);
               await _unitOfWork.CommitAsync();
            
            return billingfee;
        }     

        public async Task<List<BillingFeeModel>> GetBillingFeeByMembershipId(int membershipId)
        {
            List<BillingFeeModel> billingFees = new List<BillingFeeModel>();

            var fee = await _unitOfWork.BillingFees.GetBillingFeesDetailsByMembershipIdAsync(membershipId);

            billingFees = _mapper.Map<List<BillingFeeModel>>(fee);

            return billingFees;
        }

        public async Task<Billingfee> UpdateBillingFee(BillingFeeModel model)
        {
            Billingfee billingfee = await _unitOfWork.BillingFees.GetByIdAsync(model.BillingFeeId);
           
            billingfee.Fee = model.Fee;

            _unitOfWork.BillingFees.Update(billingfee);
            await _unitOfWork.CommitAsync();
            return billingfee;
        }
    }
}
