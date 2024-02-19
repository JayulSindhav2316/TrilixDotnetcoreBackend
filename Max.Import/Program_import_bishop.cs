using Max.Core;
using MySqlConnector;
using System;
using System.Globalization;

namespace Max.Import
{
    class Program_import_bishop
    {

        static void Main_import_bishop(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=trilix_bishop;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=trilix_bishop;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM import_bishops;", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string comapnyName = reader.GetValue(0).ToString();
                string person_id = reader.GetValue(1).ToString();
                string prefix = reader.GetValue(6).ToString();
                string last_name = reader.GetValue(2).ToString();
                string first_name = reader.GetValue(3).ToString();
                string mid_name = reader.GetValue(4).ToString();
                string suffix = reader.GetValue(5).ToString();
                string dob = reader.GetValue(16).ToString();
                string primary_phone = string.Empty;
                string phone_1 = reader.GetValue(7).ToString();
                string extention = reader.GetValue(8).ToString();
                string phone_2 = reader.GetValue(9).ToString();
                string email = reader.GetValue(17).ToString();
                string address1 = reader.GetValue(10).ToString();
                string address2 = reader.GetValue(11).ToString();
                string home_city = reader.GetValue(12).ToString();
                string home_state = reader.GetValue(13).ToString();
                string home_zip = reader.GetValue(14).ToString();
                string Title = reader.GetValue(15).ToString();
                string billing_street_address = string.Empty;
                string billing_city = string.Empty;
                string billing_state = string.Empty;
                string billing_zip = string.Empty;
                string fee_amount = string.Empty;
                string note_1 = string.Empty;
                string note_2 = string.Empty;
                string note_3 = string.Empty;
                string gender = "Male";
               
                if(extention.Length > 0)
                {
                    phone_1 = $"{phone_1}x{extention}";
                }

                string currentDate = "2022-03-01";
                Console.WriteLine($"Creating Person {person_id}|{last_name}||{suffix}|{first_name}|{mid_name}|{prefix}|{dob}|{primary_phone}|{email}|{address1}|{address2}|{home_city}|{home_state}|{home_zip}|{billing_street_address}|{billing_city}|{billing_state}|{billing_zip}");
                string dbo_mysql = string.Empty;
                string birthdate = string.Empty;
                try
                {
                    DateTime birthDate = DateTime.Parse(dob);
                    birthdate = birthDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    birthdate = "1000-01-01";
                }

                string startDate = "2021-01-01";
                string nextDueDate = "2022-01-01";
                string endDate = "2021-12-31";
              

               
                string paymentMethod = string.Empty;

               
                //Insert Entity for Person

                string entityPersonSql = $"INSERT INTO entity (EntityId,Name,OrganizationId,WebLoginName,WebPassword) VALUES ({person_id},'{prefix} {first_name.Replace("'", "''") } {last_name.Replace("'", "''") }' ,1,'{email.Replace("'", "''")}','Password')";
                MySqlCommand cmdInsertEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long personEntityId = cmdInsertEntity.LastInsertedId;

                //Insert  Person

                string personSql = $"INSERT INTO person(PersonId,Prefix,FirstName,LastName,MiddleName,CasualName,Suffix,Gender,Salutation,DateOfBirth,OrganizationId,Status,EntityId,Title) VALUES ({person_id},'{prefix}','{first_name.Replace("'", "''")}','{last_name.Replace("'", "''")}','{mid_name.Replace("'", "''")}','','{suffix}','{gender}','','{birthdate}',1,1,{personEntityId},'{Title}')";
                MySqlCommand cmdInsertPerson = new MySqlCommand(personSql, targetConnection);
                cmdInsertPerson.ExecuteNonQuery();
                long personId = cmdInsertPerson.LastInsertedId;

                //Update  Person->Entity relation

                entityPersonSql = $"Update entity set PersonId = {person_id} where entityId = {personEntityId}";
                MySqlCommand cmdUpdateEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdUpdateEntity.ExecuteNonQuery();

