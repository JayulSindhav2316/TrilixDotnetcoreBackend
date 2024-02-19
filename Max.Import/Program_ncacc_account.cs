using Max.Core;
using Max.Data.DataModel;
using MySqlConnector;
using System;
using System.Globalization;
using Twilio.TwiML.Voice;

namespace Max.Import
{
    class Program_ncacc_account
    {

        static void Main_ncacc_account(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=ncacc-demo;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=ncacc-demo;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM import_accounts;", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string accountId = reader.GetValue(0).ToString();
                string comapnyName = reader.GetValue(1).ToString();
                string primaryEmail = reader.GetValue(4).ToString();
                string primaryPhone = reader.GetValue(6).ToString();
                string otherPhone = reader.GetValue(5).ToString();
                string webSite = reader.GetValue(15).ToString();
                string address1 = reader.GetValue(21).ToString();
                string address2 = reader.GetValue(22).ToString();
                string city = reader.GetValue(23).ToString();
                string state = reader.GetValue(24).ToString();
                string zip = reader.GetValue(25).ToString();
                string addressType = reader.GetValue(26).ToString();

                //Custom fields

                string custom_Underwriter = reader.GetValue(2).ToString();
                //string custom_OutReachAssociate = reader.GetValue(2).ToString();
                string custom_AccountType = reader.GetValue(7).ToString();
                string custom_DistrictNumber = reader.GetValue(8).ToString();
                string custom_Commisioners = reader.GetValue(9).ToString();
                string custom_ElectionMethod = reader.GetValue(10).ToString();
                string custom_NC_House_District = reader.GetValue(11).ToString();
                string custom_NC_State_District = reader.GetValue(12).ToString();
                string custom_TermsOfOffice = reader.GetValue(13).ToString();
                string custom_UsCongressDistrict = reader.GetValue(14).ToString();
                string custom_LP_PoolMembers = reader.GetValue(16).ToString();
                string custom_WC_PoolMembers = reader.GetValue(17).ToString();
                string custom_CountySeat = reader.GetValue(18).ToString();
                string custom_Population = reader.GetValue(19).ToString();
                string custom_Cog = reader.GetValue(20).ToString();
                bool hasAddress1 = true;
                if (string.IsNullOrEmpty(address1))
                {
                    hasAddress1 = false;
                }

                if (comapnyName.Length == 0)
                {
                    continue;
                }

                //clean up data
                //If no Zip Code provided for an Address, please default address Zip to “00000”
                if(zip == "")
                {
                    zip = "00000";
                }

                //We need minimum one address
                /*
                 if there is not at least 1 Address provided, please default address to
                    Mark as Primary
                    Address Type = “Other”
                    Address = “0”
                    Country = “United States”
                    City = “0”
                    State = “North Carolina”
                    Zip = “0”
                 */
                
                if(addressType=="")
                {
                    addressType = "Other";
                }
                if(address1=="" && address2 == "")
                {
                    address1 = "0";
                    hasAddress1 = true;
                }
                if (city == "" )
                {
                    city = "0";
                }
                if (state == "")
                {
                    state = "NC";
                }
                if (zip == "")
                {
                    zip = "0";
                }

                /*
                  If there is not at least 1 Phone Numbers provided, please default phone number to
                    Phone Type = “Other”
                    Phone Number = “(000) 000-0000”
                    Mark as Primary
                */
                var phoneType = "Work";
                if(primaryPhone=="" && otherPhone=="")
                {
                    primaryPhone = "0000000000";
                    phoneType = "Other";
                }

                /*
                If there is not at least 1 Email Addresses provided, please default email to
                    Email Type = “Other”
                    Phone Number = “0@0.0”
                    Mark as Primary
                */
                var emailType = "Work";
                if (primaryEmail =="")
                {
                    emailType = "Other";
                    primaryEmail = $"{accountId}@0.0";
                }

                Console.WriteLine($"Creating Account {comapnyName}||{primaryEmail}|{primaryPhone}|{otherPhone}|{webSite}|{address1}|{address2}|{city}|{address1}|{address2}|{city}|{state}|{zip} {addressType}");

                //Insert Entity for Company

                string entityCompanySql = $"INSERT INTO entity (Name,OrganizationId) VALUES ('{comapnyName.Replace("'", "''")}',1)";
                MySqlCommand cmdInsertEntity = new MySqlCommand(entityCompanySql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long comapnyEntityId = cmdInsertEntity.LastInsertedId;

                //Insert  Company

                string companySql = $"INSERT INTO company(CompanyName,EntityId,WebSite) VALUES ('{comapnyName}','{comapnyEntityId}','{webSite}')";
                MySqlCommand cmdInsertCompany = new MySqlCommand(companySql, targetConnection);
                cmdInsertCompany.ExecuteNonQuery();
                long companyId = cmdInsertCompany.LastInsertedId;

                //Update company->Entity relation

                entityCompanySql = $"Update entity set companyId = {companyId} where entityId = {comapnyEntityId}";
                MySqlCommand cmdUpdateEntity = new MySqlCommand(entityCompanySql, targetConnection);
                cmdUpdateEntity.ExecuteNonQuery();

                //Update CompanyAccountId with ENtity

                var entityAccountSql = $"Update import_accountimages set entityId = {comapnyEntityId} where `Account Id` = '{accountId}'";
                MySqlCommand cmdUpdateAccountEntity = new MySqlCommand(entityAccountSql, targetConnection);
                cmdUpdateAccountEntity.ExecuteNonQuery();

                //Insert Address

                var stateCode = state;
                var stateName = "North Carolina";

                if (stateCode != null)
                {
                    stateCode = state.Trim();
                    string stateUpdateSql = $"select Name from States where ShortName = '{stateCode.Replace("'", "''")}'";
                    MySqlCommand contactRoleCommand = new MySqlCommand(stateUpdateSql, targetConnection);
                    using var documentReader = contactRoleCommand.ExecuteReader();
                    while (documentReader.Read())
                    {
                        stateName = documentReader.GetValue(0).ToString();
                    }
                    documentReader.Close();
                }


                if (hasAddress1)
                {
                    string addressSql = $"INSERT INTO address(AddressType,Address1,Address2,City,State,StateCOde,Zip,Country,CountryCode,CompanyId,IsPrimary) VALUES ('{addressType}','{address1.Replace("'", "''")}','{address2.Replace("'", "''")}','{city.Replace("'", "''")}','{stateName}','{stateCode}','{zip.Replace(" ", "")}','United States','US',{companyId},1)";
                    MySqlCommand cmdInsertAddress = new MySqlCommand(addressSql, targetConnection);
                    cmdInsertAddress.ExecuteNonQuery();
                }
                else
                {
                    string addressSql = $"INSERT INTO address(AddressType,Address1,City,State,StateCOde,Zip,Country,CountryCode,CompanyId,IsPrimary) VALUES ('{addressType}','{address2.Replace("'", "''")}','{city.Replace("'", "''")}','{stateName}','{stateCode}','{zip.Replace(" ", "")}','United States','US',{companyId},1)";
                    MySqlCommand cmdInsertAddress = new MySqlCommand(addressSql, targetConnection);
                    cmdInsertAddress.ExecuteNonQuery();
                }


