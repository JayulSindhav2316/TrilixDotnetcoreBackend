using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System.Linq;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {

        private readonly ILogger<EntityController> _logger;
        private readonly IEntityService _entityService;

        public EntityController(ILogger<EntityController> logger, IEntityService entityService)
        {
            _logger = logger;
            _entityService = entityService;
        }

        [HttpGet("GetEntityProfileById")]
        public async Task<ActionResult<EntityModel>> GetEntityProfileById(int entityId)
        {
            if (entityId < 0) return BadRequest(new { message = "Invalid Entity Id." });
            var entity = new EntityModel();
            try
            {
                entity = await _entityService.GetEntityProfileById(entityId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(entity);
        }
        [HttpGet("GetEntityById")]
        public async Task<ActionResult<EntityModel>> GetEntityById(int entityId)
        {
            if (entityId < 0) return BadRequest(new { message = "Invalid Entity Id." });
            var entity = new EntityModel();
            try
            {
                entity = await _entityService.GetEntityById(entityId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(entity);
        }

        [HttpGet("GetEntitySummaryById")]
        public async Task<ActionResult<EntitySummaryModel>> GetEntitySummaryById(int entityId)
        {
            if (entityId < 0) return BadRequest(new { message = "Invalid Entity Id." });
            var entity = new EntitySummaryModel();
            try
            {
                entity = await _entityService.GetEntitySummaryById(entityId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(entity);
        }

        [HttpGet("GetEntitiesByName")]
        public async Task<ActionResult<List<Entity>>> GetEntitiesByName(string name)
        {
           
            try
            {
                var entities = await _entityService.GetEntitiesByName(name);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            
        }

        [HttpGet("GetMembershipProfileByEntityId")]
        public async Task<ActionResult<IEnumerable<EntityMembershipProfileModel>>> GetMembershipProfileByEntityId(int entityId)
        {
            var history = await _entityService.GetMembershipProfileById(entityId);
            return Ok(history);
        }

        [HttpGet("GetMembershipHistory")]
        public async Task<ActionResult<IEnumerable<EntityMembershipHistoryModel>>> GetMembershipHistory(int entityId)
        {
            var history = await _entityService.GetMembershipHistoryByEntityId(entityId);
            return Ok(history);
        }

        [HttpGet("GetScheduledBillingByEntityId")]
        public async Task<ActionResult<List<EntityBillingModel>>> GetScheduledBillingByEntityId(int entityId)
        {
            var history = await _entityService.GetScheduledBillingByEntityId(entityId);
            return Ok(history);
        }

        [HttpGet("getCreditBalanceById")]
        public async Task<ActionResult<List<EntityBillingModel>>> getCreditBalanceById(int entityId)
        {
            var history = await _entityService.GetCreditBalanceById(entityId);
            return Ok(history);
        }

        [HttpGet("GetEntitiesByIds")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetEntitiesByIds(string entityIds)
        {
            if (String.IsNullOrEmpty(entityIds)) return BadRequest(new { message = "Invalid Entity Id." });

            var persons = await _entityService.GetEntitiesByEntityIds(entityIds);
            return Ok(persons);
        }
        [HttpGet("GetBillingAddressByEntityId")]
        public async Task<ActionResult<BillingAddressModel>> GetBillingAddressByEntityId(int entityId)
        {
            var addressModel = await _entityService.GetBillingAddressByEntityId(entityId);
            return Ok(addressModel);
        }

        [HttpGet("AddBillableContact")]
        public async Task<ActionResult<Boolean>> AddBillableContact(int entityId, int billableContactId)
        {
            var result = await _entityService.AddBillableContact(entityId, billableContactId);
            return Ok(result);
        }

        [HttpPost("UpdateWebLogin")]
        public async Task<ActionResult<Person>> UpdateWebLogin(WebLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update Person" });
            }
            bool response = false;
            if (model.EntityId <= 0) return BadRequest(new { message = "Invalid Id supplied." });
            try
            {
                response = await _entityService.UpdateWebLoginPasword(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Person's Web Login & Password" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateBillingNotification")]
        public async Task<ActionResult<Person>> UpdateBillingNotification(BillingNotificationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update Billing Notification" });
            }
            bool response = false;
            if (model.EntityId <= 0) return BadRequest(new { message = "Invalid Id supplied." });
            try
            {
                response = await _entityService.UpdateBillingNotification(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Person's Billing Notification" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
    }
}
