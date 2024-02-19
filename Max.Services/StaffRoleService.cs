
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
    public class StaffRoleService : IStaffRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StaffRoleService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Staffrole> CreateStaffRole(int staffId, int  roleId)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            var staff  = await _unitOfWork.Staffusers.GetByIdAsync(staffId);
            var staffrole = new Staffrole();

            if (role != null &&  staff != null)
            {
               
                staffrole.StaffId = staffId;
                staffrole.RoleId = roleId;
                await _unitOfWork.Staffroles.AddAsync(staffrole);
                await _unitOfWork.CommitAsync();
            }
            return staffrole;
        }
        
        public async Task DeleteStaffRole(int id)
        {
            var staffRole = await _unitOfWork.Staffroles.GetByIdAsync(id);
            if (staffRole != null)
            {
                _unitOfWork.Staffroles.Remove(staffRole);
                await _unitOfWork.CommitAsync();
            }

        }

        public async Task<IEnumerable<Staffrole>> GetAllStaffRoles()
        {
            return await _unitOfWork.Staffroles
                .GetAllStaffRolesAsync();
        }

        public async Task<Staffrole> GetStaffRoleById(int id)
        {
            return await _unitOfWork.Staffroles
                .GetStaffRoleByIdAsync(id);
        }

        public async Task<IEnumerable<Staffrole>> GetStaffRoleByStaffId(int id)
        {
            return await _unitOfWork.Staffroles
                .GetAllStaffRolesByStaffIdAsync(id);
        }


    }
}