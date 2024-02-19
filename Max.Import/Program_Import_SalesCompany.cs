using Max.Core;
using MySqlConnector;
using System;
using System.Globalization;

namespace Max.Import
{
    class Program_Company
    {

        static void Main_Comapny(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=trilix_demo;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=trilix_demo;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM democompany;", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string comapnyName = reader.GetValue(0).ToString();
                string email = reader.GetValue(17).ToString();
                string primary_phone = reader.GetValue(20).ToString();
                string address1 = reader.GetValue(13).ToString();
                string address2 = reader.GetValue(14).ToString();
                string home_city = reader.GetValue(16).ToString();
                string home_state = reader.GetValue(17).ToString();
                string home_zip = reader.GetValue(18).ToString();
               


                string paymentMethod = string.Empty;

                //Insert Entity for Company

                string entitySql = $"INSERT INTO entity (Name,OrganizationId) VALUES ('{comapnyName.Replace("'", "''")}',1)";
                var cmdInsertEntity = new MySqlCommand(entitySql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long companyEntityId = cmdInsertEntity.LastInsertedId;

                //Insert Company

                string companySql = $"INSERT INTO company(CompanyName,EntityId,streetAddress,City,State,Zip,Email,Phone) VALUES ('{comapnyName.Replace("'", "''")}',{companyEntityId},'{address1} {address2}','{home_city}','{home_state}','{home_zip}','{email}','{primary_phone.GetCleanPhoneNumber()}' )";
                MySqlCommand cmdInsertComapny = new MySqlCommand(companySql, targetConnection);
                cmdInsertComapny.ExecuteNonQuery();
                long companyId = cmdInsertComapny.LastInsertedId;

                //Update Entity

                string companyEntitySql = $"Update Entity set CompanyId = {companyId} Where EntityId = {companyEntityId};";
                MySqlCommand cmdUpdateComapnyEntity = new MySqlCommand(companyEntitySql, targetConnection);
                cmdUpdateComapnyEntity.ExecuteNonQuery();

                string EntityCompanySql = $"Update Company set EntityId = {companyEntityId} Where CompanyId = {companyId};";
                MySqlCommand cmdUpdateEntityCompany = new MySqlCommand(EntityCompanySql, targetConnection);
                cmdUpdateEntityCompany.ExecuteNonQuery();

            }
        }

    }
}
