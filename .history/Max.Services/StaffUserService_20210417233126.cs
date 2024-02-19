
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
            //Validate User Name
            if(staffUserModel.UserName.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"User Name can not be empty.");
            }

            if (staffUserModel.UserName == null)
            {
                throw new NullReferenceException($"User Name can not be NULL.");
            }

            //Check if User already exists

            var user = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync(staffUserModel.UserName);
            
            if(user != null)
            {
                throw new InvalidOperationException($"Duplicate User Name.");
            }

            //Validate Email
            if (staffUserModel.Email.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Email can not be empty.");
            }

            if (staffUserModel.Email == null)
            {
                throw new NullReferenceException($"Email can not be NULL.");
            }

            //Check if email already exists

             user = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(staffUserModel.Email);

            if (user != null)
            {
                throw new InvalidOperationException($"Duplicate User Name.");
            }

            Staffuser staff = new Staffuser();
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

        public async Task UpdateStaffUser(Staffuser staffuser)
        {
             _unitOfWork.Staffusers.Update(staffuser);
            await _unitOfWork.CommitAsync();
        }
    }
}