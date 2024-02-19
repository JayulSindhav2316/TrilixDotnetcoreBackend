using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class SociableGroupService : ISociableGroupService
    {
        private readonly ILogger<SociableService> _logger;
        private readonly IOrganizationService _organizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantService _tenantService;
        public SociableGroupService(ILogger<SociableService> logger, ITenantService tenantService, IOrganizationService organizationService, IUnitOfWork unitOfWork)
        {
            this._logger = logger;
            this._tenantService = tenantService;
            this._organizationService = organizationService;
            this._unitOfWork = unitOfWork;
        }

        public async Task<int> CreateSocialGroup(GroupModel groupModel)
        {
            try
            {
                int socialGroupId = 0;
                if (groupModel.OrganizationId != null)
                {
                    var organization = await _organizationService.GetOrganizationById(Convert.ToInt32(groupModel.OrganizationId));
                    var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

                    var baseUrl = tenant.SociableBaseUrl;

                    //Set Basic Auth
                    var user = tenant.SociableAdminUserName;
                    var password = tenant.SociableAdminPassword;

                    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));

                    CreateSociableGroupRequestModel createSociableGroupRequestModel = new CreateSociableGroupRequestModel();
                    CreateSociableGroupRequestLabel label = new CreateSociableGroupRequestLabel();
                    label.value = groupModel.GroupName;
                    createSociableGroupRequestModel.label.Add(label);
                    createSociableGroupRequestModel._links.type.href = baseUrl + "/rest/type/group/closed_group";

                    var requestBody = JsonConvert.SerializeObject(createSociableGroupRequestModel);

                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/entity/group?_format=json");
                    request.Headers.Add("Authorization", "Basic " + base64String);
                    request.Headers.Add("LabelType", "{\"label\" : ["+ groupModel.GroupName + "],\"type\" : \"closed_group\"}");
                    var content = new StringContent(requestBody, null, "application/hal+json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    if(response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var responseStr = await response.Content.ReadAsStringAsync();
                        var responseModel = JsonConvert.DeserializeObject<CreateSociableGroupResponseModel>(responseStr);
                        if(responseModel.id != null && responseModel.id.Any())
                        {
                            socialGroupId = responseModel.id[0].value;
                        }
                        else
                        {
                            _logger.LogInformation($"Failed to Create group: {responseStr}");
                        }
                    }
                }
                return socialGroupId;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Failed to Create group: {ex.Message}");
                return 0;
            }
        }

        public async Task<CreateSociableGroupResponseModel> GetSocialGroup(int socialGroupId, int? organizationId)
        {
            try
            {
                CreateSociableGroupResponseModel createSociableGroupResponseModel = new CreateSociableGroupResponseModel();
                if(socialGroupId != 0 && organizationId != null && organizationId != 0)
                {
                    var organization = await _organizationService.GetOrganizationById(Convert.ToInt32(organizationId));
                    var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

                    var baseUrl = tenant.SociableBaseUrl;

                    //Set Basic Auth
                    var user = tenant.SociableAdminUserName;
                    var password = tenant.SociableAdminPassword;

                    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + "/group/"+socialGroupId+"?_format=json");
                    request.Headers.Add("Authorization", "Basic " + base64String);
                    var content = new StringContent(string.Empty);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        createSociableGroupResponseModel = JsonConvert.DeserializeObject<CreateSociableGroupResponseModel>(responseStr);
                    }
                    else
                    {
                        _logger.LogInformation($"Failed to Create group: {responseStr}");
                    }
                }
                return createSociableGroupResponseModel;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Failed to Create group: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateSocialGroup(GroupModel groupModel, int socialGroupId, bool isUpdateGroup, LinkGroupRoleModel linkGroupRoleModel)
        {
            try
            {
                bool isSuccess = false;
                int? organizationId = 0;
                if(isUpdateGroup)
                {
                    organizationId = groupModel.OrganizationId;
                }
                else
                {
                    organizationId = linkGroupRoleModel.OrganizationId;
                }
                if(organizationId != null && organizationId != 0)
                {
                    var organization = await _organizationService.GetOrganizationById(Convert.ToInt32(organizationId));
                    var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

                    var baseUrl = tenant.SociableBaseUrl;

                    //Set Basic Auth
                    var user = tenant.SociableAdminUserName;
                    var password = tenant.SociableAdminPassword;

                    var requestBody = string.Empty;
                    if (isUpdateGroup)
                    {
                        var updateSociableGroupRequestModel = SetUpdateSociableGroupRequestModel(groupModel, baseUrl);
                        requestBody = JsonConvert.SerializeObject(updateSociableGroupRequestModel);
                        requestBody = requestBody.Replace("\"groupId\":null,", String.Empty);
                    }
                    else
                    {
                        var updateSocialGroupRole = await UpdateSocialGroupRole(linkGroupRoleModel, socialGroupId, baseUrl);
                        requestBody = JsonConvert.SerializeObject(updateSocialGroupRole);
                    }

                    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Patch, baseUrl + "/group/"+ socialGroupId + "?_format=json");
                    request.Headers.Add("Authorization", "Basic " + base64String);
                    string groupName = string.Empty;
                    if(groupModel != null)
                    {
                        groupName = groupModel.GroupName;
                    }
                    else if(linkGroupRoleModel != null)
                    {
                        if (linkGroupRoleModel.GroupId != null)
                        {
                            var groupDetails = await _unitOfWork.Groups.GetByIdAsync(Convert.ToInt32(linkGroupRoleModel.GroupId));
                            if(groupDetails != null)
                            {
                                groupName = groupDetails.GroupName;
                            }
                        }
                    }
                    request.Headers.Add("LabelType", "{\"label\" : [" + groupName + "],\"type\" : \"closed_group\"}");
                    var content = new StringContent(requestBody, null, "application/hal+json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    if(response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = false;
                        _logger.LogInformation($"Failed to update group: {responseStr}");
                    }
                }
                return isSuccess;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Failed to update group: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteSocialGroup(int socialGroupId, int? organizationId)
        {
            try
            {
                var isSuccess = false;
                if(organizationId != 0)
                {
                    var organization = await _organizationService.GetOrganizationById(Convert.ToInt32(organizationId));
                    var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

                    var baseUrl = tenant.SociableBaseUrl;

                    //Set Basic Auth
                    var user = tenant.SociableAdminUserName;
                    var password = tenant.SociableAdminPassword;

                    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));

                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Delete, baseUrl + "/group/"+socialGroupId+"?_format=json");
                    request.Headers.Add("Authorization", "Basic " + base64String);
                    var content = new StringContent(string.Empty);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogInformation($"Failed to delete group: {responseStr}");
                    }
                }
                return isSuccess;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Failed to update group: {ex.Message}");
                return false;
            }
        }

        public async Task<int> CreateSocialGroupMembers(SociableGroupMemberModel sociableGroupMemberModel,int? organizationId)
        {
            try
            {
                if (organizationId != 0)
                {
                    var organization = await _organizationService.GetOrganizationById(Convert.ToInt32(organizationId));
                    var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

                    var baseUrl = tenant.SociableBaseUrl;

                    //Set Basic Auth
                    var user = tenant.SociableAdminUserName;
                    var password = tenant.SociableAdminPassword;
                    var requestBody = JsonConvert.SerializeObject(sociableGroupMemberModel);
                    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/entity/group_content"  + "?_format=json");
                    request.Headers.Add("Authorization", "Basic " + base64String);
                    var content = new StringContent(requestBody, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        dynamic socialGroupMember = JsonConvert.DeserializeObject(responseStr);
                        int socialGroupMemberId = socialGroupMember.id[0]["value"].ToObject<int>();
                        return socialGroupMemberId;
                    }
                    else
                    {
                        _logger.LogInformation($"Failed to create group member: {responseStr}");
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to create group member: {ex.Message}");
                return 0;
            }
        }

        private async Task<UpdateSociableGroupRoleRequestModel> UpdateSocialGroupRole(LinkGroupRoleModel linkGroupRoleModel, int socialGroupId, string baseUrl)
        {
            try
            {
                UpdateSociableGroupRoleRequestModel updateSociableGroupRoleRequestModel = new UpdateSociableGroupRoleRequestModel();
                var socialGroupDetails = await GetSocialGroup(socialGroupId, linkGroupRoleModel.OrganizationId);
                if(socialGroupDetails != null)
                {
                    if(linkGroupRoleModel.IsLinked == (int)Status.Active)
                    {
                        FieldGroupPosition fieldGroupRole = new FieldGroupPosition
                        {
                            value = linkGroupRoleModel.GroupRoleName
                        };
                        socialGroupDetails.field_group_positions.Add(fieldGroupRole);
                    }
                    else
                    {
                        var roleToBeRemoved = socialGroupDetails.field_group_positions.FirstOrDefault(s => s.value == linkGroupRoleModel.GroupRoleName);
                        if(roleToBeRemoved != null)
                        {
                            socialGroupDetails.field_group_positions.Remove(roleToBeRemoved);
                        }
                    }

                    foreach(var item in socialGroupDetails.field_group_positions)
                    {
                        UpdateSociableGroupStringValue roleName = new UpdateSociableGroupStringValue
                        {
                            value = item.value,
                        };
                        updateSociableGroupRoleRequestModel.field_group_positions.Add(roleName);
                    }
                    updateSociableGroupRoleRequestModel._links.type.href = baseUrl + "/rest/type/group/closed_group";
                }
                return updateSociableGroupRoleRequestModel;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Failed to create group role: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateSocialGroupMembers(SociableGroupMemberModel sociableGroupMemberModel, int? organizationId, int socialGroupMemberId, int groupId)
        {
            try
            {
                if (organizationId != 0)
                {
                    var organization = await _organizationService.GetOrganizationById(Convert.ToInt32(organizationId));
                    var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

                    var baseUrl = tenant.SociableBaseUrl;

                    //Set Basic Auth
                    var user = tenant.SociableAdminUserName;
                    var password = tenant.SociableAdminPassword;
                    var requestBody = JsonConvert.SerializeObject(sociableGroupMemberModel);
                    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Patch, baseUrl + "/group/"+ groupId + "/content/"+ socialGroupMemberId + "" + "?_format=json");
                    request.Headers.Add("Authorization", "Basic " + base64String);
                    var content = new StringContent(requestBody, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogInformation($"Failed to create group member: {responseStr}");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to update group role: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteSocialGroupMembers(int? organizationId, int socialGroupMemberId, int groupId)
        {
            try
            {
                if (organizationId != 0)
                {
                    var organization = await _organizationService.GetOrganizationById(Convert.ToInt32(organizationId));
                    var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

                    var baseUrl = tenant.SociableBaseUrl;

                    //Set Basic Auth
                    var user = tenant.SociableAdminUserName;
                    var password = tenant.SociableAdminPassword;
                    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Delete, baseUrl + "/group/" + groupId + "/content/" + socialGroupMemberId + "" + "?_format=json");
                    request.Headers.Add("Authorization", "Basic " + base64String);
                    var content = new StringContent(string.Empty);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogInformation($"Failed to create group member: {responseStr}");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to update group role: {ex.Message}");
                return false;
            }
        }

        private UpdateSociableGroupRequestModel SetUpdateSociableGroupRequestModel(GroupModel groupModel, string baseUrl)
        {
            UpdateSociableGroupRequestModel updateSociableGroupRequestModel = new UpdateSociableGroupRequestModel();
            UpdateSociableGroupStringValue name = new UpdateSociableGroupStringValue
            {
                value = groupModel.GroupName
            };
            updateSociableGroupRequestModel.field_name.Add(name);
            UpdateSociableGroupStringValue description = new UpdateSociableGroupStringValue
            {
                value = groupModel.GroupDescription
            };
            updateSociableGroupRequestModel.field_group_description.Add(description);
            UpdateSociableGroupNumberValue targetSize = new UpdateSociableGroupNumberValue
            {
                value = groupModel.PreferredNumbers
            };
            updateSociableGroupRequestModel.field_target_size.Add(targetSize);
            UpdateSociableGroupBoolValue isActive = new UpdateSociableGroupBoolValue
            {
                value = groupModel.IsActive
            };
            updateSociableGroupRequestModel.field_isactive.Add(isActive);
            UpdateSociableGroupBoolValue term = new UpdateSociableGroupBoolValue
            {
                value = groupModel.IsTermApplied
            };
            updateSociableGroupRequestModel.field_term.Add(term);
            UpdateSociableGroupStringValue startDate = new UpdateSociableGroupStringValue
            {
                value = groupModel.TerrmStartDate != null ? groupModel.TermEndDate.Value.ToString("yyyy-MM-ddTHH:mm:sszzz") : null
            };
            updateSociableGroupRequestModel.field_start_date.Add(startDate);
            UpdateSociableGroupStringValue endDate = new UpdateSociableGroupStringValue
            {
                value = groupModel.TermEndDate != null ? groupModel.TermEndDate.Value.ToString("yyyy-MM-ddTHH:mm:sszzz") : null
            };
            updateSociableGroupRequestModel.field_end_date.Add(endDate);
            var linkedRoles = groupModel.Roles.Where(s => s.IsLinked == (int)Status.Active);
            if(linkedRoles.Any())
            {
                var selectLinkedRoleNames = linkedRoles.Select(s => s.GroupRoleName).ToList();
                foreach(var item in selectLinkedRoleNames)
                {
                    UpdateSociableGroupStringValue roleName = new UpdateSociableGroupStringValue
                    {
                        value = item
                    };
                    updateSociableGroupRequestModel.field_group_positions.Add(roleName);
                }
            }
            updateSociableGroupRequestModel._links.type.href = baseUrl + "/rest/type/group/closed_group";
            return updateSociableGroupRequestModel;
        }
    }    
}
