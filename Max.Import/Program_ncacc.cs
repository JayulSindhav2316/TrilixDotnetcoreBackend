using Azure.Storage.Blobs.Models;
using Max.Core;
using Max.Data.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using System;
using System.Collections;
using System.Globalization;
using Twilio.TwiML.Voice;

namespace Max.Import
{
    class Program_ncacc
    {

        static void Main_ncacc(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=ncacc-demo;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=ncacc-demo;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM import_contacts;", sourceConnection);
            using var reader = command.ExecuteReader();

            Hashtable contactTable = new Hashtable();

            while (reader.Read())
            {
                string contactId = reader.GetValue(0).ToString();
                string comapnyName = reader.GetValue(2).ToString();
                string contactRole = reader.GetValue(3).ToString();
                string title = reader.GetValue(9).ToString();
                string prefix = reader.GetValue(8).ToString();
                string last_name = reader.GetValue(5).ToString();
                string first_name = reader.GetValue(6).ToString();
                string mid_name = reader.GetValue(7).ToString();
                string suffix = reader.GetValue(10).ToString();
                string dob = reader.GetValue(24).ToString();
                string primary_phone = string.Empty;
                string workPhone = reader.GetValue(14).ToString();
                string workPhoneExtention = reader.GetValue(15).ToString();
                string mobilePhone = reader.GetValue(16).ToString();
                string homePhone = reader.GetValue(17).ToString();
                string otherPhone = reader.GetValue(18).ToString();
                string fax = reader.GetValue(19).ToString();
                string email = reader.GetValue(20).ToString();
                string secondaryEmail = reader.GetValue(21).ToString();
                //string email_3 = reader.GetValue(23).ToString();
                string preferredContact = reader.GetValue(23).ToString();
                string address1 = reader.GetValue(29).ToString();
                string address2 = reader.GetValue(30).ToString();
                //string address3 = reader.GetValue(35).ToString();
                //string address4 = reader.GetValue(36).ToString();
                string city = reader.GetValue(31).ToString();
                string addressType = reader.GetValue(32).ToString();
                string home_zip = reader.GetValue(33).ToString();
                string home_state = reader.GetValue(34).ToString();
                string gender = reader.GetValue(11).ToString();
                string effectiveDate = reader.GetValue(35).ToString();
                //Custom fields
                string custom_Race = reader.GetValue(12).ToString();
                string custom_PartyAffiliation = reader.GetValue(13).ToString();
                string custom_ContactType = reader.GetValue(22).ToString(); 
                string custom_Children = reader.GetValue(25).ToString();
                string custom_Alumunai = reader.GetValue(26).ToString();
                string custom_SpouseBirthDate = reader.GetValue(27).ToString();
                string custom_StateBarNumber = reader.GetValue(28).ToString();
                if (workPhoneExtention.Length > 0)
                {
                    workPhone = $"{workPhone}x{workPhoneExtention}";
                }
                if(preferredContact.Length == 0)
                {
                    preferredContact = "1";
                }
                else
                {
                    if (preferredContact.Contains("Email"))
                    {
                        preferredContact = "1";
                    }
                    else if (preferredContact.Contains("Phone"))
                    {
                        preferredContact = "2";
                    }
                    else 
                    {
                        preferredContact = "3";
                    }
                }
                if (addressType == "")
                {
                    addressType = "Other";
                }
                if (address1 == "" && address2 == "")
                {
                    address1 = "0";
                }
                if (city == "")
                {
                    city = "0";
                }
                if (home_state == "")
                {
                    home_state = "NC";
                }
                if (home_zip == "")
                {
                    home_zip = "00000";
                }
                var emailType = "Work";
                if (email =="" & secondaryEmail =="")
                {
                    email = $"{contactId}@0.0";
                    emailType = "Other";
                }

               
                string currentDate = "1900-01-01";
                Console.WriteLine($"Creating Person {last_name}||{suffix}|{first_name}|{mid_name}|{prefix}|{dob}|{primary_phone}|{email}|{address1}|{address2}|{city}|{home_state}|{home_zip}");
                string dbo_mysql = string.Empty;
                string birthdate = string.Empty;
                try
                {
                    DateTime birthDate = DateTime.Parse(dob);
                    birthdate = birthDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    birthdate = null;
                }

                if(title.Length > 150)
                {
                    title = title.Substring(0, 150);
                }

                //Role start Date
                string roleEffectiveDate = string.Empty;
                try
                {
                    DateTime birthDate = DateTime.Parse(effectiveDate);
                    roleEffectiveDate = birthDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    roleEffectiveDate = "1900-01-01";
                }

                var contactPersonId = string.Empty;
                //Check if Contact Account exists

                if(contactTable.Contains(contactId))
                {
                    contactPersonId = (string)contactTable[contactId];
                }
                

                if (contactPersonId != string.Empty)
                {
                    //Insert address 
                    var secondStateCode = home_state;
                    var secondStateName = "North Carolina";

                    if (secondStateCode != null)
                    {
                        secondStateCode = home_state.Trim();
                        string stateUpdateSql = $"select Name from States where ShortName = '{secondStateCode.Replace("'", "''")}'";
                        MySqlCommand contactRoleCommand = new MySqlCommand(stateUpdateSql, targetConnection);
                        using var stateReader = contactRoleCommand.ExecuteReader();
                        while (stateReader.Read())
                        {
                            secondStateName = stateReader.GetValue(0).ToString();
                        }
                        stateReader.Close();
                    }
                    //Insert Address
                    string insertAddressSql = $"INSERT INTO address(AddressType,Address1,Address2,City,State,StateCode,Zip,Country,CountryCode,PersonId,IsPrimary) VALUES ('{addressType}','{address1.Replace("'", "''")}','{address2.Replace("'", "''")}','{city.Replace("'", "''")}','{secondStateName}','{secondStateCode}','{home_zip.Replace(" ", "")}','United States','US',{contactPersonId},0)";
                    MySqlCommand cmdInsertDuplicateAddress = new MySqlCommand(insertAddressSql, targetConnection);
                    cmdInsertDuplicateAddress.ExecuteNonQuery();

                    continue;
                }

                //Insert Entity for Person

                string entityPersonSql = $"INSERT INTO entity (Name,OrganizationId,WebLoginName,WebPassword) VALUES ('{prefix} {first_name.Replace("'", "''")} {last_name.Replace("'", "''")}' ,1,'{email.Replace("'", "''")}','Password')";
                MySqlCommand cmdInsertEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long personEntityId = cmdInsertEntity.LastInsertedId;

