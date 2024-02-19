using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using Twilio.Types;

namespace Max.Import
{
    class Programda
    {
        public static void Mainda(string[] args)
        {

            MainAsyncd(args).GetAwaiter().GetResult();

        }
        public static async System.Threading.Tasks.Task MainAsyncd(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=max_usccb;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=max_usccb;uid=root;password=Anit1066";


            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM person  inner join entity on entity.PersonId = person.PersonId inner join email on email.PersonId = person.PersonId inner join phone on phone.PersonId = person.PersonId inner join address on address.PersonId = person.PersonId inner join company on company.CompanyId = person.CompanyId where PhoneType = 'Home' and email.isPrimary=1 and sociableuserid is null; ", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string personId = reader.GetValue(0).ToString();
                string prefix = reader.GetValue(1).ToString();
                string firstName = reader.GetValue(2).ToString();
                string lastName = reader.GetValue(3).ToString();
                string middleName = reader.GetValue(4).ToString();
                string casualName = reader.GetValue(5).ToString();
                string suffix = reader.GetValue(6).ToString();
                string title = reader.GetValue(8).ToString();
                string dob = reader.GetValue(10).ToString();
                string entityId = reader.GetValue(20).ToString();
                string emailId = reader.GetValue(39).ToString();
                string phoneNumber = reader.GetValue(44).ToString();
                string address1 = reader.GetValue(49).ToString();
                string address2 = reader.GetValue(50).ToString();
                string city = reader.GetValue(52).ToString();
                string state = reader.GetValue(53).ToString();
                string zip = reader.GetValue(54).ToString();
                string country = reader.GetValue(55).ToString();
                string organization = reader.GetValue(59).ToString();
                string preferredContact = reader.GetValue(19).ToString();
                Console.WriteLine($"Creating Person In Social {personId}");

                firstName = firstName.Replace(" ", "");
                lastName = lastName.Replace(" ", "");

                try
                {
                    DateTime birhDate = DateTime.Parse(dob);
                    dob = birhDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    dob = "1900-01-01";
                }

                if(emailId == "#N/A" || emailId == "")
                {
                    emailId = $"{firstName.Trim()}{lastName.Trim()}@testemail.com";
                }
                if (phoneNumber == "#N/A" || phoneNumber == "")
                {
                    phoneNumber = $"1111111111";
                }
                try
                {

                    int sociableUserId = await CreatePersonAsync(personId, firstName.Trim() + lastName.Trim(), emailId, entityId);

                    if (sociableUserId > 0)
                    {

                        var userInfo = await GetUserByIdAsync(sociableUserId);

                        dynamic profile = JObject.Parse(userInfo);
                        var profileId = profile.profile_profiles[0].target_id;
                        if (profileId > 0)
                        {
                            int newProfileId = (int)profileId;
                            var result = UpdatePersonProfile(newProfileId, prefix, firstName, lastName, middleName, casualName, suffix, title, dob, entityId, organization, phoneNumber, emailId, address1, address2, city, state, zip, preferredContact);

                            if (result)
                            {
                                using var targetConnection = new MySqlConnection(targetConnectionString);
                                targetConnection.Open();

                                string updateSql = $"UPDATE Entity Set SociableUserId={sociableUserId}, SociableProfileId={profileId} WHERE EntityId={entityId};";
                                MySqlCommand cmdUpdateEntity = new MySqlCommand(updateSql, targetConnection);
                                cmdUpdateEntity.ExecuteNonQuery();
                                targetConnection.Clone();
                            }

                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create Sociable Profile {personId} {ex.StackTrace}");
                }
            }
        }


        public static async Task<int> CreatePersonAsync(string personId, string name, string email, string entityId)
        {
            string baseUrl = "https://bishops.usccb.org";

            using (var client = new HttpClient())
            {
                var uri = "entity/user?_format=json";
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = "administrator";
                var password = "dK994owatSvH43MwrJte";

                StringBuilder sb = new StringBuilder();

                sb.Append("{\n");
                sb.Append("\"name\":[{\"value\":");
                sb.Append($"\"{name}\"}}],\n");
                sb.Append("\"pass\":[{\"value\":");
                sb.Append($"\"{name}@{personId}\"}}],\n");
                sb.Append("\"mail\":[{\"value\":");
                sb.Append($"\"{email}\"}}],\n");
                sb.Append("\"status\":[{\"value\":");
                sb.Append("\"1\"}],\n");
                sb.Append("\"field_entity_id\":[{\"value\":");
                sb.Append($"\"{entityId}\"}}],\n");
                sb.Append("\"roles\":[{\"target_id\":");
                sb.Append("\"authenticated\"}],\n");
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + baseUrl + "/rest/type/user/user\"}}\n}");

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
                        return userid;
                    }
                    else
                    {

                        return -1;
                    }
                }
                catch (Exception ex)
                {
return -1;
}
}
}
public static bool UpdatePersonProfile(int profileId, string prefix, string firstName, string lastName, string middleName, string casualName, string suffix, string title, string dob, string entityId, string organization, string phoneNumber, string emailId, string address1, string address2, string city, string state, string zip, string preferredContact)
{
        string baseUrl = "https://bishops.usccb.org";
        using (var client = new HttpClient())
        {
                var uri = $"profile/{profileId}?_format=json";
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = "administrator";
                var password = "dK994owatSvH43MwrJte";
                StringBuilder sb = new StringBuilder();
                sb.Append("{\n");
                sb.Append("\"status\":[{\"value\":");
                sb.Append("\"1\"}],\n");
                sb.Append("\"field_profile_first_name\":[{\"value\":");
                sb.Append($"\"{firstName}\"}}],\n");
                sb.Append("\"field_profile_last_name\":[{\"value\":");
                sb.Append($"\"{lastName}\"}}],\n");
                sb.Append("\"field_title\":[{\"value\":");
                sb.Append($"\"{title}\"}}],\n");
                sb.Append("\"field_preferred_contact\":[{\"value\":");
                sb.Append($"\"{preferredContact}\"}}],\n");
                //sb.Append("\"field_profile_function\":[{\"value\":");
                //sb.Append($"\"{person.Function}\"}}],\n");
                sb.Append("\"field_profile_organization\":[{\"value\":");
                sb.Append($"\"{organization}\"}}],\n");
                sb.Append("\"field_profile_phone_number\":[{\"value\":");
                sb.Append($"\"{phoneNumber}\"}}],\n");
                sb.Append("\"field_birthdate\":[{\"value\":");
                sb.Append($"\"{dob}\"}}],\n");
                sb.Append("\"field_prefix\":[{\"value\":");
                sb.Append($"\"{prefix}\"}}],\n");
                sb.Append("\"field_suffix\":[{\"value\":");
                sb.Append($"\"{suffix}\"}}],\n");
                sb.Append("\"field_preferred_name\":[{\"value\":");
                sb.Append($"\"{casualName}\"}}],\n");
                sb.Append("\"field_middle_name\":[{\"value\":");
                sb.Append($"\"{middleName}\"}}],\n");
                sb.Append("\"field_profile_address\":[{");
                sb.Append($"\"locality\":\"{city}\",");
                sb.Append($"\"administrative_area\":\"{state}\",");
                sb.Append($"\"postal_code\":\"{zip}\",");
                sb.Append($"\"country_code\":\"US\",");
                sb.Append($"\"address_line1\":\"{address1} {address2}\"");
                sb.Append("}],");
                sb.Append("\"_links\":{\"type\":{\"href\":\"" + baseUrl + "/rest/type/profile/profile\"}}\n}");

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

                    if ((int)response.StatusCode == 200)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static async Task<string> GetUserByIdAsync(int userId)
        {
            string baseUrl = "https://bishops.usccb.org";
            using (var client = new HttpClient())
            {
                var uri = $"user/{userId}?_format=json";
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //Set Basic Auth
                var user = "administrator";
                var password = "dK994owatSvH43MwrJte";

                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

                // Construct an HttpContent from a StringContent
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // synchronous request without the need for .ContinueWith() or await
                var response = client.GetAsync(uri).Result;

                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }

    }
}
