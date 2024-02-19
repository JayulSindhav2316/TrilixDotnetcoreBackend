using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Max.Api.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class AuditHistoryController: ControllerBase
    {
    
        private readonly IAuditHistoryService _auditHistoryService;
        public AuditHistoryController(IAuditHistoryService auditHistoryService) 
        {       
            _auditHistoryService = auditHistoryService;
        }

        [HttpGet("GetAuditHisotryById")]
        public async Task<ActionResult<List<AuditChangesModel>>> GetAuditHisotryById(string primaryKey)
        {
            var auditHistory = await _auditHistoryService.GetAuditHistoryById(primaryKey);
            return Ok(auditHistory);
        }
    }
}
