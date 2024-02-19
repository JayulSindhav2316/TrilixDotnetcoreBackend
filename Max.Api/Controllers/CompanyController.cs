using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Services;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {

        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyService _companyService;
        private readonly IOrganizationService _organizationService;

        public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService, IOrganizationService organizationService)
        {
            _logger = logger;
            _companyService = companyService;
            _organizationService = organizationService;
        }


        [HttpPost("CreateCompany")]
        public async Task<ActionResult<CompanyModel>> CreateCompany(CompanyModel model)
        {
            CompanyModel comapny = new CompanyModel();
            try
            {
                comapny = await _companyService.CreateCompany(model);
                if (comapny.CompanyId == 0)
                {
                    return BadRequest(new { message = "Failed to create Comapny" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(comapny);
        }

        [HttpPost("UpdateCompany")]
        public async Task<ActionResult<CompanyModel>> UpdateCompany(CompanyModel model)
        {
            bool response = false;
            try
            {
                response = await _companyService.UpdateCompany(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Company" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(response);
        }
        [HttpPost("DeleteCompany")]
        public async Task<ActionResult<bool>> DeleteCompany(CompanyModel model)
        {
            bool response = false;
            try
            {
                response = await _companyService.DeleteCompany(model.CompanyId);
                if (!response)
                {
                    var organizatioDetails = await _organizationService.GetOrganizationById(model.OrganizationId);
                    var accountName = "company";
                    if (organizatioDetails != null && !string.IsNullOrEmpty(organizatioDetails.AccountName))
                    {
                        accountName = organizatioDetails.AccountName;
                    }
                    return BadRequest(new { message = "Sorry, we are not able to delete " + accountName + " profile. Please contact trilix support team." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpGet("GetCompaniesByName")]
        public async Task<ActionResult<IEnumerable<CompanyModel>>> GetCompaniesByName(string companyName, string exceptMemberIds)
        {
            var companies = await _companyService.GetCompaniesByName(companyName, exceptMemberIds);
            return Ok(companies);
        }
        [HttpGet("GetCompanyById")]
        public async Task<ActionResult<CompanyModel>> GetCompanyById(int companyId)
        {
            var company = await _companyService.GetCompanyById(companyId);
            return Ok(company);
        }
        [HttpGet("GetCompanyProfileById")]
        public async Task<ActionResult<CompanyModel>> GetCompanyProfileById(int companyId)
        {
            var company = await _companyService.GetCompanyProfileById(companyId);
            return Ok(company);
        }

        [HttpGet("GetCompanyEmployeesById")]
        public async Task<ActionResult<SelectListModel>> GetCompanyEmployeesById(int entityId)
        {
            var company = await _companyService.GetCompanyEmployeesById(entityId);
            return Ok(company);
        }
        [HttpGet("GetAllCompanies")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetAllCompanies()
        {
            var companies = await _companyService.GetAllCompanies();
            return Ok(companies);
        }

        [HttpGet("GetPrimaryAddressByCompanyId")]
        public async Task<ActionResult<AddressModel>> GetPrimaryAddressByCompanyId(int companyId)
        {
            var AddressModel = await _companyService.GetPrimaryAddressByCompanyId(companyId);
            return Ok(AddressModel);
        }
        [HttpGet("GetLastAddedCompany")]
        public async Task<ActionResult<CompanyModel>> GetLastAddedCompany()
        {
            try
            {
                var companyModel = await _companyService.GetLastAddedCompanyAsync();
                return Ok(companyModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
