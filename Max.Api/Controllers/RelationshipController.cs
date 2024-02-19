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
    public class RelationshipController : ControllerBase
    {

        private readonly ILogger<RelationController> _logger;
        private readonly IRelationshipService _relationshipService;

        public RelationshipController(ILogger<RelationController> logger, IRelationshipService relationshipService)
        {
            _logger = logger;
            _relationshipService = relationshipService;
        }

        [HttpGet("GetRelationshipSelectList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetRelationshipSelectList()
        {
            var selectList = await _relationshipService.GetSelectList();
            return Ok(selectList);
        }
    }
}
