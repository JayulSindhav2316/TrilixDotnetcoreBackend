
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
    public class StaffUserService : IStaffUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StaffUserService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Staffuser> CreateStaffUser(StaffUserModel staffUserModel)
        {
            Staffuser staff =  new Staffuser();

            //Map Model Data

            staff.UserName = staffUserModel.UserName;
            staff.FirstName =  staff.FirstName;
            staff.LastName = staffUserModel.LastName;
            staff.Password  = staffUserModel.Password;
            staff.Email = staffUserModel.Email;
            staff.Department  =  staffUserModel.Department;
            staff.LastAccessed = Constants.MySQL_MinDate;
            staff.CreatedBy = "GetFromHttpContext";
            staff.CreatedOn = DateTime.Now;
            staff.ModifiedBy = String.Empty;
            staff.ModifiedOn =  Constants.MySQL_MinDate;

            await _unitOfWork.Staffusers.AddAsync(staff);
            await _unitOfWork.CommitAsync();
            return staff;
        }

        public async Task DeleteMusic(Staffuser staffuser)
        {
            _unitOfWork.Staffusers.Remove(staffuser);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<Staffuser>> GetAllStaffUsers()
        {
            return await _unitOfWork.Staffusers
                .GetAllStaffUsersAsync();
        }

        public async Task<Staffuser> GetStaffUserById(int id)
        {
            return await _unitOfWork.Staffusers
                .GetStaffUserByIdAsync(id);
        }

        public async Task UpdateStaffUser(Staffuser staffuserToUpdate, Staffuser staffuser)
        {
            staffuserToUpdate.FirstName = staffuser.FirstName;
            staffuserToUpdate.LastName = staffuser.LastName;

            await _unitOfWork.CommitAsync();
        }
    }
}