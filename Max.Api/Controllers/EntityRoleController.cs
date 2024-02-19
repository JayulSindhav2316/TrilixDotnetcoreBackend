using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EntityRoleController : ControllerBase
    {
        private readonly ILogger<EntityRoleController> _logger;
        private readonly IEntityRoleService _entityRoleService;

        public EntityRoleController(ILogger<EntityRoleController> logger, IEntityRoleService entityRoleService)
        {
            _logger = logger;
            _entityRoleService = entityRoleService;
        }

        //TODO:AKS add more descriptive logging 
        [HttpPost("CreateEntityRole")]
        public async Task<ActionResult<Entityrole>> CreateEntityRole(EntityRoleModel model)
        {
            Entityrole role = new Entityrole();

            try
            {
                role = await _entityRoleService.CreateEntityRole(model);
                if (role.EntityRoleId == 0)
                {
                    return BadRequest(new { message = "Failed to add contact Role" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(role);

        }
        //TODO:AKS add more descriptive logging 
        [HttpPost("UpdateEntityRole")]
        public async Task<ActionResult<Contactrole>> UpdateEntityRole(EntityRoleModel model)
        {
            bool success = false;

            try
            {
                success = await _entityRoleService.UpdateEntityRole(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }

        //TODO:AKS add more descriptive logging 
        [HttpGet("GetAllEntityRoles")]
        public async Task<ActionResult<List<Entityrole>>> GetAllEntityRoles()
        {
            var entityRoles = await _entityRoleService.GetAllEntityRoles();
            return Ok(entityRoles);
        }

        //TODO:AKS add more descriptive logging 
        [HttpGet("GetActiveEntityRolesByEntityId")]
        public async Task<ActionResult<List<Entityrole>>> GetActiveEntityRolesByEntityId(int entityId)
        {
            var entityRoles = await _entityRoleService.GetActiveEntityRolesByEntityId(entityId);
            return Ok(entityRoles);
        }

        [HttpGet("GetActiveEntityRoleListByEntityId")]
        public async Task<ActionResult<List<SelectListModel>>> GetActiveEntityRoleListByEntityId(int entityId)
        {
            var entityRoles = await _entityRoleService.GetActiveEntityRoleListByEntityId(entityId);
            return Ok(entityRoles);
        }
        [HttpGet("GetActiveEntityAccountRolesByEntityId")]
        public async Task<ActionResult<List<Entityrole>>> GetActiveEntityAccountRolesByEntityId(int entityId)
        {
            //var entity = await _personService
            var entityRoles = await _entityRoleService.GetActiveEntityRolesByEntityId(entityId);
            return Ok(entityRoles);
        }
        [HttpGet("GetActiveEntityRolesByCompanyId")]
        public async Task<ActionResult<List<AccountContactRoleModel>>> GetActiveEntityRolesByCompanyId(int companyId)
        {
            var entityRoles = await _entityRoleService.GetActiveEntityRolesByCompanyId(companyId);
            return Ok(entityRoles);
        }

        [HttpGet("GetActiveEntityRoleListByCompanyId")]
        public async Task<ActionResult<List<SelectListModel>>> GetActiveEntityRoleListByCompanyId(int companyId)
        {
            var entityRoles = await _entityRoleService.GetActiveEntityRoleListByCompanyId(companyId);
            return Ok(entityRoles);
        }

        [HttpGet("GetActiveAccountContactsByEntityId")]
        public async Task<ActionResult<List<AccountContactRoleModel>>> GetActiveAccountContactsByEntityId(int entityId)
        {
            var entityRoles = await _entityRoleService.GetActiveAccountContactsByEntityId(entityId);
            return Ok(entityRoles);
        }
        [HttpGet("GetAccountContactsByEntityId")]
        public async Task<ActionResult<List<AccountContactRoleModel>>> GetAccountContactsByEntityId(int entityId)
        {
            var entityRoles = await _entityRoleService.GetAccountContactsByEntityId(entityId);
            return Ok(entityRoles);
        }
        [HttpGet("GetAllEntityRolesByEntityId")]
        public async Task<ActionResult<List<Entityrole>>> GetAllEntityRolesByEntityId(int entityId)
        {
            var entityRoles = await _entityRoleService.GetAllEntityRolesByEntityId(entityId);
            return Ok(entityRoles);
        }
        [HttpGet("GetEntityRolesByCompanyId")]
        public async Task<ActionResult<List<ContactRoleModel>>> GetEntityRolesByCompanyId(int companyId)
        {
            var entityRoles = await _entityRoleService.GetEntityRolesByCompanyId(companyId);
            return Ok(entityRoles);
        }
        [HttpGet("GetContactsByFirstAndLastName")]
        public async Task<ActionResult<List<AccountContactModel>>> GetContactsByFirstAndLastName(string firstName, string lastName, int companyId)
        {
            var entityRoles = await _entityRoleService.GetContactsByFirstAndLastName(firstName, lastName, companyId);
            return Ok(entityRoles);
        }
        [HttpGet("GetContactsByName")]
        public async Task<ActionResult<List<AccountContactModel>>> GetContactsByName(string name, int companyId)
        {
            var entityRoles = await _entityRoleService.GetContactsByName(name, companyId);
            return Ok(entityRoles);
        }
        [HttpGet("GetContactsByComapnyId")]
        public async Task<ActionResult<List<AccountContactRoleModel>>> GetContactsByComapnyId(int companyId)
        {
            var entityRoles = await _entityRoleService.GetContactsByCompanyId(companyId);
            return Ok(entityRoles);
        }
        [HttpGet("GetContactsByRoleAndComapnyId")]
        public async Task<ActionResult<List<AccountContactModel>>> GetContactsByRoleAndComapnyId(int roleId, int companyId)
        {
            var entityRoles = await _entityRoleService.GetContactsByRoleAndCompanyId(roleId, companyId);
            return Ok(entityRoles);
        }
        [HttpGet("GetContactsByRoleAndEntityId")]
        public async Task<ActionResult<List<AccountContactModel>>> GetContactsByRoleAndEntityId(int roleId, int entityId)
        {
            var entityRoles = await _entityRoleService.GetContactsByRoleAndEntityId(roleId, entityId);
            return Ok(entityRoles);
        }
        [HttpPost("UnassignEntityRole")]
        public async Task<ActionResult<bool>> UnassignEntityRole(EntityRoleModel model)
        {
            bool response = false;
            try
            {
                response = await _entityRoleService.UnassignEntityRole(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("DeleteAssignment")]
        public async Task<ActionResult<bool>> DeleteAssignment(EntityRoleModel model)
        {
            bool response = false;
            try
            {
                response = await _entityRoleService.DeleteAssignment(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetContactRoleHistoryEntityId")]
        public async Task<ActionResult<List<AccountContactRoleHistoryModel>>> GetContactRoleHistoryEntityId(int entityId)
        {
            var entityRolesHistory = await _entityRoleService.GetContactRoleHistoryByEntityId(entityId);
            return Ok(entityRolesHistory);
        } 
        [HttpGet("GetActiveContactRolesByEntityId")]
        public async Task<ActionResult<List<AccountContactRoleHistoryModel>>> GetActiveContactRolesByEntityId(int entityId)
        {
            var entityRolesHistory = await _entityRoleService.GetActiveContactRolesByEntityId(entityId);
            return Ok(entityRolesHistory);
        }
        [HttpPost("UpdateEntityRoleEffectiveDates")]
        public async Task<ActionResult<bool>> UpdateEntityRoleEffectiveDates(EntityRoleModel entityRoleModel)
        {
            var entityRoles = await _entityRoleService.UpdateEntityRoleEffectiveDates(entityRoleModel);
            return Ok(entityRoles);
        }

        [HttpPost("AccountChangeRoleChangeOperation")]
        public async Task<ActionResult<bool>> AccountChangeRoleChangeOperation(List<EntityRoleModel> entityRoleModels)
        {
            bool response = false;
            try
            {
                response = await _entityRoleService.AccountChangeRoleChangeOperation(entityRoleModels);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

           
            return Ok(response);
        }
        [HttpGet("GetContactSelectList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetContactSelectList(int accountId)
        {
            try
            {
                var selectList = await _entityRoleService.GetContactSelectList(accountId);
                return Ok(selectList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
