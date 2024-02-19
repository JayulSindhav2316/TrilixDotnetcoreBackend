using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SociableController : ControllerBase
    {
        private readonly ILogger<SociableController> _logger;
        private readonly IEntityService _entityService;
        private readonly IPersonService _personService;

        public SociableController(ILogger<SociableController> logger, IEntityService entityService, IPersonService personService)
        {
            _logger = logger;
            _entityService = entityService;
            _personService = personService;
        }

        [HttpGet("GetEntityDetails")]
        public async Task<ActionResult<EntitySociableModel>> GetEntityDetailsById(int entityId)
        {
            if (entityId < 0) return BadRequest(new { message = "Invalid Entity Id." });
            var entity = new EntitySociableModel();
            try
            {
                entity = await _entityService.GetSociableEntityDetailsById(entityId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(entity);
        }

        [HttpPost("UpdateEntityDetails")]
        public async Task<ActionResult<bool>> UpdateEntityDetails(EntitySociableModel entitySociableModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update entity" });
            }
            bool response = false;
            if (entitySociableModel.EntityId <= 0) return BadRequest(new { message = "Invalid EntityId supplied." });
            try
            {
                response = await _personService.UpdateSociablePerson(entitySociableModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update entity" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
    }
}
