using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyModel> CreateCompany(CompanyModel model);
        Task<bool> UpdateCompany(CompanyModel model);
        Task<bool> DeleteCompany(int id);
        Task<List<CompanyModel>> GetCompaniesByName(string name, string exceptMemberIds);
        Task<CompanyModel> GetCompanyById(int id);
        Task<CompanyProfileModel> GetCompanyProfileById(int id);
        Task<List<SelectListModel>> GetCompanyEmployeesById(int id);
        Task<List<SelectListModel>> GetAllCompanies();
        Task<AddressModel> GetPrimaryAddressByCompanyId(int id);
        Task<List<CompanyModel>> GetCompaniesByQuickSearchAsync(string searchParameter);
        Task<CompanyModel> GetCompanyByEmailIdAsync(string email);
        Task<CompanyModel> GetLastAddedCompanyAsync();
    }
}
