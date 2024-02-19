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
    public class RelationController : ControllerBase
    {

        private readonly ILogger<RelationController> _logger;
        private readonly IRelationService _relationService;

        public RelationController(ILogger<RelationController> logger, IRelationService RelationService)
        {
            _logger = logger;
            _relationService = RelationService;
        }

        [HttpGet("GetAllRelations")]
        public async Task<ActionResult<IEnumerable<Relation>>> GetAllRelations()
        {
            var Relations = await _relationService.GetAllRelations();
            return Ok(Relations);
        }

        [HttpGet("GetRelationsByEntityId")]
        public async Task<ActionResult<IEnumerable<Relation>>> GetRelationsByEntityId(int entityId)
        {
            bool response = false;
            
            if (entityId <= 0) return BadRequest(new { message = "Invalid Entity Id supplied." });
       
            try
            {
                var relations = await _relationService.GetRelationsByEntityId(entityId);
                return Ok(relations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CreateRelation")]
        public async Task<ActionResult<Relation>> CreateRelation([FromBody] RelationModel model)
        {
            Relation response = new Relation();

            try
            {
                response = await _relationService.CreateRelation(model);
                if (response.RelationId == 0)
                {
                    return BadRequest(new { message = "Failed to create Relation" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateRelation")]
        public async Task<ActionResult<Relation>> UpdateRelation([FromBody] RelationModel model)
        {
            Relation response = new Relation();

            try
            {
                response = await _relationService.UpdateRelation(model);
                if (response.RelationId == 0)
                {
                    _logger.LogError("$Could not find reccord with RelationId={model.RelationId}");
                    return BadRequest(new { message = "Failed to create Relation" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteRelation")]
        public async Task<ActionResult<bool>> DeleteRelation([FromBody] RelationModel model)
        {

            try
            {
                await _relationService.DeleteRelation(model.RelationId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }

        [HttpGet("GetRelationSelectListByEntityId")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetRelationSelectListByEntityId(int entityId)
        {

            var relations = await _relationService.GetRelationSelectListByEntityId(entityId);
            return Ok(relations);
        }

        [HttpPost("AddOrUpdateRelation")]
        public async Task<ActionResult<bool>> AddOrUpdateRelation([FromBody] List<RelationModel> model)
        {
            try
            {
                var relations = await _relationService.AddOrUpdateRelation(model);
                return Ok(relations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
