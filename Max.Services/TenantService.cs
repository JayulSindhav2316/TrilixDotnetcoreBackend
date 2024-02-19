using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class TenantService : ITenantService
    {

        private readonly ITenantUnitOfWork _unitOfWork;
        public TenantService(ITenantUnitOfWork unitOfWork)
        {

            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Tenant>> GetAllTenants()
        {
            return await _unitOfWork.Tenants.GetAllAsync();
        }
        public async Task<Tenant> GetTenantByOrganizationName(string organizationName)
        {
            return await _unitOfWork.Tenants.GetTenantByOrganizationNameAsync(organizationName.ToLower());
        }

        public async Task<Tenant> GetTenantByOrganizationId(int organizationId)
        {
            return await _unitOfWork.Tenants.GetTenantByOrganizationIdAsync(organizationId);
        }

        public async Task<Tenant> GetTenantById(string id)
        {
            return await _unitOfWork.Tenants.GetTenantByIdAsync(id);
        }


    }
}