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
namespace Max.Services
{
    public class MembershipPeriodService : IMembershipPeriodService
    {
      
        private readonly IUnitOfWork _unitOfWork;
        public MembershipPeriodService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Membershipperiod>> GetAllMembershipPeriods()
        {
            return await _unitOfWork.MembershipPeriods
                .GetAllMembershipPeriodsAsync();
        }

        public async Task<Membershipperiod> GetMembershipPeriodById(int id)
        {
            return await _unitOfWork.MembershipPeriods
                .GetMembershipPeriodByIdAsync(id);
        }

        public async Task<Membershipperiod> CreateMembershipPeriod(MembershipPeriodModel model)
        {
            Membershipperiod membershiPeriod = new Membershipperiod();
            var isValid = ValidMembershipPeriod(model);
            if (isValid)
            {
                //Map Model Data
                membershiPeriod.Name = model.Name;
                membershiPeriod.Description = model.Description;
                membershiPeriod.PeriodUnit = model.PeriodUnit;
                membershiPeriod.Duration = model.Duration;
                membershiPeriod.Status = model.Status;

                await _unitOfWork.MembershipPeriods.AddAsync(membershiPeriod);
                await _unitOfWork.CommitAsync();
            }
            return membershiPeriod;
        }

        public async Task<List<SelectListModel>> GetSelectList()
        {
            var periods= await _unitOfWork.MembershipPeriods.GetAllMembershipPeriodsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var period in periods)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = period.MembershipPeriodId.ToString();
                selectListItem.name = period.Name;
                selectList.Add(selectListItem);
            }

            return selectList;
        }

        public async Task<List<SelectListModel>> GetFrequencySelectList(int periodId)
        {
            var membershipPeriod = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(periodId);
            var periods = await _unitOfWork.MembershipPeriods.GetAllMembershipPeriodsAsync();
            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var period in periods)
            {
                if(membershipPeriod.PeriodUnit=="Year")
                {
                    //Allow all except week & days
                    if(period.PeriodUnit=="Day")
                    {
                        continue;
                    }
                    SelectListModel selectListItem = new SelectListModel();
                    selectListItem.code = period.MembershipPeriodId.ToString();
                    selectListItem.name = period.Name;
                    selectList.Add(selectListItem);
                }
                if (membershipPeriod.PeriodUnit == "Month")
                {
                    //Allow all except Year, week & days
                    if (period.PeriodUnit == "Day" || period.PeriodUnit == "Year")
                    {
                        continue;
                    }
                    if(period.Duration > membershipPeriod.Duration)
                    {
                        continue;
                    }
                    if (period.Duration == 2) //Skip Bi-Monthly
                    {
                        continue;
                    }
                    SelectListModel selectListItem = new SelectListModel();
                    selectListItem.code = period.MembershipPeriodId.ToString();
                    selectListItem.name = period.Name;
                    selectList.Add(selectListItem);
                }
                if (membershipPeriod.PeriodUnit == "Day")
                {
                    SelectListModel selectListItem = new SelectListModel();
                    selectListItem.code = membershipPeriod.MembershipPeriodId.ToString();
                    selectListItem.name = membershipPeriod.Name;
                    selectList.Add(selectListItem);
                    return selectList;
                }

            }

            return selectList;
        }

        private bool ValidMembershipPeriod(MembershipPeriodModel model)
        {
            //Validate  Name
            if (model.Name == null)
            {
                throw new NullReferenceException($"Membership Period Name can not be NULL.");
            }

            if (model.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Membership Period Name can not be empty.");
            }

            return true;
        }

    }
}
