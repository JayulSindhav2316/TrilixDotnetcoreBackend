using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Microsoft.Extensions.Hosting;
using Max.Services;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OpportunityController : ControllerBase
    {
        private readonly ILogger<OpportunityController> _logger;
        private readonly IOpportunityService _opportunityService;

        public OpportunityController(ILogger<OpportunityController> logger, IOpportunityService opprtunityService)
        {
            _logger = logger;
            _opportunityService = opprtunityService;
        }

        [HttpGet("GetAllPipelines")]
        public async Task<ActionResult<List<OpportunityPipelineModel>>> GetAllPipelines()
        {
            var pipelines = await _opportunityService.GetAllOpportunityPipelines();
            return Ok(pipelines);
        }
        [HttpGet("GetAllOpportunities")]
        public async Task<ActionResult<List<OpportunityModel>>> GetAllOpportunities()
        {
            var opportunities = await _opportunityService.GetAllOpportunites();
            return Ok(opportunities);
        }

        [HttpGet("GetPipelineById")]
        public async Task<ActionResult<OpportunityPipelineModel>> GetPipelineById(int id)
        {
            var pipeline = await _opportunityService.GetOpportunityPipelineById(id);
            return Ok(pipeline);
        }

        [HttpGet("GetOpportunitesByAccountContactId")]
        public async Task<ActionResult<List<OpportunityModel>>> GetOpportunitesByAccountContactId(int id)
        {
            var opportunities = await _opportunityService.GetAllOpportunitesByAccountContactId(id);
            return Ok(opportunities);
        }

        [HttpGet("GetOpportunitesByPipelineId")]
        public async Task<ActionResult<List<OpportunityModel>>> GetOpportunitesByPipelineId(int id)
        {
            var opportunities = await _opportunityService.GetAllOpportunitesByPipelineId(id);
            return Ok(opportunities);
        }
        [HttpGet("GetOpportunitesByPipelineIdGroupedByStages")]
        public async Task<ActionResult<List<OpportunityModel>>> GetOpportunitesByPipelineIdGroupedByStages(int id)
        {
            var opportunities = await _opportunityService.GetAllOpportunitesByPipelineIdGroupedByStages(id);
            return Ok(opportunities);
        }
        [HttpGet("GetOpportunitesByCompanyId")]
        public async Task<ActionResult<List<OpportunityModel>>> GetOpportunitesByCompanyId(int id)
        {
            var opportunities = await _opportunityService.GetAllOpportunitesByCompanyId(id);
            return Ok(opportunities);
        }

        [HttpGet("GetOpportunitiesBySearchText")]
        public async Task<ActionResult<List<OpportunityStageGroupModel>>> GetOpportunitiesBySearchText(int pipelineId,string searchText)
        {
            var opportunities = await _opportunityService.GetOpportunitiesBySearchText(pipelineId, searchText);
            return Ok(opportunities);
        }

        [HttpGet("GetOpportunityById")]
        public async Task<ActionResult<OpportunityModel>> GetOpportunityById(int id)
        {
            var opprtunity = await _opportunityService.GetOpportunityById(id);
            return Ok(opprtunity);
        }

        [HttpGet("GetAllActivePipelines")]
        public async Task<ActionResult<List<OpportunityPipelineModel>>> GetAllActivePipelines()
        {
            var pipelines = await _opportunityService.GetAllActiveOpportunityPipelines();
            return Ok(pipelines);
        }

        [HttpPost("CreatePipeline")]
        public async Task<ActionResult<OpportunityPipelineModel>> CreatePipeline(OpportunityPipelineModel model)
        {
            OpportunityPipelineModel pipeline = new OpportunityPipelineModel();
            try
            {
                pipeline = await _opportunityService.CreateOpportunityPipeline(model);
                if (pipeline.PipelineId == 0)
                {
                    return BadRequest(new { message = "Failed to create Pipeline" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(pipeline);
        }

        [HttpPost("CreateOpportunity")]
        public async Task<ActionResult<OpportunityModel>> CreateOpportunity(OpportunityModel model)
        {
            OpportunityModel opportunity = new OpportunityModel();
            if (model == null)
            {
                return BadRequest(new { message = "No data found" });
            }
            try
            {
                opportunity = await _opportunityService.CreateOpportunity(model);
                if (opportunity.OpportunityId == 0)
                {
                    return BadRequest(new { message = "Failed to create Opportunity" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(opportunity);
        }

        [HttpPost("UpdatePipeline")]
        public async Task<ActionResult<OpportunityPipelineModel>> UpdatePipeline(OpportunityPipelineModel model)
        {
            var success = false;
            try
            {
                success = await _opportunityService.UpdateOpportunityPipeline(model);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update Pipeline" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(success);
        }

        [HttpPost("UpdateOpportunity")]
        public async Task<ActionResult<OpportunityModel>> UpdateOpportunity(OpportunityModel model)
        {
            var success = false;
            try
            {
                success = await _opportunityService.UpdateOpportunity(model);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update Opportunity" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(success);
        }
        [HttpPost("MoveOpportunityToAnotherStage")]
        public async Task<ActionResult<List<OpportunityPipelineModel>>> MoveOpportunityToAnotherStage(OpportunityModel model)
        {
            try
            {
                var pipielineGroupedByStage = await _opportunityService.MoveOpportunityToAnotherStage(model);
                return Ok(pipielineGroupedByStage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("DeletePipeline")]
        public async Task<ActionResult<Opportunitypipeline>> DeletePipeline([FromBody] OpportunityPipelineModel model)
        {

            try
            {
                var result = await _opportunityService.DeleteOpportunityPipeline(model.PipelineId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }


        }
        [HttpPost("ClonePipeline")]
        public async Task<ActionResult<OpportunityPipelineModel>> ClonePipeline([FromBody] OpportunityPipelineModel model)
        {
            OpportunityPipelineModel clonedPipeline = new OpportunityPipelineModel();
            try
            {
                clonedPipeline = await _opportunityService.CloneOpportunityPipeline(model);

            }
            catch(InvalidOperationException ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = "Failed to clone Pipeline" });
            }

            return Ok(clonedPipeline);
        }

        [HttpGet("GetPipelineSelectList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetPipelineSelectList()
        {
            try
            {
                var selectList = await _opportunityService.GetPipelineSelectList();
                return Ok(selectList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("GetProductSelectList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetProductSelectList(int pipelineId)
        {
            try
            {
                var selectList = await _opportunityService.GetProductSelectList(pipelineId);
                return Ok(selectList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("GetStageSelectList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetStageSelectList(int pipelineId)
        {
            try
            {
                var selectList = await _opportunityService.GetStageSelectList(pipelineId);
                return Ok(selectList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("GetOpportunitiesByStageId")]
        public async Task<ActionResult<IEnumerable<OpportunityModel>>> GetOpportunitiesByStageId(int stageId)
        {
            try
            {
                var data = await _opportunityService.GetOpportunitesByStageId(stageId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("GetOpportunitiesByProductId")]
        public async Task<ActionResult<IEnumerable<OpportunityModel>>> GetOpportunitiesByProductId(int productId)
        {
            try
            {
                var data = await _opportunityService.GetOpportunitesByProductId(productId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("GetStagesByPipelineId")]
        public async Task<ActionResult<List<PipelineStageModel>>> GetStagesByPipelineId(int pipelineId)
        {
            try
            {
                var pipelineStages = await _opportunityService.GetStagesByPipelineId(pipelineId);
                return Ok(pipelineStages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}