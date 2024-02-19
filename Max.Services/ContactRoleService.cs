using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Max.Services
{
    public class ContactRoleService : IContactRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactRoleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Contactrole> CreateContactRole(ContactRoleModel model)
        {
            Contactrole role = new Contactrole();

            var isValidrole = await ValidateRole(model);
            if (isValidrole)
            {
                role.Name = model.Name;
                role.Description = model.Description;
                role.Active = model.Active;
                await _unitOfWork.ContactRoles.AddAsync(role);
                await _unitOfWork.CommitAsync();
            }

            return role;
        }

        public async Task<bool> UpdateContactRole(ContactRoleModel model)
        {
            Contactrole role = await _unitOfWork.ContactRoles.GetContactRoleByIdAsync(model.ContactRoleId);

            if (role != null)
            {
                var isValidrole = await ValidateRole(model);
                if (isValidrole)
                {
                    role.Name = model.Name;
                    role.Description= model.Description;
                    role.Active = model.Active;
                    _unitOfWork.ContactRoles.Update(role);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<Contactrole>> GetAllContactRoles()
        {
            return (List<Contactrole>)await _unitOfWork.ContactRoles.GetAllContactRolesAsync();
        }

        public async Task<List<SelectListModel>> GetContactRoleSelectList()
        {
            List<SelectListModel> contactRoleList = new List<SelectListModel>();

            var contactRoles = await _unitOfWork.ContactRoles.GetActiveContactRolesAsync();

            foreach (var role in contactRoles)
            {
                SelectListModel item = new SelectListModel();
                item.name = role.Name;
                item.code = role.ContactRoleId.ToString();
                contactRoleList.Add(item);
            }

            return contactRoleList;
        }

        public async Task<Contactrole> GetContactRoleById(int id)
        {
            return await _unitOfWork.ContactRoles.GetByIdAsync(id);
        }

        private async Task<bool> ValidateRole(ContactRoleModel model)
        {
            var contactRoleList = await _unitOfWork.ContactRoles.GetAllContactRolesAsync();
            if (contactRoleList != null)
            {
                if (contactRoleList.Any(x => x.Name == model.Name.Trim() && x.ContactRoleId != model.ContactRoleId))
                {
                    throw new InvalidOperationException($"Duplicate Contact Role name.");
                }
            }
            return true;
        }
        public async Task<bool> DeleteContactRole(int contactRoleId)
        {
            var contactRole = await _unitOfWork.ContactRoles.GetByIdAsync(contactRoleId);
            if (contactRole != null)
            {
                try
                {
                    _unitOfWork.ContactRoles.Remove(contactRole);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex) 
                {
                    throw new InvalidOperationException($"Failed to delete contact role.");
                }
            }
            return false;
        }
        
    }
}