                //Insert  Person
                long personId = 0L;
                if (birthdate != null)
                {
                    string personSql = $"INSERT INTO person(Prefix,FirstName,LastName,MiddleName,CasualName,Suffix,Gender,Salutation,DateOfBirth,OrganizationId,Status,EntityId,Title,PreferredContact,Designation) VALUES ('{prefix}','{first_name.Replace("'", "''")}','{last_name.Replace("'", "''")}','{mid_name.Replace("'", "''")}','','{suffix}','{gender}','','{birthdate}',1,1,{personEntityId},'{title.Replace("'", "''")}','{preferredContact}','Unassigned')";
                    MySqlCommand cmdInsertPerson = new MySqlCommand(personSql, targetConnection);
                    cmdInsertPerson.ExecuteNonQuery();
                    personId = cmdInsertPerson.LastInsertedId;
                }
                else
                {
                    string personSql = $"INSERT INTO person(Prefix,FirstName,LastName,MiddleName,CasualName,Suffix,Gender,Salutation,OrganizationId,Status,EntityId,Title,PreferredContact,Designation) VALUES ('{prefix}','{first_name.Replace("'", "''")}','{last_name.Replace("'", "''")}','{mid_name.Replace("'", "''")}','','{suffix}','{gender}','',1,1,{personEntityId},'{title.Replace("'", "''")}','{preferredContact}','Unassigned')";
                    MySqlCommand cmdInsertPerson = new MySqlCommand(personSql, targetConnection);
                    cmdInsertPerson.ExecuteNonQuery();
                    personId = cmdInsertPerson.LastInsertedId;
                }
                //Add person to contactTable

                contactTable.Add(contactId, personId.ToString());

                //Update  Person->Entity relation

                entityPersonSql = $"Update entity set PersonId = {personId} where entityId = {personEntityId}";
                MySqlCommand cmdUpdateEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdUpdateEntity.ExecuteNonQuery();

                //Update ContactId with ENtity

                var entityContactSql = $"Update import_contactimages set entityId = {personEntityId} where `Contact Id` = '{contactId}'";
                MySqlCommand cmdUpdateContactEntity = new MySqlCommand(entityContactSql, targetConnection);
                cmdUpdateContactEntity.ExecuteNonQuery();

                var stateCode = home_state;
                var stateName = "North Carolina";

