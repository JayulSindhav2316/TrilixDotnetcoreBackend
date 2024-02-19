using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;


namespace Max.Services.Interfaces
{
    public interface IContactRoleService
    {
        Task<List<Contactrole>> GetAllContactRoles();
        Task<Contactrole> GetContactRoleById(int id);
        Task<Contactrole> CreateContactRole(ContactRoleModel model);
        Task<bool> UpdateContactRole(ContactRoleModel model);
        Task<bool> DeleteContactRole(int contactRoleId);
        Task<List<SelectListModel>> GetContactRoleSelectList();
    }
}
