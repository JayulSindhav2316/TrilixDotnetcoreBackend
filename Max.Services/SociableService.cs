using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class SociableService : ISociableService
    {

        private readonly ITenantService _tenantService;
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<SociableService> _logger;


        public SociableService(ILogger<SociableService> logger, ITenantService tenantService, IOrganizationService organizationService)
        {
            this._logger = logger;
            this._tenantService = tenantService;
            this._organizationService = organizationService;
        }

        public async Task<int> CreatePerson(PersonModel person, CompanyModel companyModel, int organizationId)
        {
            //Get current tenant configuration
            _logger.LogInformation("Creating Sociable User");

            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            using (var client = new HttpClient())
            {
                var uri = "entity/user?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();

                if (person != null)
                {
                    var PrimaryEmail = person.Emails.GetPrimaryEmail();
                    var loginName = person.Entity.WebLoginName;
                    sb.Append("{\n");
                    sb.Append("\"name\":[{\"value\":");
                    sb.Append($"\"{loginName}\"}}],\n");
                    sb.Append("\"mail\":[{\"value\":");
                    sb.Append($"\"{PrimaryEmail}\"}}],\n");
                    sb.Append("\"status\":[{\"value\":");
                    sb.Append("\"1\"}],\n");
                    sb.Append("\"field_entity_id\":[{\"value\":");
                    sb.Append($"\"{person.EntityId}\"}}],\n");
                    sb.Append("\"roles\":[{\"target_id\":");
                    sb.Append("\"authenticated\"}\n");
                    sb.Append(",{\"target_id\":\"verified\"}\n");
                    if (person.IsSociablemanager)
                    {
                        sb.Append(",{\"target_id\":\"sociable_manager\"}\n");
                    }
                    if (person.IsBillableManager)
                    {
                        sb.Append(",{\"target_id\":\"billable_account\"}\n");
                    }
                    sb.Append("],\n");
                }
                else if (companyModel != null)
                {
                    var PrimaryEmail = companyModel.Emails.GetPrimaryEmail();
                    var loginName = companyModel.Entity?.WebLoginName;
                    if (string.IsNullOrEmpty(loginName))
                    {
                        loginName = companyModel.CompanyName;
                    }
                    sb.Append("{\n");
                    sb.Append("\"name\":[{\"value\":");
                    sb.Append($"\"{loginName}\"}}],\n");
                    sb.Append("\"mail\":[{\"value\":");
                    sb.Append($"\"{PrimaryEmail}\"}}],\n");
                    sb.Append("\"status\":[{\"value\":");
                    sb.Append("\"1\"}],\n");
                    sb.Append("\"field_entity_id\":[{\"value\":");
                    if (companyModel.EntityId != 0)
                    {
                        sb.Append($"\"{companyModel.EntityId}\"}}],\n");
                    }
                    else
                    {
                        sb.Append($"\"{companyModel.Entity?.EntityId}\"}}],\n");
                    }
                    sb.Append("\"roles\":[{\"target_id\":");
                    sb.Append("\"company\"}],\n");
                }
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/user/user\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);


                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // synchronous request without the need for .ContinueWith() or await
                try
                {
                    var response = client.PostAsync(uri, _Body).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        dynamic member = JsonConvert.DeserializeObject(stringResponse);

                        int userid = member.uid[0]["value"].ToObject<int>();
                        if (person != null)
                        {
                            _logger.LogInformation($"User Created Person Id:{person.PersonId} User Id:{userid}");
                        }
                        else if (companyModel != null)
                        {
                            _logger.LogInformation($"User Created company Id:{companyModel.CompanyId} User Id:{userid}");
                        }
                        return userid;
                    }
                    else
                    {
                        _logger.LogError($"CreateMember failed. Response: {await response.Content.ReadAsStringAsync()}");
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"CreateMember {ex.Message}");
                    return -1;
                }

            }
        }

        public async Task<SociableApiResponseModel> CreateStaffUser(StaffUserModel staffUser, int organizationId, bool isContentManager)
        {
            //Get current tenant configuration
            _logger.LogInformation("Creating Sociable User");
            var apiResponse = new SociableApiResponseModel();

            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            apiResponse.StaffUserId = staffUser.UserId;
            using (var client = new HttpClient())
            {
                var uri = "entity/user?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();
                var PrimaryEmail = staffUser.Email;
                var loginName = staffUser.UserName;
                var loginPassword = staffUser.Password;
                sb.Append("{\n");
                sb.Append("\"name\":[{\"value\":");
                sb.Append($"\"{loginName}\"}}],\n");
                sb.Append("\"pass\":[{\"value\":");
                sb.Append($"\"{loginPassword}\"}}],\n");
                sb.Append("\"mail\":[{\"value\":");
                sb.Append($"\"{PrimaryEmail}\"}}],\n");
                sb.Append("\"status\":[{\"value\":");
                sb.Append("\"1\"}],\n");
                sb.Append("\"field_staffuser_id\":[{\"value\":");
                sb.Append($"\"{staffUser.UserId}\"}}],\n");
                sb.Append("\"roles\":[{\"target_id\":");
                if (isContentManager)
                {
                    sb.Append("\"contentmanager\"},\n");
                }
                else
                {
                    sb.Append("\"authenticated\"},\n");
                }
                sb.Append("{\"target_id\":\"verified\"}],\n");
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/user/user\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);


                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // synchronous request without the need for .ContinueWith() or await
                try
                {
                    var response = client.PostAsync(uri, _Body).Result;
                    apiResponse.ResponseStatus.StatusCode = response.StatusCode;
                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        dynamic member = JsonConvert.DeserializeObject(stringResponse);

                        int userId = member.uid[0]["value"].ToObject<int>();
                        apiResponse.SociableUserId = userId;
                        apiResponse.ResponseStatus.Message = $"CreateStaffUser: User Created Staff User Id:{staffUser.UserName} User Id:{userId}";

                        _logger.LogInformation($"CreateStaffUser: User Created Staff User Id:{staffUser.UserName} User Id:{userId}");
                        return apiResponse;
                    }
                    else
                    {
                        _logger.LogError($"CreateStaffUser failed. Response: {await response.Content.ReadAsStringAsync()}");
                        apiResponse.ResponseStatus.Message = await response.Content.ReadAsStringAsync();
                        return apiResponse;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"CreateStaffUser {ex.Message}");
                    apiResponse.ResponseStatus.Message = $"CreateStaffUser {ex.Message}";
                    return apiResponse;
                }

            }
        }

        public async Task<int> CreatePersonProfile(PersonModel person, CompanyModel companyModel, int organizationId)
        {
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            using (var client = new HttpClient())
            {
                var uri = "entity/profile?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                string organizationName = string.Empty;

                if (person.Company != null)
                {
                    organizationName = person.Company.CompanyName;
                }

                StringBuilder sb = new StringBuilder();

                sb.Append("{\n");
                sb.Append("\"type\":[{\"value\":");
                sb.Append($"\"profile\"}}],\n");
                sb.Append("\"is_default\":[{\"value\":");
                sb.Append($"\"1\"}}],\n");
                sb.Append("\"status\":[{\"value\":");
                sb.Append("\"1\"}],\n");
                if (person != null)
                {
                    sb.Append("\"uid\":[{\"target_id\":");
                    sb.Append($"\"{person.Entity.SociableUserId}\"}}],\n");
                    sb.Append("\"field_profile_first_name\":[{\"value\":");
                    sb.Append($"\"{person.FirstName}\"}}],\n");
                    sb.Append("\"field_profile_last_name\":[{\"value\":");
                    sb.Append($"\"{person.LastName}\"}}],\n");
                    sb.Append("\"field_profile_organization\":[{\"value\":");
                    sb.Append($"\"{organizationName}\"}}],\n");
                    sb.Append("\"field_organization_id\":[{\"value\":");
                    sb.Append($"\"{person.OrganizationId}\"}}],\n");
                    sb.Append("\"field_profile_phone_number\":[{\"value\":");
                    sb.Append($"\"{person.PrimaryPhone}\"}}],\n");
                    sb.Append("\"field_profile_self_introduction\":[{\"value\":");
                    sb.Append("\"field_profile_title\":[{\"value\":");
                    sb.Append($"\"{person.Title}\"}}],\n");
                    sb.Append("\"field_profile_address\":[{");
                    sb.Append($"\"locality\":\"{person.City}\",");
                    sb.Append($"\"administrative_area\":\"{person.State}\",");
                    sb.Append($"\"postal_code\":\"{person.Zip}\",");
                    sb.Append($"\"country_code\":\"{person.Country}\",");
                    sb.Append($"\"address_line1\":\"{person.StreetAddress}\"");

                }
                else if (companyModel != null)
                {
                    var primaryCompanyPhone = companyModel.Phones.GetPrimaryPhoneNumber();
                    sb.Append("\"uid\":[{\"target_id\":");
                    sb.Append($"\"{companyModel.Entity.SociableUserId}\"}}],\n");
                    sb.Append("\"field_profile_first_name\":[{\"value\":");
                    sb.Append($"\"{companyModel.CompanyName}\"}}],\n");
                    sb.Append("\"field_profile_organization\":[{\"value\":");
                    sb.Append($"\"{companyModel.CompanyName}\"}}],\n");
                    sb.Append("\"field_organization_id\":[{\"value\":");
                    sb.Append($"\"{companyModel.OrganizationId}\"}}],\n");
                    sb.Append("\"field_profile_phone_number\":[{\"value\":");
                    sb.Append($"\"{primaryCompanyPhone}\"}}],\n");
                    sb.Append("\"field_profile_address\":[{");
                    sb.Append($"\"locality\":\"{companyModel.City}\",");
                    sb.Append($"\"administrative_area\":\"{companyModel.State}\",");
                    sb.Append($"\"postal_code\":\"{companyModel.Zip}\",");
                    sb.Append($"\"country_code\":\"{companyModel.Country}\",");
                    sb.Append($"\"address_line1\":\"{companyModel.StreetAddress}\"");
                }
                sb.Append("}],");
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/profile/profile\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);


                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    // synchronous request without the need for .ContinueWith() or await
                    var response = client.PostAsync(uri, _Body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        dynamic profile = JsonConvert.DeserializeObject(stringResponse);
                        int profileId = profile.profile_id[0]["value"].ToObject<int>();
                        _logger.LogInformation($"Profile Created Person Id:{person.PersonId} ProfileId:{profileId}");
                        return profileId;
                    }
                    else
                    {
                        _logger.LogError($"CreateProfile failed. Response: {await response.Content.ReadAsStringAsync()}");
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"CreateMemberProfile {ex.Message}");
                    return -1;
                }
            }
        }

        public async Task<bool> UpdatePerson(int sociableUserId, string loginName, string loginPassword, string primaryEmail, int organizationId, bool isBillableManager, bool isSoicableManager, bool isUpdateRoles)
        {
            _logger.LogInformation($"Update User Info in Sociable.");
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            using (var client = new HttpClient())
            {
                var uri = $"user/{sociableUserId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();
                if (loginPassword == string.Empty)
                {
                    sb.Append("{\n");
                    sb.Append("\"name\":[{\"value\":");
                    sb.Append($"\"{loginName}\"}}],\n");
                    sb.Append("\"mail\":[{\"value\":");
                    sb.Append($"\"{primaryEmail}\"}}],\n");
                    sb.Append("\"status\":[{\"value\":");
                    sb.Append($"\"{1}\"}}],\n");
                }
                else
                {
                    sb.Append("{\n");
                    sb.Append("\"name\":[{\"value\":");
                    sb.Append($"\"{loginName}\"}}],\n");
                    sb.Append("\"pass\":[{\"value\":");
                    sb.Append($"\"{loginPassword}\"}}],\n");
                    sb.Append("\"mail\":[{\"value\":");
                    sb.Append($"\"{primaryEmail}\"}}],\n");
                    sb.Append("\"status\":[{\"value\":");
                    sb.Append($"\"{1}\"}}],\n");
                }
                if (isUpdateRoles)
                {
                    sb.Append("\"roles\":[{\"target_id\":");
                    sb.Append("\"authenticated\"}\n");
                    sb.Append(",{\"target_id\":\"verified\"}\n");
                    if (isSoicableManager)
                    {
                        sb.Append(",{\"target_id\":\"sociable_manager\"}\n");
                    }
                    if (isBillableManager)
                    {
                        sb.Append(",{\"target_id\":\"billable_account\"}\n");
                    }
                    sb.Append("],\n");
                }
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/user/user\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // synchronous request without the need for .ContinueWith() or await
                try
                {
                    var response = client.PatchAsync(uri, _Body).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation($"User Updated Sociable User Id:{loginName}");
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"CreateProfile failed. Response: {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"UpdatePerson {ex.Message}");
                    return false;
                }

            }
        }

        public async Task<SociableApiResponseModel> UpdateStaffUser(int sociableUserId, string loginName, string loginPassword, string primaryEmail, int organizationId, bool isContentManager, bool isNewContentManager)
        {
            _logger.LogInformation($"Update User Info in Sociable.");
            var apiResponse = new SociableApiResponseModel();

            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            apiResponse.SociableUserId = sociableUserId;
            using (var client = new HttpClient())
            {
                var uri = $"user/{sociableUserId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();
                if (loginPassword == string.Empty)
                {
                    sb.Append("{\n");
                    sb.Append("\"name\":[{\"value\":");
                    sb.Append($"\"{loginName}\"}}],\n");
                    sb.Append("\"mail\":[{\"value\":");
                    sb.Append($"\"{primaryEmail}\"}}],\n");
                    sb.Append("\"status\":[{\"value\":");
                    sb.Append($"\"{1}\"}}],\n");
                }
                else
                {
                    sb.Append("{\n");
                    sb.Append("\"name\":[{\"value\":");
                    sb.Append($"\"{loginName}\"}}],\n");
                    sb.Append("\"pass\":[{\"value\":");
                    sb.Append($"\"{loginPassword}\"}}],\n");
                    sb.Append("\"mail\":[{\"value\":");
                    sb.Append($"\"{primaryEmail}\"}}],\n");
                    sb.Append("\"status\":[{\"value\":");
                    sb.Append($"\"{1}\"}}],\n");
                }
                sb.Append("\"roles\":[{\"target_id\":");
                if (isNewContentManager)
                {
                    sb.Append("\"contentmanager\"},\n");
                }
                if (!isContentManager)
                {
                    sb.Append("{\"target_id\":\"authenticated\"},\n");
                }
                if (isNewContentManager || !isContentManager)
                {
                    sb.Append("{\"target_id\":\"verified\"}],\n");
                }
                else
                {
                    sb.Append("\"verified\"}],\n");
                }
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/user/user\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // synchronous request without the need for .ContinueWith() or await
                try
                {
                    var response = client.PatchAsync(uri, _Body).Result;
                    apiResponse.ResponseStatus.StatusCode = response.StatusCode;
                    string stringResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic member = JsonConvert.DeserializeObject(stringResponse);
                        int profileId = member.profile_profiles[0]["value"].ToObject<int>();
                        _logger.LogInformation($"User Updated Sociable User Id:{loginName}");
                        apiResponse.SociableProfileId = profileId;
                        apiResponse.ResponseStatus.Message = $"User Updated Sociable User Id:{loginName}";
                        return apiResponse;
                    }
                    else
                    {
                        _logger.LogError($"UpdateStaffUser {stringResponse}");
                        apiResponse.ResponseStatus.Message = $"UpdateStaffUser {stringResponse}";
                        return apiResponse;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"UpdateStaffUser {ex.Message}");
                    apiResponse.ResponseStatus.Message = $"UpdateStaffUser {ex.Message}";
                    return apiResponse;
                }

            }
        }

        public async Task<int> UpdatePersonProfile(PersonModel person, CompanyModel companyModel, int profileId, int organizationId)
        {
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            using (var client = new HttpClient())
            {
                var uri = $"profile/{profileId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();

                sb.Append("{\n");
                sb.Append("\"status\":[{\"value\":");
                sb.Append("\"1\"}],\n");
                if (person != null)
                {
                    var preferredContact = Enum.GetName(typeof(PreferredContact), person.PreferredContact ?? 0);
                    var primaryAddress = person.Addresses.GetPrimaryAddress();
                    var PrimaryPhone = person.Phones.GetPrimaryPhoneNumber();
                    var primaryEmail = person.Emails.GetPrimaryEmail();

                    string dob = string.Empty;
                    try
                    {
                        dob = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex)
                    {
                        dob = "1900-01-01";
                    }

                    string organizationName = string.Empty;

                    if (person.Company != null)
                    {
                        organizationName = person.Company.CompanyName;
                    }

                    sb.Append("\"field_profile_first_name\":[{\"value\":");
                    sb.Append($"\"{person.FirstName}\"}}],\n");
                    sb.Append("\"field_profile_last_name\":[{\"value\":");
                    sb.Append($"\"{person.LastName}\"}}],\n");
                    sb.Append("\"field_title\":[{\"value\":");
                    sb.Append($"\"{person.Title}\"}}],\n");
                    sb.Append("\"field_designation\":[{\"value\":");
                    sb.Append($"\"{person.Designation}\"}}],\n");
                    sb.Append("\"field_preferred_contact\":[{\"value\":");
                    sb.Append($"\"{preferredContact}\"}}],\n");
                    sb.Append("\"field_preferred_contact_list\":[{\"value\":");
                    sb.Append($"\"{person.PreferredContact ?? 0}\"}}],\n");
                    sb.Append("\"field_profile_organization\":[{\"value\":");
                    sb.Append($"\"{organizationName}\"}}],\n");
                    sb.Append("\"field_organization_id\":[{\"value\":");
                    sb.Append($"\"{person.OrganizationId}\"}}],\n");
                    sb.Append("\"field_profile_phone_number\":[{\"value\":");
                    sb.Append($"\"{PrimaryPhone}\"}}],\n");
                    sb.Append("\"field_birthdate\":[{\"value\":");
                    sb.Append($"\"{dob}\"}}],\n");
                    sb.Append("\"field_prefix_list\":[{\"value\":");
                    sb.Append($"\"{person.Prefix}\"}}],\n");
                    sb.Append("\"field_suffix\":[{\"value\":");
                    sb.Append($"\"{person.Suffix}\"}}],\n");
                    sb.Append("\"field_preferred_name\":[{\"value\":");
                    sb.Append($"\"{person.CasualName}\"}}],\n");
                    sb.Append("\"field_middle_name\":[{\"value\":");
                    sb.Append($"\"{person.MiddleName}\"}}],\n");
                    sb.Append("\"field_profile_address\":[{");
                    sb.Append($"\"locality\":\"{primaryAddress.City}\",");
                    sb.Append($"\"administrative_area\":\"{primaryAddress.StateCode}\",");
                    sb.Append($"\"postal_code\":\"{primaryAddress.FormattedZip}\",");
                    sb.Append($"\"country_code\":\"{primaryAddress.CountryCode}\",");
                    sb.Append($"\"address_line1\":\"{primaryAddress.StreetAddress.Replace("\n", "").Replace("\r", "")}\"");
                    sb.Append("}],");
                    sb.Append("\"field_gender\":[{\"value\":");
                    sb.Append($"\"{person.Gender}\"}}],\n");
                    sb.Append("\"field_primary_phone_number\":[{\"value\":");
                    sb.Append($"\"{PrimaryPhone}\"}}],\n");
                    sb.Append("\"field_primary_email_address\":[{\"value\":");
                    sb.Append($"\"{primaryEmail}\"}}],\n");
                    sb.Append("\"field_facebook\":[{\"value\":");
                    sb.Append($"\"{person.FacebookName}\"}}],\n");
                    sb.Append("\"field_twitter\":[{\"value\":");
                    sb.Append($"\"{person.TwitterName}\"}}],\n");
                    sb.Append("\"field_linked_in\":[{\"value\":");
                    sb.Append($"\"{person.LinkedinName}\"}}],\n");
                    sb.Append("\"field_web_site\":[{\"value\":");
                    sb.Append($"\"{person.Website}\"}}],\n");
                    sb.Append("\"field_personid\":[{\"value\":");
                    sb.Append($"\"{person.PersonId}\"}}],\n");
                    if (person.SocialCompanyId > 0)
                    {
                        sb.Append("\"field_company\":[{\"target_id\":");
                        sb.Append($"\"{person.SocialCompanyId}\"}}],\n");
                    }
                    if (!string.IsNullOrEmpty(person.Entity?.MembershipType))
                    {
                        if (person.Entity?.MembershipType.ToLower() != "non member")
                        {
                            sb.Append("\"field_membership_status\":[{\"value\":");
                            sb.Append($"\"{1}\"}}],\n");
                        }
                        else
                        {
                            sb.Append("\"field_membership_status\":[{\"value\":");
                            sb.Append($"\"{0}\"}}],\n");
                        }

                        sb.Append("\"field_membership_type\":[{\"value\":");
                        sb.Append($"\"{person.Entity?.MembershipType}\"}}],\n");
                    }
                }
                else if (companyModel != null)
                {
                    var primaryAddress = companyModel.Addresses.GetPrimaryAddress();
                    var PrimaryPhone = companyModel.Phones.GetPrimaryPhoneNumber();
                    var primaryEmail = companyModel.Emails.GetPrimaryEmail();

                    sb.Append("\"field_company_name\":[{\"value\":");
                    sb.Append($"\"{companyModel.CompanyName}\"}}],\n");
                    sb.Append("\"field_company_link\":[{\"value\":");
                    sb.Append($"\"{companyModel.Website}\"}}],\n");
                    sb.Append("\"field_designation\":[{\"value\":");
                    sb.Append($"\"{companyModel.Designation}\"}}],\n");
                    //sb.Append("\"field_preferred_contact\":[{\"value\":");
                    //sb.Append($"\"Primary Email\"}}],\n");
                    sb.Append("\"field_company_email\":[{\"value\":");
                    sb.Append($"\"{primaryEmail}\"}}],\n");
                    sb.Append("\"field_profile_organization\":[{\"value\":");
                    sb.Append($"\"{companyModel.CompanyName}\"}}],\n");
                    sb.Append("\"field_organization_id\":[{\"value\":");
                    sb.Append($"\"{companyModel.OrganizationId}\"}}],\n");
                    sb.Append("\"field_company_phone_number\":[{\"value\":");
                    sb.Append($"\"{PrimaryPhone}\"}}],\n");
                    sb.Append("\"field_profile_address\":[{");
                    sb.Append($"\"locality\":\"{primaryAddress.City}\",");
                    sb.Append($"\"administrative_area\":\"{primaryAddress.StateCode}\",");
                    sb.Append($"\"postal_code\":\"{primaryAddress.FormattedZip}\",");
                    sb.Append($"\"country_code\":\"{primaryAddress.CountryCode}\",");
                    sb.Append($"\"address_line1\":\"{primaryAddress.StreetAddress}\"");
                    sb.Append("}],");
                    sb.Append("\"field_company_id\":[{\"value\":");
                    if (companyModel.CompanyId != 0)
                    {
                        sb.Append($"\"{companyModel.CompanyId}\"}}],\n");
                    }
                    else
                    {
                        sb.Append($"\"{companyModel.Entity?.CompanyId}\"}}],\n");
                    }
                    if (companyModel.SociablePrimaryContact != 0)
                    {
                        sb.Append("\"field_primary_contact\":[{\"target_id\":");
                        sb.Append($"\"{companyModel.SociablePrimaryContact}\"}}],\n");
                    }
                    if (companyModel.SociableBillableContact != 0)
                    {
                        sb.Append("\"field_billable_contact\":[{\"target_id\":");
                        sb.Append($"\"{companyModel.SociableBillableContact}\"}}],\n");
                    }
                    if (companyModel.SociableManager.Any())
                    {
                        sb.Append("\"field_sociable_manager\":[");
                        int counter = 0;
                        foreach (var item in companyModel.SociableManager)
                        {
                            counter++;
                            sb.Append("{");
                            sb.Append($"\"target_id\":\"{item}\"");
                            sb.Append("}");
                            if (companyModel.SociableManager.Count() > counter)
                            {
                                sb.Append(",");
                            }
                        }
                        sb.Append("],");
                    }
                    if (!string.IsNullOrEmpty(companyModel.Entity?.MembershipType))
                    {
                        if (companyModel.Entity?.MembershipType.ToLower() != "non member")
                        {
                            sb.Append("\"field_membership_status\":[{\"value\":");
                            sb.Append($"\"{1}\"}}],\n");
                        }
                        else
                        {
                            sb.Append("\"field_membership_status\":[{\"value\":");
                            sb.Append($"\"{0}\"}}],\n");
                        }

                        sb.Append("\"field_membership_type\":[{\"value\":");
                        sb.Append($"\"{companyModel.Entity?.MembershipType}\"}}],\n");
                    }
                }
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/profile/profile\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);


                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    // synchronous request without the need for .ContinueWith() or await
                    var response = client.PatchAsync(uri, _Body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        if (person != null)
                        {
                            _logger.LogInformation($"Profile updated PersonId:{person.PersonId} ProfileId: {profileId}");
                        }
                        else if (companyModel != null)
                        {
                            _logger.LogInformation($"Profile updated companyId:{companyModel.CompanyId} ProfileId: {profileId}");
                        }
                        return profileId;
                    }
                    else
                    {
                        _logger.LogError($"UpdatePersonProfile Error: {await response.Content.ReadAsStringAsync()}");
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"UpdatePersonProfile {ex.Message}");
                    return -1;
                }
            }
        }

        public async Task<bool> UpdateCustomFields(string fieldName, string fieldvalue, int profileId, int organizationId)
        {
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            using (var client = new HttpClient())
            {
                var uri = $"profile/{profileId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fieldvalue))
                {
                    sb.Append("{\n");
                    if ("field_" + fieldName.ToLower() == "field_votingindividual")
                    {
                        var value = "0";
                        sb.Append("\"field_votingindividual\":[{\"value\":");
                        if (fieldvalue.Replace("\"", "").ToString() == "Yes")
                        {
                            value = "1";
                        }
                        sb.Append("\"" + value + "\"}],\n");
                    }
                    sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/profile/profile\"}}\n}");
                }


                string body = sb.ToString();

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    // synchronous request without the need for .ContinueWith() or await
                    var response = client.PatchAsync(uri, _Body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"UpdateCustomFields Error: {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"UpdateCustomFields {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> RemoveCompanyForUser(int profileId, int organizationId)
        {
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            using (var client = new HttpClient())
            {
                var uri = $"profile/{profileId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();

                sb.Append("{\n");
                sb.Append("\"field_company\":[],\n");
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/profile/profile\"}}\n}");


                string body = sb.ToString();

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    // synchronous request without the need for .ContinueWith() or await
                    var response = client.PatchAsync(uri, _Body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"UpdateCustomFields Error: {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"UpdateCustomFields {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<SociableApiResponseModel> UpdateUserProfile(StaffUserModel userModel, int profileId, int organizationId)
        {
            var apiResponse = new SociableApiResponseModel();
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            apiResponse.SociableProfileId = profileId;
            using (var client = new HttpClient())
            {
                var uri = $"profile/{profileId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();

                var PrimaryPhone = userModel.CellPhoneNumber;

                string organizationName = string.Empty;


                sb.Append("{\n");
                sb.Append("\"status\":[{\"value\":");
                sb.Append("\"1\"}],\n");
                sb.Append("\"field_profile_first_name\":[{\"value\":");
                sb.Append($"\"{userModel.FirstName}\"}}],\n");
                sb.Append("\"field_profile_last_name\":[{\"value\":");
                sb.Append($"\"{userModel.LastName}\"}}],\n");
                sb.Append("\"field_profile_phone_number\":[{\"value\":");
                sb.Append($"\"{PrimaryPhone}\"}}],\n");
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/profile/profile\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);


                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    // synchronous request without the need for .ContinueWith() or await
                    var response = client.PatchAsync(uri, _Body).Result;
                    apiResponse.ResponseStatus.StatusCode = response.StatusCode;
                    string result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Profile updated PersonId:{userModel.UserName} ProfileId: {profileId}");
                    apiResponse.ResponseStatus.Message = $"Profile updated User:{userModel.UserName} ProfileId: {profileId}";
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"UpdateUserProfile {ex.Message}");
                    apiResponse.ResponseStatus.Message = $"UpdateUserProfile failed:{userModel.UserName} ProfileId: {profileId} {ex.Message}";
                    return apiResponse;
                }
            }
        }

        public async Task<string> GetUserById(int userId, int organizationId)
        {
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            using (var client = new HttpClient())
            {
                var uri = $"user/{userId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                // Construct an HttpContent from a StringContent
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // synchronous request without the need for .ContinueWith() or await
                var response = client.GetAsync(uri).Result;

                string result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response:" + result);
                return result;
            }
        }

        public async Task<bool> UpdateProfileImage(DocumentModel documentModel)
        {
            string loggerMessage = string.Empty;
            string loggerErrorMessage = string.Empty;
            var organization = await _organizationService.GetOrganizationById(documentModel.OrganizationId ?? 0);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            using (var client = new HttpClient())
            {
                string base64ImageData = Convert.ToBase64String(documentModel.Document);

                var uri = "/api/updateProfilePicture?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();

                sb.Append("{\n");
                if (documentModel.EntityId > 0)
                {
                    sb.Append("\"entityId\":[{\"value\":");
                    sb.Append($"\"{documentModel.EntityId}\"}}],\n");
                    loggerMessage = $"Updated Profile Image for Member Id :{documentModel.EntityId}";
                    loggerErrorMessage = $"Profile Picture update failed for Member Id : {documentModel.EntityId}. Response : ";
                }
                else if (documentModel.StaffId > 0)
                {
                    sb.Append("\"staff\":[{\"value\":");
                    sb.Append($"\"{documentModel.StaffId}\"}}],\n");
                    loggerMessage = $"Updated Profile Image for Staff Id:{documentModel.StaffId}";
                    loggerErrorMessage = $"Profile Picture update failed for Staff Id : {documentModel.StaffId}. Response : ";
                }

                sb.Append("\"mime\":[{\"value\":");
                sb.Append($"\"{documentModel.ContentType}\"}}],\n");
                sb.Append("\"name\":[{\"value\":");
                sb.Append($"\"{documentModel.DisplayFileName}\"}}],\n");
                sb.Append("\"content\":[{\"value\":");
                sb.Append($"\"{base64ImageData}\"}}]\n");
                sb.Append("}");

                string body = sb.ToString();

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    // synchronous request without the need for .ContinueWith() or await
                    var response = client.PostAsync(uri, _Body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        dynamic profile = JsonConvert.DeserializeObject(stringResponse);
                        _logger.LogInformation(loggerMessage);
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"{loggerErrorMessage} {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{loggerErrorMessage} {ex.Message}");
                    return false;
                }
            }
        }
        public async Task<bool> UpdatePersonMembership(string membershipType, int membershipStatus, int profileId, int organizationId)
        {
            var organization = await _organizationService.GetOrganizationById(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            using (var client = new HttpClient())
            {
                var uri = $"profile/{profileId}?_format=json";
                client.BaseAddress = new Uri(tenant.SociableBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = tenant.SociableAdminUserName;
                var password = tenant.SociableAdminPassword;

                StringBuilder sb = new StringBuilder();

                sb.Append("{\n");
                sb.Append("\"field_membership_status\":[{\"value\":");
                sb.Append($"\"{membershipStatus}\"}}],\n");
                sb.Append("\"field_membership_type\":[{\"value\":");
                sb.Append($"\"{membershipType}\"}}],\n");
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + tenant.SociableBaseUrl + "/rest/type/profile/profile\"}}\n}");

                string body = sb.ToString();
                //string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);


                // Construct an HttpContent from a StringContent
                HttpContent _Body = new StringContent(body);
                _Body.Headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    // synchronous request without the need for .ContinueWith() or await
                    var response = client.PatchAsync(uri, _Body).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation($"Profile Membership updated Profile Id:{profileId}");
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"UpdatePersonMembership Error: {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"UpdatePersonMembership {ex.Message}");
                    return false;
                }
            }
        }
    }
}