                if (stateCode != null)
                {
                    stateCode = home_state.Trim();
                    string stateUpdateSql = $"select Name from States where ShortName = '{stateCode.Replace("'", "''")}'";
                    MySqlCommand contactRoleCommand = new MySqlCommand(stateUpdateSql, targetConnection);
                    using var stateReader = contactRoleCommand.ExecuteReader();
                    while (stateReader.Read())
                    {
                        stateName = stateReader.GetValue(0).ToString();
                    }
                    stateReader.Close();
                }

                //Insert Address
                string addressSql = $"INSERT INTO address(AddressType,Address1,Address2,City,State,StateCode,Zip,Country,CountryCode,PersonId,IsPrimary) VALUES ('{addressType}','{address1.Replace("'", "''")}','{address2.Replace("'", "''")}','{city.Replace("'", "''")}','{stateName}','{stateCode}','{home_zip.Replace(" ", "")}','United States','US',{personId},1)";
                MySqlCommand cmdInsertAddress = new MySqlCommand(addressSql, targetConnection);
                cmdInsertAddress.ExecuteNonQuery();


                //Insert EMail
                string emailSql = $"INSERT INTO email (EmailAddressType,EmailAddress,PersonId,IsPrimary) VALUES ('{emailType}','{email.Replace("'", "''")}',{personId},1)";
                MySqlCommand cmdInsertEmail = new MySqlCommand(emailSql, targetConnection);
                cmdInsertEmail.ExecuteNonQuery();

                if(secondaryEmail.Length > 0)
                {
                    //Insert EMail
                    string secondaryemailSql = $"INSERT INTO email (EmailAddressType,EmailAddress,PersonId,IsPrimary) VALUES ('Personal','{secondaryEmail.Replace("'", "''")}',{personId},0)";
                    cmdInsertEmail = new MySqlCommand(secondaryemailSql, targetConnection);
                    cmdInsertEmail.ExecuteNonQuery();
                }

                //Insert Phone
                int phonecount = 0;
                var isPrimary = 1;

                if (workPhone.Length > 0)
                {
                    string phoneSql = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Work','{workPhone.GetCleanPhoneNumber()}',{personId},{isPrimary})";
                    MySqlCommand cmdInsertWorkPhone = new MySqlCommand(phoneSql, targetConnection);
                    cmdInsertWorkPhone.ExecuteNonQuery();
                    isPrimary = 0;
                    phonecount++;
                }
               
                if (homePhone.Length > 0)
                {
                    //Insert Phone
                    string homePhoneSQL = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Home','{homePhone.GetCleanPhoneNumber()}',{personId},{isPrimary})";
                    MySqlCommand cmdInsertPhone = new MySqlCommand(homePhoneSQL, targetConnection);
                    cmdInsertPhone.ExecuteNonQuery();
                    isPrimary = 0;
                    phonecount++;
                }
            
                if(otherPhone.Length > 0)
                {
                    string otherPhoneSQL = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Other','{otherPhone.GetCleanPhoneNumber()}',{personId},{isPrimary})";
                    MySqlCommand cmdInsertPhone = new MySqlCommand(otherPhoneSQL, targetConnection);
                    cmdInsertPhone.ExecuteNonQuery();
                    isPrimary = 0;
                    phonecount++;
                }

                if (mobilePhone.Length > 0)
                {
                    string otherPhoneSQL = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Cell','{mobilePhone.GetCleanPhoneNumber()}',{personId},{isPrimary})";
                    MySqlCommand cmdInsertPhone = new MySqlCommand(otherPhoneSQL, targetConnection);
                    cmdInsertPhone.ExecuteNonQuery();
                    isPrimary = 0;
                    phonecount++;
                }

                if (fax.Length > 0 && phonecount<=3)
                {
                    //Insert Phone
                    string faxSQL = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Other','{fax.GetCleanPhoneNumber()}',{personId},{isPrimary})";
                    MySqlCommand cmdInsertPhone = new MySqlCommand(faxSQL, targetConnection);
                    cmdInsertPhone.ExecuteNonQuery();
                    isPrimary = 0;
                    phonecount++;
                }

                if (phonecount==0)
                {
                    //Insert Phone
                    string faxSQL = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Other','0000000000',{personId},1)";
                    MySqlCommand cmdInsertPhone = new MySqlCommand(faxSQL, targetConnection);
                    cmdInsertPhone.ExecuteNonQuery();
                }

                string contactAccountSql = $"select EntityId, CompanyId from entity where Name = '{comapnyName}'";
                MySqlCommand contactAccountCommand = new MySqlCommand(contactAccountSql, targetConnection);
                var companyEntityId = string.Empty;
                var companyId = string.Empty;
                using var companyReader = contactAccountCommand.ExecuteReader();
                while (companyReader.Read())
                {
                    companyEntityId = companyReader.GetValue(0).ToString();
                    companyId= companyReader.GetValue(1).ToString();
                }
                companyReader.Close();

                //Set companyid to the current Person

                if(companyEntityId.Length >0)
                {
                    entityPersonSql = $"Update Person set CompanyId = {companyId} where PersonId = {personId}";
                    MySqlCommand cmdUpdatePerson = new MySqlCommand(entityPersonSql, targetConnection);
                    cmdUpdatePerson.ExecuteNonQuery();

                    //Set entityRole
                    // Find account for the current person
                    var contactRoles = contactRole.Split(",");
                    if(contactRoles.Length > 0)
                    {
                        foreach(var role in contactRoles)
                        {
                            var roleName = role.Trim();
                            string contactRoleSql = $"select ContactRoleId from contactRole where Name = '{roleName.Replace("'", "''")}'";
                            MySqlCommand contactRoleCommand = new MySqlCommand(contactRoleSql, targetConnection);
                            var contactRoleId = string.Empty;
                            using var roleReader = contactRoleCommand.ExecuteReader();
                            while (roleReader.Read())
                            {
                                contactRoleId = roleReader.GetValue(0).ToString();
                            }
                            roleReader.Close();

                            if (contactRoleId.Length > 0)
                            {
                                string entityRoleSQL = $"INSERT INTO entityRole(EntityId, ContactRoleId, Status,CompanyId,EffectiveDate) VALUES ({personEntityId},{contactRoleId},1,{companyEntityId},'{roleEffectiveDate}')";
                                MySqlCommand cmdInsertRole = new MySqlCommand(entityRoleSQL, targetConnection);
                                cmdInsertRole.ExecuteNonQuery();

                                //Add Assign role acitivity
                                if (effectiveDate !="")
                                {
                                    var dateParts = effectiveDate.Split("/");
                                    currentDate = $"{dateParts[2]}-{dateParts[0]}-{dateParts[1]}";
                                }
                                var historyDescription = $"Assigned {first_name.Replace("'", "''")} {last_name.Replace("'", "''")} to {roleName.Replace("'", "''")} role at {comapnyName.Replace("'", "''")}.";

                                var insertHistorySql = $"INSERT INTO entityrolehistory (EntityId,ContactRoleId,CompanyId,ActivityType,ActivityDate,Description,Status)VALUES ({personEntityId},{contactRoleId},{companyEntityId},'Created','{currentDate}','{historyDescription}',1 )";
                                MySqlCommand cmdInsertHistory = new MySqlCommand(insertHistorySql, targetConnection);
                                cmdInsertHistory.ExecuteNonQuery();
                                
                                //Add contact Activity for role assigment

                                var subject = $"Role Change for {first_name.Replace("'", "''")} {last_name.Replace("'", "''")}";
                                var description = $"{first_name.Replace("'", "''")} {last_name.Replace("'", "''")} assigned to {roleName.Replace("'", "''")} role at {comapnyName.Replace("'", "''")}";

                                var insertActivitySql = $"INSERT INTO contactactivity (EntityId,AccountId,ActivityDate,InteractionType,ActivityConnection,Subject,Description,Status,ContactRoleId) VALUES({personEntityId},{companyEntityId},'{currentDate}',4,2,'{subject}','{description}',1,{contactRoleId})";
                                MySqlCommand cmdInsertActivity = new MySqlCommand(insertActivitySql, targetConnection);
                                cmdInsertActivity.ExecuteNonQuery();
                            }
                        }
                    }
                }

                var customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (115,'\"{custom_Children}\"',{personEntityId})";
                MySqlCommand cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (116,'\"{custom_Alumunai}\"',{personEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (117,'\"{custom_StateBarNumber}\"',{personEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (118,'\"{custom_SpouseBirthDate}\"',{personEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (119,'\"{custom_Race}\"',{personEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (120,'\"{custom_PartyAffiliation}\"',{personEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                string contactType = string.Empty;

                if (custom_ContactType == "T" )
                {
                    contactType = $"[\"Primary Contact\"]";
                }
                if (contactType != string.Empty)
                {
                    customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (114,'{contactType}',{personEntityId})";
                    cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                    cmdCustomFieldSqlSQL.ExecuteNonQuery();
                }

            }
        }

    }
}
