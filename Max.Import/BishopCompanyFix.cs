using Max.Core;
using Max.Data.DataModel;
using MySqlConnector;
using System;
using System.Globalization;

namespace Max.Import
{
    class Program_bishop_fix
    {

        static void Main(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=bishop-pp;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=bishop-pp;uid=root;password=Anit1066";

            /*
                CREATE  TABLE fixcompanyid (`CompanyId` int(11) NOT NULL ,`CompanyName` varchar(100) COLLATE utf8_bin DEFAULT NULL, `EntityId` int(11) NOT NULL);

                insert into fixcompanyid(CompanyId,CompanyName,EntityId)
                select min(CompanyId) as firstId, companyName,EntityId
                    from company
                   where companyName in (
                              select companyName 
                                from company
                               group by companyName
                              having count(*) > 1
                       )
                   group by companyName;
            */

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            using var command = new MySqlCommand("SELECT CompanyId,CompanyName,EntityId FROM fixcompanyid;", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string companyId = reader.GetValue(0).ToString();
                string companyName = reader.GetValue(1).ToString();
                string entityId = reader.GetValue(2).ToString();
                //Update  Person

                string personSql = $"UPDATE Person SET companyId = {companyId} Where companyId in  ( Select companyId from  company  WHERE companyName='{companyName}');";
                Console.WriteLine($"Updating Person {personSql}");

                MySqlCommand cmdInsertPerson = new MySqlCommand(personSql, targetConnection);
                cmdInsertPerson.ExecuteNonQuery();

                //Update  Relation

                string relationSql = $"UPDATE Relation SET entityId = {entityId} Where EntityId in  ( Select EntityId from  company  WHERE companyName='{companyName}');";
                Console.WriteLine($"Updating Relation {relationSql}");

                MySqlCommand cmdRelationSql = new MySqlCommand(relationSql, targetConnection);
                cmdRelationSql.ExecuteNonQuery();

                //Delete Phones
                string phoneSql = $"Delete from Phone Where CompanyId in ( Select CompanyId from  company  WHERE companyId  != '{companyId}' and companyName='{companyName}');";
                Console.WriteLine($"Updating Phones {phoneSql}");

                MySqlCommand cmdPhoneSql = new MySqlCommand(phoneSql, targetConnection);
                cmdPhoneSql.ExecuteNonQuery();

                //Delete Emails
                string emailSql = $"Delete from Email Where CompanyId in ( Select CompanyId from  company  WHERE companyId  != '{companyId}' and companyName='{companyName}');";
                Console.WriteLine($"Updating Emails {emailSql}");

                MySqlCommand cmdEmailSql = new MySqlCommand(emailSql, targetConnection);
                cmdEmailSql.ExecuteNonQuery();

                //Delete Address
                string addressSql = $"Delete from Address Where CompanyId in ( Select CompanyId from  company  WHERE companyId  != '{companyId}' and companyName='{companyName}');";
                Console.WriteLine($"Updating Addresses {addressSql}");

                MySqlCommand cmdAddressSql = new MySqlCommand(addressSql, targetConnection);
                cmdAddressSql.ExecuteNonQuery();

                //Now delete the duplicate companies

                string deleteCmpanySql = $"Delete from company Where companyId  != '{companyId}' and  companyName='{companyName}';";
                Console.WriteLine($"Deleting duplicate companies {deleteCmpanySql}");

                MySqlCommand cmdDeleteCompany = new MySqlCommand(deleteCmpanySql, targetConnection);
                cmdDeleteCompany.ExecuteNonQuery();
            }
        }

    }
}
