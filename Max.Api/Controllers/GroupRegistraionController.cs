using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core.Helpers;

namespace Max.Api.Controllers
{
    [ApiController]
    
    [Route("api/[controller]")]
    public class GroupRegistraionController : Controller
    {
        private readonly IGroupRegistrationService _GroupRegistrationService;
        public GroupRegistraionController(IGroupRegistrationService groupRegistrationService, ILogger<GroupRegistraionController> logger)
        {
            _GroupRegistrationService = groupRegistrationService;
        }
        [HttpPost("RegisterGroup")]
        public async Task<ActionResult> RegisterGroup(RegistrationGroupModel data)
        {
            var res= await _GroupRegistrationService.RegisterGroup(data);
            return Ok();
        }
        [HttpGet("GetAllRegisterGroups")]
        public async Task<ActionResult> GetRegisterGroup(string searchText)
        {
            var res = await _GroupRegistrationService.GetRegisterGroups(searchText);
            return Ok(res);
        }
        [HttpDelete("DeleteGroup")]
        public async Task<ActionResult> DeleteGroup(int groupId)
        {
            var res = await _GroupRegistrationService.DeleteGroup(groupId);
            return Ok(res);
        }
        [HttpPost("UpdateGroup")]
        public async Task<ActionResult> UpdateGroup(RegistrationGroupModel data)
        {
            var res = await _GroupRegistrationService.UpdateGroup(data);
            return Ok();
        }

        [HttpPost("LinkMembership")]
        public async Task<ActionResult> LinkMembership(RegistrationGroupModel data)
        {
            var res = await _GroupRegistrationService.LinkMembership(data);
            return Ok(res);
        }
        [HttpDelete("DeleteLink")]
        public async Task<ActionResult> DeleteLink(int linkId)
        {
            var res = await _GroupRegistrationService.DeleteLink(linkId);
            return Ok(res);
        }

        [HttpGet("GetLinkEventModelForAllRegisterGroups")]
        public async Task<ActionResult> GetLinkEventModelForAllRegisterGroups(int eventId)
        {
            var linkEventGroupModelList = await _GroupRegistrationService.GetLinkEventModelForAllRegisterGroups(eventId);
            return Ok(linkEventGroupModelList);
        }
    }
}
