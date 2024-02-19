using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactActivityController : ControllerBase
    {
        private readonly ILogger<ContactActivityController> _logger;
        private readonly IContactActivityService _contactActivityService;

        public ContactActivityController(ILogger<ContactActivityController> logger, IContactActivityService contactActivityService)
        {
            _logger = logger;
            _contactActivityService = contactActivityService;
        }

        //TODO:AKS add more descriptive logging 
        [HttpPost("CreateContactActivity")]
        public async Task<ActionResult<Contactactivity>> CreateContactActivity(ContactActivityInputModel model)
        {
            Contactactivity activity = new Contactactivity();

            try
            {
                activity = await _contactActivityService.CreateContactActivity(model);
                if (activity.ContactActivityId == 0)
                {
                    return BadRequest(new { message = "Failed to create contact activity" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(activity);

        }
        //TODO:AKS add more descriptive logging 
        [HttpPost("UpdateContactActivity")]
        public async Task<ActionResult<Contactactivity>> UpdateContactActivity(ContactActivityInputModel model)
        {
            bool success = false;

            try
            {
                success = await _contactActivityService.UpdateContactActivity(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }
        //TODO:AKS add more descriptive logging 
        [HttpPost("DeleteContactActivity")]
        public async Task<ActionResult<bool>> DeleteContactActivity(ContactActivityModel model)
        {
            bool response = false;
            try
            {
                response = await _contactActivityService.DeleteContactActivity(model.ContactActivityId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        //TODO:AKS add more descriptive logging 
        [HttpGet("GetContactActivitiesByEntityId")]
        public async Task<ActionResult<List<ContactActivityOutputModel>>> GetContactActivitiesByEntityId(int id, int? recordsCount = 0)
        {
            try
            {
                var activities = await _contactActivityService.GetContactActivityByEntityId(id, recordsCount);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetRoleActivitiesByEntityId")]
        public async Task<ActionResult<List<ContactActivityModel>>> GetRoleActivitiesByEntityId(int id, int roleId)
        {
            var activities = await _contactActivityService.GetRoleActivityByEntityId(id, roleId);
            return Ok(activities);
        }

        [HttpGet("GetAllContactActivities")]
        public async Task<ActionResult<List<ContactActivityModel>>> GetAllContactActivities(int id)
        {
            var activities = await _contactActivityService.GetAllContactActivities();
            return Ok(activities);
        }
        [HttpGet("GetContactActivityById")]
        public async Task<ActionResult<ContactActivityModel>> GetContactActivityById(int activityId)
        {
            var activity = await _contactActivityService.GetContactActivityById(activityId);
            return Ok(activity);
        }

        [HttpGet("GetContactActivityBySearchCondition")]
        public async Task<ActionResult<List<ContactActivityModel>>> GetContactActivityBySearchCondition(int entityId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId)
        {
            var activities = await _contactActivityService.GetContactActivityBySearchCondition(entityId, fromDate, toDate, interactionType, interactionEntityId);
            return Ok(activities);
        }

        [HttpGet("GetRoleActivityBySearchCondition")]
        public async Task<ActionResult<List<ContactActivityModel>>> GetRoleActivityBySearchCondition(int entityId,
           DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId, int roleId)
        {
            var activities = await _contactActivityService.GetRoleActivityBySearchCondition(entityId, fromDate, toDate, interactionType, interactionEntityId, roleId);
            return Ok(activities);
        }

        [HttpGet("GetContactActivityByDate")]
        public async Task<ActionResult<List<ContactActivityModel>>> GetContactActivityByActivityDate(int entityId, DateTime activityDate)
        {
            var activities = await _contactActivityService.GetContactActivityByActivityDate(entityId, activityDate);
            return Ok(activities);
        }
        [HttpGet("GetContactActivityByRoleIdAndActivityDateAsync")]
        public async Task<ActionResult<List<ContactActivityModel>>> GetContactActivityByRoleIdAndActivityDateAsync(int entityId, int roleId, DateTime activityDate)
        {
            var activities = await _contactActivityService.GetContactActivityByRoleIdAndActivityDateAsync(entityId, roleId, activityDate);
            return Ok(activities);
        }
    }
}
