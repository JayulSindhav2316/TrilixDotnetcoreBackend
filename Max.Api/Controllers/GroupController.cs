using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Api.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;
        private readonly IGroupService _groupService;
        private readonly IGroupRoleService _groupRoleService;
        private readonly ILinkGroupRoleService _linkGroupRoleService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IGroupMemberRoleService _groupMemberRoleService;

        public GroupController(ILogger<GroupController> logger, IGroupService groupService, IGroupRoleService groupRoleService, ILinkGroupRoleService linkGroupRoleService, IGroupMemberService groupMemberService, IGroupMemberRoleService groupMemberRoleService)
        {
            _logger = logger;
            _groupService = groupService;
            _groupRoleService = groupRoleService;
            _linkGroupRoleService = linkGroupRoleService;
            _groupMemberService = groupMemberService;
            _groupMemberRoleService = groupMemberRoleService;
        }

        [HttpPost("CreateGroup")]
        public async Task<ActionResult<Group>> CreateGroup(GroupModel groupModel)
        {
            Group group = new Group();

            try
            {
                if (groupModel != null)
                {
                    group = await _groupService.CreateGroup(groupModel);
                    if (group.GroupId == 0)
                    {
                        return BadRequest(new { message = "Failed to create Board / Group" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Request model getting null" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(group);

        }

        [HttpPost("UpdateGroup")]
        public async Task<ActionResult<Group>> UpdateGroup(GroupModel groupModel)
        {
            bool success = false;

            try
            {
                var str = JsonConvert.SerializeObject(groupModel);
                success = await _groupService.UpdateGroup(groupModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }

        [HttpPost("UpdateSocialGroup")]
        public async Task<ActionResult<Group>> UpdateSocialGroup([FromHeader] string data)
        {
            bool success = false;

            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var groupModel = JsonConvert.DeserializeObject<UpdateSociableGroupRequestModel>(data);
                    success = await _groupService.UpdateSocialGroup(groupModel);
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }

        [HttpDelete("DeleteGroup/{groupId}")]
        public async Task<ActionResult<bool>> DeleteGroup(int groupId)
        {
            bool response = false;
            try
            {
                response = await _groupService.DeleteGroup(groupId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("AddGroupRole")]
        public async Task<ActionResult<Grouprole>> AddGroupRole(GroupRoleModel groupRoleModel)
        {
            Grouprole grouprole = new Grouprole();

            try
            {
                grouprole = await _groupRoleService.AddGroupRole(groupRoleModel);
                if (grouprole.GroupRoleId == 0)
                {
                    return BadRequest(new { message = "Failed to create Position" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(grouprole);

        }

        [HttpPost("UpdateGroupRole")]
        public async Task<ActionResult<bool>> UpdateGroupRole(GroupRoleModel groupRoleModel)
        {
            bool success = false;
            try
            {
                success = await _groupRoleService.UpdateGroupRole(groupRoleModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }


        [HttpGet("GetAllGroupDetailsByOrganizationId")]
        public async Task<ActionResult<List<GroupModel>>> GetAllGroupDetailsByOrganizationId(int organizationId)
        {
            var groups = await _groupService.GetAllGroupDetailsByOrganizationId(organizationId);
            return Ok(groups);
        }

        [HttpGet("GetAllGroupsByOrganizationId")]
        public async Task<ActionResult<List<GroupModel>>> GetAllGroupsByOrganizationId(int organizationId)
        {
            var groups = await _groupService.GetAllGroupsByOrganizationId(organizationId);
            return Ok(groups);
        }

        [HttpGet("GetGroupsByOrganizationId")]
        public async Task<ActionResult<List<GroupModel>>> GetGroupsByOrganizationId(int organizationId)
        {
            var groups = await _groupService.GetGroupsByOrganizationId(organizationId);
            return Ok(groups);
        }

        [HttpGet("GetGroupById")]
        public async Task<ActionResult<GroupModel>> GetGroupById(int groupId)
        {
            var groups = await _groupService. GetGroupByGroupId(groupId);
            return Ok(groups);
        }

        [HttpGet("GetDefaultGroupRoles")]
        public async Task<ActionResult<List<LinkGroupRoleModel>>> GetDefaultGroupRoles(int organizationId)
        {
            var groupRoles = await _groupRoleService.GetDefaultGroupRoles(organizationId);
            return Ok(groupRoles);
        }

        [HttpGet("GetLinkGroupRoleByOrganizationId")]
        public async Task<ActionResult<List<LinkGroupRoleModel>>> GetLinkGroupRoleByOrganizationId(int organizationId)
        {
            var groupRolesList = await _linkGroupRoleService.GetLinkGroupRoleByOrganizationId(organizationId);
            return Ok(groupRolesList);
        }

        [HttpPost("CreateLinkGroupRolesOnOrganizationSetUp")]
        public async Task<ActionResult<bool>> CreateLinkGroupForDefaultRolesOnOrganizationSetUp(int organizationId)
        {
            var success = await _linkGroupRoleService.CreateLinkGroupForDefaultRolesOnOrganizationSetUp(organizationId);
            return Ok(success);
        }

        [HttpGet("GetLinkedRolesByGroupId")]
        public async Task<ActionResult<List<LinkGroupRoleModel>>> GetLinkedRolesByGroupId(int groupId, int organizationId)
        {
            var linkedGrouproles = await _linkGroupRoleService.GetLinkedRolesByGroupId(groupId, organizationId);
            return Ok(linkedGrouproles);
        }

        [HttpPost("AddLinkGroupRole")]
        public async Task<ActionResult<bool>> AddLinkGroupRole(LinkGroupRoleModel linkGroupRoleModel)
        {
            Linkgrouprole linkgrouprole = new Linkgrouprole();
            try
            {
                linkgrouprole = await _linkGroupRoleService.AddLinkGroupRole(linkGroupRoleModel);
                if (linkgrouprole.LinkGroupRoleId == 0)
                {
                    return BadRequest(new { message = "Failed to create Link Group Position record" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(linkgrouprole);

        }

        [HttpPost("UpdateLinkGroupRole")]
        public async Task<ActionResult<bool>> UpdateLinkGroupRole(LinkGroupRoleModel linkGroupRoleModel)
        {
            bool success = false;
            try
            {
                success = await _linkGroupRoleService.UpdateLinkGroupRole(linkGroupRoleModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }

        [HttpGet("GetRolesSelectListByGroupId")]
        public async Task<ActionResult<List<LinkGroupRoleModel>>> GetRolesSelectListByGroupId(int groupId, int status = 1)
        {
            var linkedGrouproles = await _linkGroupRoleService.GetLinkGroupRoleByGroupId(groupId, status);
            return Ok(linkedGrouproles);
        }

        [HttpGet("GetAllGroupMembersByGroupId")]
        public async Task<ActionResult<List<GroupMemberViewModel>>> GetAllGroupMembersByGroupId(int groupId)
        {
            var groupMembers = await _groupMemberService.GetAllGroupMembersByGroupId(groupId);
            return Ok(groupMembers);
        }

        [HttpGet("GetAllGroupsByEntityId")]
        public async Task<ActionResult<List<GroupMemberModel>>> GetAllGroupsByEntityId(int entityId)
        {
            var groups = await _groupMemberService.GetAllGroupsByEntityId(entityId);
            return Ok(groups);
        }

        [HttpGet("GetGroupsForGroupMemberByEntityId")]
        public async Task<ActionResult<List<GroupMemberModel>>> GetGroupsForGroupMemberByEntityId(int entityId)
        {
            var groups = await _groupService.GetGroupsForGroupMemberByEntityId(entityId);
            return Ok(groups);
        }

        [HttpGet("GetGroupsByEntityId")]
        public async Task<ActionResult<List<GroupMemberModel>>> GetGroupsByEntityId(int entityId)
        {
            var groups = await _groupService.GetGroupsByEntityId(entityId);
            return Ok(groups);
        }

        [HttpGet("GetRolesByGroupMemberId")]
        public async Task<ActionResult<List<GroupMemberModel>>> GetRolesByGroupMemberId(int groupMemberId)
        {
            var groups = await _groupMemberRoleService.GetRolesByGroupMemberId(groupMemberId);
            return Ok(groups);
        }

        [HttpPost("AddGroupMember")]
        public async Task<ActionResult<Groupmember>> AddGroupMember(GroupMemberModel groupmemberModel)
        {
            Groupmember groupmember;
            try
            {
                groupmember = await _groupMemberService.AddGroupMember(groupmemberModel);
                if (groupmember.GroupMemberId == 0)
                {
                    return BadRequest(new { message = "Failed to create Board / Group" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(groupmember);

        }

        [HttpPost("AddSocialGroupMember")]
        public async Task<ActionResult<bool>> AddSocialGroupMember([FromHeader]string data)
        {
            var isSuccess = false;
            try
            {
                if(string.IsNullOrEmpty(data))
                {
                    isSuccess = false;
                }
                else
                {
                    var groupmemberModel = JsonConvert.DeserializeObject<AddSocialGroupMember>(data);
                    isSuccess = await _groupMemberService.AddSocialGroupMember(groupmemberModel);
                    if (isSuccess == false)
                    {
                        return BadRequest(new { message = "Failed to create Board / Group" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(isSuccess);
        }

        [HttpPost("UpdateGroupMember")]
        public async Task<ActionResult<bool>> UpdateGroupMember(GroupMemberModel groupmemberModel)
        {
            bool success = false;

            try
            {
                success = await _groupMemberService.UpdateGroupMember(groupmemberModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }

        [HttpDelete("DeleteGroupMember/{groupMemberId}")]
        public async Task<ActionResult<bool>> DeleteGroupMember(int groupMemberId)
        {
            bool response = false;
            try
            {
                response = await _groupMemberService.DeleteGroupMember(groupMemberId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete group member" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpDelete("DeleteGroupMemberRole/{groupMemberRoleId}")]
        public async Task<ActionResult<bool>> DeleteGroupMemberRole(int groupMemberRoleId)
        {
            bool response = false;
            try
            {
                response = await _groupMemberRoleService.DeleteGroupMemberRole(groupMemberRoleId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete group member" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateGroupMemberRole")]
        public async Task<ActionResult<bool>> UpdateGroupMemberRole(GroupMemberRoleModel groupmemberRoleModel)
        {
            bool success = false;

            try
            {
                success = await _groupMemberRoleService.UpdateGroupMemberRole(groupmemberRoleModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(success);

        }
    }
}
