
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
            Staffuser staff = new Staffuser();
            var isValidUser = await ValidStaffUser(staffUserModel);
            if (isValidUser)
            {
                //Map Model Data
                staff.UserName = staffUserModel.UserName;
                staff.FirstName = staffUserModel.FirstName;
                staff.LastName = staffUserModel.LastName;
                staff.Password = staffUserModel.Password;
                staff.Email = staffUserModel.Email;
                staff.Department = staffUserModel.Department;
                staff.LastAccessed = Constants.MySQL_MinDate;
                staff.CreatedBy = "GetFromHttpContext";
                staff.CreatedOn = DateTime.Now;
                staff.ModifiedBy = String.Empty;
                staff.ModifiedOn = Constants.MySQL_MinDate;

                await _unitOfWork.Staffusers.AddAsync(staff);
                await _unitOfWork.CommitAsync();
            }
            return staff;
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

        public async Task<bool> UpdateStaffUser(StaffUserModel staffUserModel)
        {
            var isValidUser = await ValidStaffUser(staffUserModel);
            if (isValidUser)
            {
                var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(staffUserModel.UserId);

                if(staffUser != null)
                {
                    staffUser.UserName = staffUserModel.UserName;
                    staffUser.FirstName = staffUserModel.FirstName;
                    staffUser.LastName = staffUserModel.LastName;
                    staffUser.Password = staffUserModel.Password;
                    staffUser.Email = staffUserModel.Email;
                    staffUser.Department = staffUserModel.Department;
                    staffUser.ModifiedBy = staffUserModel.ModifiedBy;
                    staffUser.ModifiedOn = staffUserModel.ModifiedOn;
                }
                _unitOfWork.Staffusers.Update(staffUser);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task DeleteStaffUser(Staffuser staffuser)
        {
             _unitOfWork.Staffusers.Remove(staffuser);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> AssignRole(int staffUserId,  int roleId)
        {
            var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(staffUserId);
           // _unitOfWork.Staffusers.Remove(staffuser);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async  Task<bool> Authenticate(string userName, string password)
        {
            var staffUser = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync(userName);
            if(staffUser  ==  null)
            {
                return  false;
            }
            if(staffUser.UserName==userName  && staffUser.Password==password)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> ValidStaffUser(StaffUserModel staffUser)
        {
            //Validate User Name
            if (staffUser.UserName.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"User Name can not be empty.");
            }

            if (staffUser.UserName == null)
            {
                throw new NullReferenceException($"User Name can not be NULL.");
            }

            //Check if User already exists

            var user =   await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync(staffUser.UserName);

            if (user != null)
            {
                //check id ID & UserName are same -> Updating
                if(user.UserId != staffUser.UserId)
                {
                    throw new InvalidOperationException($"Duplicate User Name.");
                }
                
            }

            //Validate Email
            if (staffUser.Email.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Email can not be empty.");
            }

            if (staffUser.Email == null)
            {
                throw new NullReferenceException($"Email can not be NULL.");
            }

            //Check if email already exists

            user = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(staffUser.Email);

            if (user != null)
            {
                if (user.UserId != staffUser.UserId)
                {
                    throw new InvalidOperationException($"Duplicate Email.");
                }
            }

            return true;
        }
    }
}