                //Insert Address
                string addressSql = $"INSERT INTO address(AddressType,Address1,Address2,City,State,Zip,Country,PersonId,IsPrimary) VALUES ('Home','{address1.Replace("'", "''")}','{address2.Replace("'", "''")}','{home_city.Replace("'", "''")}','{home_state}','{home_zip.Replace(" ", "")}','USA',{personId},1)";
                MySqlCommand cmdInsertAddress = new MySqlCommand(addressSql, targetConnection);
                cmdInsertAddress.ExecuteNonQuery();

                //Insert Billing Address
                if(billing_street_address.Length > 0)
                {
                    string billingAddressSql = $"INSERT INTO address(AddressType,Address1,City,State,Zip,Country,PersonId,IsPrimary) VALUES ('Billing','{billing_street_address.Replace("'", "''")}','{billing_city.Replace("'", "''")}','{billing_state}','{billing_zip.Replace(" ", "")}','USA',{personId},0)";
                    MySqlCommand cmdInsertBillingAddress = new MySqlCommand(billingAddressSql, targetConnection);
                    cmdInsertBillingAddress.ExecuteNonQuery();
                }
               

                //Insert EMail
                string emailSql = $"INSERT INTO email (EmailAddressType,EmailAddress,PersonId,IsPrimary) VALUES ('Home','{email.Replace("'", "''")}',{personId},1)";
                MySqlCommand cmdInsertEmail = new MySqlCommand(emailSql, targetConnection);
                cmdInsertEmail.ExecuteNonQuery();

                //Insert Phone
                string phoneSql = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Home','{phone_1.GetCleanPhoneNumber()}',{personId},1)";
                MySqlCommand cmdInsertPhone = new MySqlCommand(phoneSql, targetConnection);
                cmdInsertPhone.ExecuteNonQuery();

                //Insert Phone
                string faxSQL = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Fax','{phone_2.GetCleanPhoneNumber()}',{personId},0)";
                cmdInsertPhone = new MySqlCommand(faxSQL, targetConnection);
                cmdInsertPhone.ExecuteNonQuery();


                //Insert Note
                if (note_1.Length > 0)
                {
                    string noteSql = $"INSERT INTO note (EntityId,Notes,Severity,DisplayOnProfile,CreatedOn,CreatedBy,Status) VALUES ({personEntityId}, '{note_1.Replace("'", "''")}','General',1,'{currentDate}','admin',1)";
                    MySqlCommand cmdInsertNote = new MySqlCommand(noteSql, targetConnection);
                    cmdInsertNote.ExecuteNonQuery();
                }
                if (note_2.Length > 0)
                {
                    string noteSql = $"INSERT INTO note (EntityId,Notes,Severity,DisplayOnProfile,CreatedOn,CreatedBy,Status) VALUES ({personEntityId}, '{note_2.Replace("'", "''")}','General',1,'{currentDate}','admin',1)";
                    MySqlCommand cmdInsertNote = new MySqlCommand(noteSql, targetConnection);
                    cmdInsertNote.ExecuteNonQuery();
                }
                if (note_3.Length > 0)
                {
                    string noteSql = $"INSERT INTO note (EntityId,Notes,Severity,DisplayOnProfile,CreatedOn,CreatedBy,Status) VALUES ({personEntityId}, '{note_3.Replace("'", "''")}','General',1,'{currentDate}','admin',1)";
                    MySqlCommand cmdInsertNote = new MySqlCommand(noteSql, targetConnection);
                    cmdInsertNote.ExecuteNonQuery();
                }

                //Insert Entity for Company

                string entitySql = $"INSERT INTO entity (Name,OrganizationId) VALUES ('{comapnyName.Replace("'", "''")}',1)";
                cmdInsertEntity = new MySqlCommand(entitySql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long companyEntityId = cmdInsertEntity.LastInsertedId;

                //Insert Company

                string companySql = $"INSERT INTO company(CompanyName,EntityId) VALUES ('{comapnyName.Replace("'", "''")}',{companyEntityId})";
                MySqlCommand cmdInsertComapny = new MySqlCommand(companySql, targetConnection);
                cmdInsertComapny.ExecuteNonQuery();
                long companyId = cmdInsertComapny.LastInsertedId;

                //Set companyid to the current Person

                entityPersonSql = $"Update Person set CompanyId = {companyId} where PersonId = {personId}";
                MySqlCommand cmdUpdatePerson = new MySqlCommand(entityPersonSql, targetConnection);
                cmdUpdatePerson.ExecuteNonQuery();

                //Update Company->Entity relation

                string entityComapnySql = $"Update entity set CompanyId = {companyId} where entityId = {companyEntityId}";
                cmdUpdateEntity = new MySqlCommand(entityComapnySql, targetConnection);
                cmdUpdateEntity.ExecuteNonQuery();


                //Set Company to Person Relation

                string companyPersonSql = $"INSERT INTO relation (EntityId,RelatedEntityId,RelationshipId,StartDate,Status) VALUES ({companyEntityId},{personEntityId},29,'{Constants.MySQL_MinDate.ToShortDateString()}',1)";
                MySqlCommand cmdInsertRelation = new MySqlCommand(companyPersonSql, targetConnection);
                cmdInsertRelation.ExecuteNonQuery();
            }
        }

    }
}
