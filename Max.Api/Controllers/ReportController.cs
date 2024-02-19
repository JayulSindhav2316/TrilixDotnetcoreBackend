using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {

        private readonly ILogger<ReportController> _logger;
        private readonly IReportService _reportService;

        public ReportController(ILogger<ReportController> logger, IReportService reportService)
        {
            _logger = logger;
            _reportService = reportService;
        }

        [HttpGet("GetAllReports")]
        public async Task<ActionResult<List<ReportModel>>> GetAllReports()
        {
            var reports = await _reportService.GetAllReports();
            return Ok(reports);
        }

        [HttpGet("GetReportById")]
        public async Task<ActionResult<IEnumerable<ReportModel>>> GetReportById(int reportId)
        {
            if (reportId <= 0) 
                return BadRequest(new { message = "Invalid reportid supplied." });
       
            try
            {
                var report = await _reportService.GetReportById(reportId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in getting the report by reportId: {reportId}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetReportFieldsByCategoryId")]
        public async Task<ActionResult<IEnumerable<ReportFieldModel>>> GetReportFieldsByCategoryId(int categoryId)
        {
            if (categoryId <= 0)
                return BadRequest(new { message = "Invalid report category id supplied." });

            try
            {
                var reportFields = await _reportService.GetReportFieldsByCategoryId(categoryId);
                return Ok(reportFields);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in getting the report fields by categoryId: {categoryId}");
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetReportTableFieldsByCategoryId")]
        public async Task<ActionResult<IEnumerable<ReportFieldTableModel>>> GetReportTableFieldsByCategoryId(int categoryId)
        {
            if (categoryId <= 0)
                return BadRequest(new { message = "Invalid report category id supplied." });

            try
            {
                var reportFields = await _reportService.GetReportTableFields(categoryId);
                return Ok(reportFields);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in getting the report fields by categoryId: {categoryId}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CreateReport")]
        public async Task<ActionResult<Report>> CreateReport(ReportConfigurationModel model)
        {
            try
            {
                var report = await _reportService.CreateReport(model);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UpdateReport")]
        public async Task<ActionResult<Report>> UpdateReport(ReportConfigurationModel model)
        {
            try
            {
                var report = await _reportService.UpdateReport(model);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UpdateFavoriteReports")]
        public async Task<ActionResult<Report>> UpdateFavoriteReports(ReportConfigurationModel model)
        {
            try
            {
                var report = await _reportService.UpdateFavoriteReports(model);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetReport")]
        public async Task<ActionResult<ReportModel>> GetReport(int ReportId)
        {
            try
            {
                var report = await _reportService.GetReport(ReportId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetReportConfiguration")]
        public async Task<ActionResult<ReportModel>> GetReportConfiguration(int reportId)
        {
            try
            {
                var report = await _reportService.GetReportConfiguration(reportId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetMembershipReportsByUserId")]
        public async Task<ActionResult<ReportConfigurationModel>> GetMembershipReportsByUserId(int userId)
        {
            try
            {
                var report = await _reportService.GetMembershipReportByUserId(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetContactsAccountsReportsByUserId")]
        public async Task<ActionResult<ReportConfigurationModel>> GetContactsAccountsReportsByUserId(int userId)
        {
            try
            {
                var report = await _reportService.GetContactsAccountsReportByUserId(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetEventReportsByUserId")]
        public async Task<ActionResult<ReportConfigurationModel>> GetEventReportsByUserId(int userId)
        {
            try
            {
                var report = await _reportService.GetEventReportByUserId(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetOpportunityReportsByUserId")]
        public async Task<ActionResult<ReportConfigurationModel>> GetOpportunityReportsByUserId(int userId)
        {
            try
            {
                var report = await _reportService.GetOpportunityReportByUserId(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetContactsAccountsCommunityReports")]
        public async Task<ActionResult<ReportConfigurationModel>> GetContactsAccountsCommunityReports()
        {
            try
            {
                var report = await _reportService.GetContactsAccountsCommunityReports();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetMembershipCommunityReports")]
        public async Task<ActionResult<MembershipReportConfigurationModel>> GetMembershipCommunityReports()
        {
            try
            {
                var report = await _reportService.GetMembershipCommunityReports();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetOpportunitiesCommunityReports")]
        public async Task<ActionResult<OpportunityReportConfigurationModel>> GetOpportunitiesCommunityReports()
        {
            try
            {
                var report = await _reportService.GetOpportunitiesCommunityReports();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CloneReport")]
        public async Task<ActionResult<Membershipreport>> CloneReport(ReportConfigurationModel model)
        {
            try
            {
                var report = await _reportService.CloneReport(model.ReportId, model.UserId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetMembershipSharedReportsByUserId")]
        public async Task<ActionResult<Report>> GetMembershipSharedReportsByUserId(int userId)
        {
            try
            {
                var report = await _reportService.GetMembershipSharedReportByUserId(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetOpportunitiesSharedReportsByUserId")]
        public async Task<ActionResult<Report>> GetOpportunitiesSharedReportsByUserId(int userId)
        {
            try
            {
                var report = await _reportService.GetOpportunitiesSharedReportByUserId(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetContactsAccountsSharedReportsByUserId")]
        public async Task<ActionResult<ReportConfigurationModel>> GetContactsAccountsSharedReportsByUserId(int userId)
        {
            try
            {
                var report = await _reportService.GetContactsAccountsSharedReportByUserId(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("DeleteReport")]
        public async Task<ActionResult<bool>> DeleteReport(ReportConfigurationModel model)
        {

            try
            {
                await _reportService.DeleteReport(model.ReportId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }

        [HttpGet("GetReportCategoryIdByReportType")]
        public async Task<ActionResult<int>> GetReportCategoryIdByReportType(string reportType)
        {
            var reportCategoryId = await _reportService.GetReportCategoryIdByReportType(reportType);
            int Id = reportCategoryId.ReportCategoryId;
            return Ok(Id);
        }

        [HttpGet("GetCustomFieldIdByReportFieldId")]
        public async Task<ActionResult<int>> GetCustomFieldIdByReportFieldId(int id)
        {
            var customFieldId = await _reportService.GetCustomFieldIdByReportFieldId(id);
            int? Id = customFieldId.CustomFieldId;
            return Ok(Id);
        }

    }
}
