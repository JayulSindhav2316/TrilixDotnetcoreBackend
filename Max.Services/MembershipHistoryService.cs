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
    public class MembershipHistoryService : IMembershipHistoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        public MembershipHistoryService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Membershiphistory>> GetAllMembershipHistorys()
        {
            return await _unitOfWork.MembershipHistories
                .GetAllMembershipHistoriesAsync();
        }

        public async Task<Membershiphistory> GetMembershipHistoryById(int id)
        {
            return await _unitOfWork.MembershipHistories
                .GetMembershipHistoryByIdAsync(id);
        }

        public async Task<IEnumerable<Membershiphistory>> GetMembershipHistoryByPersonId(int personId)
        {
            return await _unitOfWork.MembershipHistories
                .GetAllMembershipHistoryByPersonIdAsync(personId);
        }

        public async Task<Membershiphistory> CreateMembershipHistory(MembershipHistoryModel model)
        {
            Membershiphistory membershipHistory = new Membershiphistory();
            var isValid = ValidMembershipHistory(model);
            if (isValid)
            {
                //Map Model Data
                membershipHistory.MembershipId = model.MembershipId;
                membershipHistory.StatusDate = model.StatusDate;
                membershipHistory.Status = model.Status;
                membershipHistory.ChangedBy = model.ChangedBy;
                membershipHistory.Reason = model.Reason;
                membershipHistory.PreviousMembershipId = model.PreviousMembershipId;

                await _unitOfWork.MembershipHistories.AddAsync(membershipHistory);
                await _unitOfWork.CommitAsync();
            }
            return membershipHistory;
        }

        public async Task<Membershiphistory> UpdateMembershipHistory(MembershipHistoryModel model)
        {
            Membershiphistory membershipHistory = await _unitOfWork.MembershipHistories.GetMembershipHistoryByIdAsync(model.MembershipHistoryId);

            var isValid = ValidMembershipHistory(model);
            if (isValid)
            {
                //Map Model Data
                //Map Model Data
                membershipHistory.MembershipId = model.MembershipId;
                membershipHistory.StatusDate = model.StatusDate;
                membershipHistory.Status = model.Status;
                membershipHistory.ChangedBy = model.ChangedBy;
                membershipHistory.Reason = model.Reason;
                membershipHistory.PreviousMembershipId = model.PreviousMembershipId;

                _unitOfWork.MembershipHistories.Update(membershipHistory);
                await _unitOfWork.CommitAsync();
            }
            return membershipHistory;
        }

        public async Task<bool> DeleteMembershipHistory(int id)
        {
            Membershiphistory membershipHistory = await _unitOfWork.MembershipHistories.GetMembershipHistoryByIdAsync(id);

            if (membershipHistory != null)
            {
                _unitOfWork.MembershipHistories.Remove(membershipHistory);
                await _unitOfWork.CommitAsync();
                return true;

            }
            throw new InvalidOperationException($"Membership: {id} not found.");

        }
        private bool ValidMembershipHistory(MembershipHistoryModel model)
        {
            //Validate  Name
            if (model.MembershipId == 0)
            {
                throw new InvalidOperationException($"Membership Id Id not defined.");
            }

            if (model.ChangedBy==0)
            {
                throw new NullReferenceException($"Changed by User not defined.");
            }

            return true;
        }
    }
}
