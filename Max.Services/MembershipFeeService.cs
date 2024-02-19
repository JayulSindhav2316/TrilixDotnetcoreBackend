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
    public class MembershipFeeService : IMembershipFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MembershipFeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<Membershipfee>> GetAllMembershipFees()
        {
            return await _unitOfWork.Membershipfees
                .GetAllMembershipFeesAsync();
        }

        public async Task<IEnumerable<MembershipFeeModel>> GetMembershipFeesByMembershipTypeId(int id)
        {
            var fees =  await _unitOfWork.Membershipfees.GetMembershipFeeByMembershipTypeIdAsync(id);

            var model = _mapper.Map<List<MembershipFeeModel>>(fees);
            return model;
        }

        public async Task<IEnumerable<MembershipFeeModel>> GetMembershipFeesByFeeIds(string feeIds)
        {
            int[] membershipFeeIds = feeIds.Split(',').Select(int.Parse).ToArray();

            var fees = await _unitOfWork.Membershipfees.GetMembershipFeeByFeeIdsAsync(membershipFeeIds);

            var model = _mapper.Map<List<MembershipFeeModel>>(fees);
            return model;
        }

        public async Task<Membershipfee> GetMembershipFeeById(int id)
        {
            return await _unitOfWork.Membershipfees
                .GetMembershipFeeByIdAsync(id);
        }

        public async Task<Membershipfee> CreateMembershipFee(MembershipFeeModel membershipFeeModel)
        {
            Membershipfee membershiFee = new Membershipfee();
            var isValid = ValidMembershipFee(membershipFeeModel);
            if (isValid)
            {
                //Map Model Data
                membershiFee.Name = membershipFeeModel.Name;
                membershiFee.GlAccountId = membershipFeeModel.GlAccountId;
                membershiFee.Description = membershipFeeModel.Description;
                membershiFee.Name = membershipFeeModel.Name;// TODO: AKS populate from UI
                membershiFee.FeeAmount = membershipFeeModel.FeeAmount;
                membershiFee.IsMandatory = membershipFeeModel.IsMandatory;
                membershiFee.BillingFrequency = membershipFeeModel.BillingFrequency;
                membershiFee.Status = membershipFeeModel.Status;

                await _unitOfWork.Membershipfees.AddAsync(membershiFee);
                await _unitOfWork.CommitAsync();
            }
            return membershiFee;
        }

        public async Task<Membershipfee> UpdateMembershipFee(MembershipFeeModel membershipFeeModel)
        {
            Membershipfee membershiFee = await _unitOfWork.Membershipfees.GetMembershipFeeByIdAsync(membershipFeeModel.FeeId);

            if (membershiFee == null)
            {
                throw new InvalidOperationException($"MembershipFe: {membershipFeeModel.FeeId} not found.");
            }
            var isValid =  ValidMembershipFee(membershipFeeModel);
            if (isValid)
            {
                //Map Model Data
                membershiFee.Name = membershipFeeModel.Name;
                membershiFee.GlAccountId = membershipFeeModel.GlAccountId;
                membershiFee.Description = membershipFeeModel.Description;
                membershiFee.FeeAmount = membershipFeeModel.FeeAmount;
                membershiFee.IsMandatory = membershipFeeModel.IsMandatory;
                membershiFee.BillingFrequency = membershipFeeModel.BillingFrequency;
                membershiFee.Status = membershipFeeModel.Status;

                _unitOfWork.Membershipfees.Update(membershiFee);
                await _unitOfWork.CommitAsync();
            }
            return membershiFee;
        }

        public async Task<bool> DeleteMembershipFee(int feeId)
        {
            Membershipfee membershipFee = await _unitOfWork.Membershipfees.GetMembershipFeeByIdAsync(feeId);
            
            if (membershipFee.MembershipTypeId != null) { 
                var fees = await _unitOfWork.Membershipfees.GetMembershipFeeByMembershipTypeIdAsync((int)membershipFee.MembershipTypeId);

                var model = _mapper.Map<List<MembershipFeeModel>>(fees);

                if (model.Count == 1)
                    {
                        throw new InvalidOperationException($"You cannot delete this fee.");
                    }
            }
            //check if there is a related fee
            if (membershipFee.Billingfees.Count >0)
            {
                throw new InvalidOperationException($"There are transaction related to the fee. It can not be deleted.");
            }
            
            if (membershipFee == null)
            {
                throw new InvalidOperationException($"Membership Fee: {feeId} not found.");
            }
            try
            {
                _unitOfWork.Membershipfees.Remove(membershipFee);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete Fee.");
            }

        }

        private bool ValidMembershipFee(MembershipFeeModel membershipFeeModel)
        {
            //Validate  Name
            if (membershipFeeModel.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"MembershipFee Name can not be empty.");
            }

            if (membershipFeeModel.Name == null)
            {
                throw new NullReferenceException($"MembershipFee Name can not be NULL.");
            }

            if (membershipFeeModel.GlAccountId==0)
            {
                throw new InvalidOperationException($"MembershipFee GlAccount is Required.");
            }

            return true;
        }
    }
}