                //Insert EMail
                //Bad data!!!
                //checke if email address exists
                if (primaryEmail != null)
                {
                    string emailSql = $"INSERT INTO email (EmailAddressType,EmailAddress,CompanyId,IsPrimary) VALUES ('{emailType}','{primaryEmail.Replace("'", "''")}',{companyId},1)";

                    if (primaryEmail.Length > 0)
                    {
                        MySqlCommand cmdInsertEmail = new MySqlCommand(emailSql, targetConnection);
                        cmdInsertEmail.ExecuteNonQuery();

                    }
                }

                //Insert Phone
                string phoneSql = $"INSERT INTO phone(PhoneType, PhoneNumber, CompanyId, IsPrimary) VALUES ('{phoneType}','{primaryPhone.GetCleanPhoneNumber()}',{companyId},1)";
                MySqlCommand cmdInsertPhone = new MySqlCommand(phoneSql, targetConnection);
                cmdInsertPhone.ExecuteNonQuery();

                if (otherPhone.Length > 0)
                {
                    //Insert Phone
                    string faxSQL = $"INSERT INTO phone(PhoneType, PhoneNumber, CompanyId, IsPrimary) VALUES ('Other','{otherPhone.GetCleanPhoneNumber()}',{companyId},0)";
                    cmdInsertPhone = new MySqlCommand(faxSQL, targetConnection);
                    cmdInsertPhone.ExecuteNonQuery();
                }

                //Insert Custom fields

