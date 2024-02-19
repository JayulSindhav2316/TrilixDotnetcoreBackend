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
using System.Text;

namespace Max.Services
{
    public class MembershipTypeService : IMembershipTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MembershipTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<MembershipTypeModel>> GetAllMembershipTypes()
        {
            var membershipTypes = await _unitOfWork.Membershiptypes.GetAllMembershipTypesAsync();
            var periodList = await _unitOfWork.MembershipPeriods.GetAllAsync();
            var membershipTypeModel = _mapper.Map<List<MembershipTypeModel>>(membershipTypes);
            foreach(var model in membershipTypeModel)
            {
                model.PaymentFrequencyName = periodList.Where(x => x.MembershipPeriodId == model.PaymentFrequency).Select(x => x.Name).FirstOrDefault();
            }
            return membershipTypeModel.OrderBy(x => x.CategoryName).ThenBy( x=> x.PaymentFrequency);
        }

        public async Task<MembershipTypeModel> GetMembershipTypeById(int  id)
        {
            MembershipTypeModel model = new MembershipTypeModel();
            var type = await _unitOfWork.Membershiptypes.GetMembershipTypeByIdAsync(id);
            if(type != null)
            {
                model.MembershipTypeId = type.MembershipTypeId;
                model.Code = type.Code;
                model.Name = type.Name;
                model.Description = type.Description;
                model.Period = type.Period;
                model.PaymentFrequency = type.PaymentFrequency;
                model.PeriodName = type.PeriodNavigation.Name;
                model.TotalFee = type.Membershipfees.Sum(x => x.FeeAmount);
                model.StartDate = DateTime.Now;
                model.EndDate = await _unitOfWork.MembershipPeriods.GetMembershipEndDateByIdAsync(type.Period??0, DateTime.Now);
                model.Units = type.Units;
            }

            return model;
        }
        
        public async Task<IEnumerable<MembershipTypeModel>> GetMembershipTypesByCategoryIds(string ids)
        {
            int[] categoryIds = ids.Split(',').Select(int.Parse).ToArray();
            var membershipTypes =  await _unitOfWork.Membershiptypes
                                        .GetMembershipTypesByCategoriesAsync(categoryIds);
            List<MembershipTypeModel> typeList = new List<MembershipTypeModel>();
            var periodList = await _unitOfWork.MembershipPeriods.GetAllAsync();

            foreach (var item in membershipTypes)
            {
                if(item.Status > 0)
                {
                    // Get membership fee or first fee which is recurring
                    var membershipFee = item.Membershipfees.Where(x => x.Description.Contains("Membership")).Select(r => r.FeeAmount).FirstOrDefault();
                    if (membershipFee == 0) 
                    {
                        membershipFee = item.Membershipfees.Where(x => x.BillingFrequency == (int)FeeBillingFrequency.Recurring).Select(r => r.FeeAmount).FirstOrDefault();
                    }
                    MembershipTypeModel model = new MembershipTypeModel();
                    model.MembershipTypeId = item.MembershipTypeId;
                    model.Code = item.Code;
                    model.Name = item.Name;
                    model.Description = item.Description;
                    model.PaymentFrequency = item.PaymentFrequency;
                    model.PaymentFrequencyName = periodList.Where(x => x.MembershipPeriodId == item.PaymentFrequency).Select(x => x.Name).FirstOrDefault();
                    model.CategoryName = item.CategoryNavigation.Name;
                    model.PeriodName = item.PeriodNavigation.Name;
                    model.TotalFee = item.Membershipfees.Sum(x => x.FeeAmount);
                    model.MembershipFee = membershipFee;
                    model.Units = item.Units;
                    //Map all other Fee
                    model.MembershipFees = _mapper.Map<List<MembershipFeeModel>>(item.Membershipfees);

                    StringBuilder feeTag = new StringBuilder();
                    feeTag.Append("<table class='mem-fee-box' style='border-collapse:collapse; table-layout:fixed; width:200px;'>");
                    foreach (var feeItem in model.MembershipFees)
                    {
                        feeTag.Append($"<tr><td style='word-break: break-word;width:100px;'>{feeItem.Description}</td><td style='width:70px;'>${feeItem.FeeAmount.ToString()}</td></tr>");
                    }
                    feeTag.Append("</table>");

                    model.FeeDetailTag = feeTag.ToString();

                    typeList.Add(model);
                }
               
            }
            return typeList.OrderBy(x => x.CategoryName).ThenBy(x => x.PeriodName);

        }

