using Max.Core;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Import
{
    class program_update_date
    {
        static void Main_update(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=trilix_bishop;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=trilix_bishop;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            //using var targetConnection = new MySqlConnection(targetConnectionString);
            //targetConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM dates_update;", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string documentName = reader.GetValue(0).ToString();
                string createdDate = reader.GetValue(1).ToString();
              
                string currentDate = "2022-03-01";
                Console.WriteLine($"Updating Document  {documentName}|{createdDate}");
                string dbo_mysql = string.Empty;
                string documentDate = string.Empty;
                try
                {
                    DateTime tempDate = DateTime.Parse(createdDate);
                    documentDate = tempDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    documentDate = "2022-10-31";
                }


                //Find the document object 
                using var targetConnection = new MySqlConnection(targetConnectionString);
                targetConnection.Open();
                documentName = documentName.Replace("\\", "/");
                string documentObjectSql = $"select DocumentObjectId from documentobject where Concat(PathName,'/',FIlename) = '{documentName.Replace("'", "''")}'";
                MySqlCommand documentObjectCommand = new MySqlCommand(documentObjectSql, targetConnection);
               
                using var documentReader = documentObjectCommand.ExecuteReader();
                string documentObjectId = string.Empty;
                while (documentReader.Read())
                {
                    documentObjectId = documentReader.GetValue(0).ToString();
                }
                documentReader.Close();

                //Update  Created Date
                if(documentObjectId.Length > 0)
                {
                    string UpdateCreatedDateSql = $"Update documentobject set CreatedDate = '{documentDate}' where documentObjectId = {documentObjectId}";
                    Console.WriteLine($"\t Updating  :{UpdateCreatedDateSql}");
                    MySqlCommand cmdUpdateDocument = new MySqlCommand(UpdateCreatedDateSql, targetConnection);
                    cmdUpdateDocument.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine($"\t No matching document found  :{documentObjectSql}");
                }
                targetConnection.Close();

            }
        }
    }
}