                string underWriterSql = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (100,'\"{custom_Underwriter.Replace("'", "''")}\"',{comapnyEntityId})";
                MySqlCommand cmdUnderWriterSql = new MySqlCommand(underWriterSql, targetConnection);
                cmdUnderWriterSql.ExecuteNonQuery();

                string accountTypeSql = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (102,'[\"{custom_AccountType.Replace("'", "''")}\"]',{comapnyEntityId})";
                MySqlCommand cmdAccountTypeSql = new MySqlCommand(accountTypeSql, targetConnection);
                cmdAccountTypeSql.ExecuteNonQuery();

                string districtNumberSql = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (103,'\"{custom_DistrictNumber}\"',{comapnyEntityId})";
                MySqlCommand cmdDistrictNumberSql = new MySqlCommand(districtNumberSql, targetConnection);
                cmdDistrictNumberSql.ExecuteNonQuery();

                string commissionersSql = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (104,'\"{custom_Commisioners}\"',{comapnyEntityId})";
                MySqlCommand cmdCommissionersSql = new MySqlCommand(commissionersSql, targetConnection);
                cmdCommissionersSql.ExecuteNonQuery();

                string electionMethodSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (105,'\"{custom_ElectionMethod}\"',{comapnyEntityId})";
                MySqlCommand cmdelectionMethodSQL = new MySqlCommand(electionMethodSQL, targetConnection);
                cmdelectionMethodSQL.ExecuteNonQuery();

                var customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (106,'\"{custom_TermsOfOffice}\"',{comapnyEntityId})";
                MySqlCommand cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                var congressDistrict = string.Empty;
                if (custom_UsCongressDistrict != "")
                {
                    var array = custom_UsCongressDistrict.Split(',');
                    foreach (var item in array)
                    {
                        if (congressDistrict == string.Empty)
                        {
                            congressDistrict = $"\"{item.Trim()}\"";
                        }
                        else
                        {
                            congressDistrict = congressDistrict + $",\"{item.Trim()}\"";
                        }
                    }
                    customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (107,'[{congressDistrict}]',{comapnyEntityId})";
                    cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                    cmdCustomFieldSqlSQL.ExecuteNonQuery();
                }



                string poolMembers = string.Empty;

                if (custom_LP_PoolMembers == "T" && custom_WC_PoolMembers == "T")
                {
                    poolMembers = $"[\"LP\",\"WC\"]";
                }
                else if (custom_LP_PoolMembers == "F" && custom_WC_PoolMembers == "T")
                {
                    poolMembers = $"[\"WC\"]";
                }
                else if (custom_LP_PoolMembers == "T" && custom_WC_PoolMembers == "F")
                {
                    poolMembers = $"[\"LP\"]";
                }
                if (poolMembers != string.Empty)
                {
                    customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (108,'{poolMembers}',{comapnyEntityId})";
                    cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                    cmdCustomFieldSqlSQL.ExecuteNonQuery();
                }

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (109,'\"{custom_CountySeat}\"',{comapnyEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (110,'\"{custom_Population}\"',{comapnyEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (111,'\"{custom_Cog}\"',{comapnyEntityId})";
                cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                cmdCustomFieldSqlSQL.ExecuteNonQuery();

                var NcHouseDistrict = string.Empty;
                if (custom_NC_House_District != "")
                {
                    var array = custom_NC_House_District.Split(',');
                    foreach (var item in array)
                    {
                        if (NcHouseDistrict == string.Empty)
                        {
                            NcHouseDistrict = $"\"{item.Trim()}\"";
                        }
                        else
                        {
                            NcHouseDistrict = NcHouseDistrict + $",\"{item.Trim()}\"";
                        }
                    }
                    customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (112,'[{NcHouseDistrict}]',{comapnyEntityId})";
                    cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                    cmdCustomFieldSqlSQL.ExecuteNonQuery();
                }

                var NcStateDistrict = string.Empty;
                if (custom_NC_State_District != "")
                {
                    var array = custom_NC_State_District.Split(',');
                    foreach (var item in array)
                    {
                        if (NcStateDistrict == string.Empty)
                        {
                            NcStateDistrict = $"\"{item.Trim()}\"";
                        }
                        else
                        {
                            NcStateDistrict = NcStateDistrict + $",\"{item.Trim()}\"";
                        }
                    }
                    customFieldSQL = $"INSERT INTO customfielddata(CustomFieldId, Value, EntityId) VALUES (113,'[{NcStateDistrict}]',{comapnyEntityId})";
                    cmdCustomFieldSqlSQL = new MySqlCommand(customFieldSQL, targetConnection);
                    cmdCustomFieldSqlSQL.ExecuteNonQuery();
                }
            }
        }

    }
}