        public async Task<IEnumerable<SelectListModel>> GetMembershipTypeSelectListByCategoryIds(string ids)
        {
            int[] categoryIds = ids.Split(',').Select(int.Parse).ToArray();
            var membershipTypes = await _unitOfWork.Membershiptypes
                                        .GetMembershipTypesByCategoriesAsync(categoryIds);
            List<SelectListModel> typeList = new List<SelectListModel>();
            var periodList = await _unitOfWork.MembershipPeriods.GetAllAsync();

            foreach (var item in membershipTypes)
            {
                if (item.Status > 0)
                {
                    SelectListModel model = new SelectListModel();
                    model.code = item.MembershipTypeId.ToString();
                    model.name = item.Description;
                    typeList.Add(model);
                }

            }
            return typeList.OrderBy(x => x.name);

        }
        public async Task<Membershiptype> CreateMembershipType(MembershipTypeModel membershipTypeModel)
        {
            Membershiptype membershipType = new Membershiptype();
            var isValid = await ValidMembershipType(membershipTypeModel);
            if (isValid)
            {
                
                //Map Model Data
                membershipType.Name = membershipTypeModel.Name.Trim();
                membershipType.Code = membershipTypeModel.Code.Trim();
                membershipType.Description = membershipTypeModel.Description;
                membershipType.Period = membershipTypeModel.Period;
                membershipType.PaymentFrequency = membershipTypeModel.PaymentFrequency;
                membershipType.Category = membershipTypeModel.Category;
                membershipType.Status = membershipTypeModel.Status;
                membershipType.Units = membershipTypeModel.Units;

                foreach (var fee in membershipTypeModel.MembershipFees)
                {
                    Membershipfee membershipFee = new Membershipfee();

                    membershipFee.MembershipTypeId = membershipType.MembershipTypeId;
                    membershipFee.Name = fee.Description.Trim();//TODO: AKS Get from UI
                    membershipFee.Description = fee.Description.Trim();
                    membershipFee.GlAccountId = fee.GlAccountId;
                    membershipFee.FeeAmount = fee.FeeAmount;
                    membershipFee.BillingFrequency = fee.BillingFrequency;
                    membershipFee.IsMandatory = fee.IsMandatory;
                    membershipFee.Status = fee.Status;

                    membershipType.Membershipfees.Add(membershipFee);
                }
                await _unitOfWork.Membershiptypes.AddAsync(membershipType);
                await _unitOfWork.CommitAsync();
            }
            return membershipType;
        }

