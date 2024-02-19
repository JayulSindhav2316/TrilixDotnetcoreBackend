using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IContactRoleRepository : IRepository<Contactrole>
    {
        Task<IEnumerable<Contactrole>> GetAllContactRolesAsync();
        Task<IEnumerable<Contactrole>> GetActiveContactRolesAsync();
        Task<Contactrole> GetContactRoleByIdAsync(int id);
    }
}
