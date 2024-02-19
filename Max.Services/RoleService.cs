
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

namespace Max.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<Role> CreateRole(RoleModel roleModel)
        {
            Role role = new Role();
            var isValidRole = await ValidRole(roleModel);
            if (isValidRole)
            {
                //Map Model Data
                role.Name = roleModel.Name;
                role.Description = roleModel.Description;
                role.Status = roleModel.Status;

                await _unitOfWork.Roles.AddAsync(role);
                await _unitOfWork.CommitAsync();
            }
            return role;
        }

        public async Task<bool> DeleteRole(int id)
        {
            //Delete all child records from RoleMenu

            var roleMenus = await _unitOfWork.Rolemenus.GetMenuByRoleIdAsync(id);
            if (roleMenus != null)
            {
                _unitOfWork.Rolemenus.RemoveRange(roleMenus);
            }

            // Delete all chidl records from staffRoles
            var staffRoles = await _unitOfWork.Staffroles.GetAllStaffRolesByRoleIdAsync(id);
            if (staffRoles != null)
            {
                _unitOfWork.Staffroles.RemoveRange(staffRoles);
            }

            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role != null)
            {
                _unitOfWork.Roles.Remove(role);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;

        }

        public async Task<IList<RoleModel>> GetAllRoles()
        {
            var roles = await _unitOfWork.Roles.GetAllRolesAsync();
            return _mapper.Map<List<RoleModel>>(roles);
        }

        public async Task<IList<RoleModel>> GetActiveRoles()
        {
            var roles = await _unitOfWork.Roles.GetActiveRolesAsync();
            return _mapper.Map<List<RoleModel>>(roles);
        }
        public async Task<IList<RoleModel>> GetRolesByCompanyId(int companyId)
        {
            var roles = await _unitOfWork.Roles.GetActiveRolesAsync();
            return _mapper.Map<List<RoleModel>>(roles);
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _unitOfWork.Roles
                .GetRoleByIdAsync(id);
        }

        public async Task<bool> UpdateRole(RoleModel roleModel)
        {
            var isValidRole = await ValidRole(roleModel);
            if (isValidRole)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleModel.RoleId);

                if (role != null)
                {
                    role.Name = roleModel.Name;
                    role.Description = roleModel.Description;
                    role.Status = roleModel.Status; ;

                }
                _unitOfWork.Roles.Update(role);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        private async Task<bool> ValidRole(RoleModel roleModel)
        {
            //Validate  Name

            if (roleModel.Name == null)
            {
                throw new NullReferenceException($"Role Name can not be NULL.");
            }

            if (roleModel.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Role Name can not be empty.");
            }

            //Check if Role already exists

            var role = await _unitOfWork.Roles.GetRoleByNameAsync(roleModel.Name);

            if (role != null)
            {
                //check id ID & UserName are same -> Updating
                if (role.RoleId != roleModel.RoleId)
                {
                    throw new InvalidOperationException($"Duplicate Name.");
                }

            }

            return true;
        }
    }
}