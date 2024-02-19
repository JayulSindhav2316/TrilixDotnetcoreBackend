using Max.Core;
using MySqlConnector;
using System;
using System.Globalization;

namespace Max.Import
{
    class Program_sales
    {

        static void Main_sales(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=trilix_demo;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=trilix_demo;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

           


            using var command = new MySqlCommand("SELECT * FROM demoperson;", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string comapnyName = reader.GetValue(3).ToString();
                string last_name = reader.GetValue(2).ToString();
                string first_name = reader.GetValue(0).ToString();
                string dob = reader.GetValue(15).ToString();
                string primary_phone = string.Empty;
                string phone_1 = reader.GetValue(17).ToString();
                string email = reader.GetValue(4).ToString();
                string address1 = reader.GetValue(8).ToString();
                string address2 = reader.GetValue(9).ToString();
                string address3 = reader.GetValue(10).ToString();
                string home_city = reader.GetValue(11).ToString();
                string home_state = reader.GetValue(12).ToString();
                string home_zip = reader.GetValue(13).ToString();
                string Title = reader.GetValue(23).ToString();
                string gender = reader.GetValue(16).ToString();
                string weblogin = reader.GetValue(32).ToString();
                string currentDate = "2022-03-01";
                Console.WriteLine($"Creating Person {last_name}|{first_name}|{dob}|{primary_phone}|{email}|{address1}|{address2}|{home_city}|{home_state}|{home_zip}");
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
                
                //Insert Entity for Person

                string entityPersonSql = $"INSERT INTO entity (Name,OrganizationId,WebLoginName,WebPassword) VALUES ('{first_name.Replace("'", "''") } {last_name.Replace("'", "''") }' ,1,'{weblogin.Replace("'", "''")}','Password')";
                MySqlCommand cmdInsertEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long personEntityId = cmdInsertEntity.LastInsertedId;

                //Insert  Person

                string personSql = $"INSERT INTO person(FirstName,LastName,Suffix,Gender,Salutation,DateOfBirth,OrganizationId,Status,EntityId,Title) VALUES ('{first_name.Replace("'", "''")}','{last_name.Replace("'", "''")}','','{gender}','','{birthdate}',1,1,{personEntityId},'{Title}')";
                MySqlCommand cmdInsertPerson = new MySqlCommand(personSql, targetConnection);
                cmdInsertPerson.ExecuteNonQuery();
                long personId = cmdInsertPerson.LastInsertedId;

                //Update  Person->Entity relation

                entityPersonSql = $"Update entity set PersonId = {personId} where entityId = {personEntityId}";
                MySqlCommand cmdUpdateEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdUpdateEntity.ExecuteNonQuery();

                //Insert Address
                string addressSql = $"INSERT INTO address(AddressType,Address1,Address2,City,State,Zip,Country,PersonId,IsPrimary) VALUES ('Home','{address1.Replace("'", "''")}','{address2.Replace("'", "''")}','{home_city.Replace("'", "''")}','{home_state}','{home_zip.Replace(" ", "")}','USA',{personId},1)";
                MySqlCommand cmdInsertAddress = new MySqlCommand(addressSql, targetConnection);
                cmdInsertAddress.ExecuteNonQuery();

                //Insert EMail
                string emailSql = $"INSERT INTO email (EmailAddressType,EmailAddress,PersonId,IsPrimary) VALUES ('Home','{email.Replace("'", "''")}',{personId},1)";
                MySqlCommand cmdInsertEmail = new MySqlCommand(emailSql, targetConnection);
                cmdInsertEmail.ExecuteNonQuery();

                //Insert Phone
                string phoneSql = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Home','{phone_1.GetCleanPhoneNumber()}',{personId},1)";
                MySqlCommand cmdInsertPhone = new MySqlCommand(phoneSql, targetConnection);
                cmdInsertPhone.ExecuteNonQuery();

                //Get company Record 

                using var queryConnection = new MySqlConnection(targetConnectionString);
                queryConnection.Open();

                string companySQL = $"Select EntityId,CompanyId from entity where CompanyId >0 and Name = '{comapnyName.Replace("'", "''")}';";
                MySqlCommand cmdGetCompany = new MySqlCommand(companySQL, queryConnection);
                var company = cmdGetCompany.ExecuteReader();
                if(company.HasRows)
                {
                    company.Read();

                    string entityId = company.GetValue(0).ToString();
                    string companyId = company.GetValue(1).ToString();

                    //Set companyid to the current Person

                    entityPersonSql = $"Update Person set CompanyId = {companyId} where PersonId = {personId}";
                    MySqlCommand cmdUpdatePerson = new MySqlCommand(entityPersonSql, targetConnection);
                    cmdUpdatePerson.ExecuteNonQuery();

                    //Set Company to Person Relation

                    string companyPersonSql = $"INSERT INTO relation (EntityId,RelatedEntityId,RelationshipId,StartDate,Status) VALUES ({entityId},{personEntityId},29,'{Constants.MySQL_MinDate.ToShortDateString()}',1)";
                    MySqlCommand cmdInsertRelation = new MySqlCommand(companyPersonSql, targetConnection);
                    cmdInsertRelation.ExecuteNonQuery();
                }
                queryConnection.Close();

            }
        }

    }
}
