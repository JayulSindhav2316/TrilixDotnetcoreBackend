using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetAllOrganizations();
        Task<Organization> GetOrganizationById(int id);
        Task<Organization> GetOrganizationByName(string  name);
        Task<Organization> CreateOrganization(OrganizationModel organizationModel);
        Task<bool> UpdateOrganization(OrganizationModel organizationModel);
        Task<List<SelectListModel>> GetSelectList();
    }
}