        public async Task<Membershiptype> UpdateMembershipType(MembershipTypeModel membershipTypeModel)
        {
            Membershiptype membershipType = await _unitOfWork.Membershiptypes.GetMembershipTypeByIdAsync(membershipTypeModel.MembershipTypeId);

            if (membershipType   == null)
            {
                throw new InvalidOperationException($"MembershipType: {membershipTypeModel.MembershipTypeId} not found.");
            }
            var isValid = await ValidMembershipType(membershipTypeModel);
            if (isValid)
            {
                //Map Model Data
                membershipType.Name = membershipTypeModel.Name;
                membershipType.Code = membershipTypeModel.Code;
                membershipType.Description = membershipTypeModel.Description;
                membershipType.Period = membershipTypeModel.Period;
                membershipType.PaymentFrequency = membershipTypeModel.PaymentFrequency;
                membershipType.Category = membershipTypeModel.Category;
                membershipType.Status = membershipTypeModel.Status;
                membershipType.Units = membershipTypeModel.Units;
                var currentFees = membershipType.Membershipfees.ToList();

                foreach (var fee in currentFees)
                {
                    if (membershipTypeModel.MembershipFees.Any(x => x.FeeId == fee.FeeId))
                    {
                        //Update  Fee
                        MembershipFeeModel mmembershipFee = membershipTypeModel.MembershipFees.Where(x => x.FeeId == fee.FeeId).FirstOrDefault();
                        //Map data
                        fee.Name = mmembershipFee.Description.Trim();
                        fee.Description = mmembershipFee.Description.Trim();
                        fee.GlAccountId = mmembershipFee.GlAccountId;
                        fee.FeeAmount = mmembershipFee.FeeAmount;
                        fee.BillingFrequency = mmembershipFee.BillingFrequency;
                        fee.IsMandatory = mmembershipFee.IsMandatory;
                        fee.Status = mmembershipFee.Status;

                        _unitOfWork.Membershipfees.Update(fee);
                        continue;

                    }
                    if(fee.FeeId > 0)
                    {
                        _unitOfWork.Membershipfees.Remove(fee);
                        membershipType.Membershipfees.Remove(fee);
                    }
                   
                }

                foreach (var fee in membershipTypeModel.MembershipFees.Where(x=>x.FeeId==0).ToList())
                {

                    Membershipfee mmembershipFee = new Membershipfee();
                    //Map data
                    mmembershipFee.Name = fee.Description.Trim();
                    mmembershipFee.Description = fee.Description.Trim();
                    mmembershipFee.GlAccountId = fee.GlAccountId;
                    mmembershipFee.FeeAmount = fee.FeeAmount;
                    mmembershipFee.BillingFrequency = fee.BillingFrequency;
                    mmembershipFee.IsMandatory = fee.IsMandatory;
                    mmembershipFee.Status = fee.Status;

                    membershipType.Membershipfees.Add(mmembershipFee);
   
                }

                try
                {
                    _unitOfWork.Membershiptypes.Update(membershipType);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Unable to add Fee.");
                }
            }
            return membershipType;
        }

        public async Task<bool> DeleteMembershipType(int membershipTypeId)
        {
            Membershiptype membershipType = await _unitOfWork.Membershiptypes.GetMembershipTypeByIdAsync(membershipTypeId);
            var membership = await _unitOfWork.Memberships.GetAllMembershipsAsync();
            if (membership.Where(a => a.MembershipTypeId == membershipTypeId).Any())
                throw new InvalidOperationException($" A Membership uses this Membership Type. It cannot be deleted");
            if (membershipType != null)
            {
                _unitOfWork.Membershiptypes.Remove(membershipType);
                await _unitOfWork.CommitAsync();
                return true;
                
            }
            throw new InvalidOperationException($"MembershipType: {membershipTypeId} not found.");

        }

        private async Task<bool> ValidMembershipType(MembershipTypeModel membershipTypeModel)
        {
            //Validate  Name

            if (membershipTypeModel.Name == null)
            {
                throw new NullReferenceException($"MembershipType Name can not be NULL.");
            }

            if (membershipTypeModel.Code == null)
            {
                throw new NullReferenceException($"MembershipType Code can not be NULL.");
            }

            if (membershipTypeModel.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"MembershipType Name can not be empty.");
            }

            if (membershipTypeModel.Code.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"MembershipType Code can not be empty.");
            }

            //Check if MembershipType already exists

            var mmembershipType = await _unitOfWork.Membershiptypes.GetMembershipTypeByNameAndCategoryAndFrequencyAsync(membershipTypeModel.Name, membershipTypeModel.Category??0, membershipTypeModel.PaymentFrequency??0);

            if (mmembershipType != null)
            {
                //check id ID & UserName are same -> Updating
                if(membershipTypeModel.MembershipTypeId>0)
                { 
                if (mmembershipType.MembershipTypeId != membershipTypeModel.MembershipTypeId)
                {
                    throw new InvalidOperationException($"Duplicate Membership Type Name.");
                }
                }

            }
            // Check if there is an existing membership type with same code
            var membershipTypes = await _unitOfWork.Membershiptypes.GetAllAsync();
            if (membershipTypes.Where(x => x.Code == membershipTypeModel.Code.Trim() && x.MembershipTypeId != membershipTypeModel.MembershipTypeId).Any())
            {
                throw new InvalidOperationException($"A Membership Type already exists with Code:{membershipTypeModel.Code}.");
            }

            return true;
        }
    }
}
