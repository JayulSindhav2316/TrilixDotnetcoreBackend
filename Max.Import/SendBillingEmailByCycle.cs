using Max.Core;
using MySqlConnector;
using System;
using System.Globalization;

namespace Max.Import
{
    class Program_nbma_billing
    {
        static void noMain(string[] args)
        {
            string cycleId = "67";
            string sourceConnectionString = "server=localhost;port=3306;database=max_nbma;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=max_nbma;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            //Get paper Invoices
            using var command = new MySqlCommand($"SELECT * FROM paperinvoice where paperbillingcycleid={cycleId} and status= 1", sourceConnection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string invoiceId = reader.GetValue(2).ToString();
              
                //Insert Entity for Email

                string insertSql = $"INSERT INTO billingemail (BillingCycleId,InvoiceId,Status) VALUES ({cycleId}, {invoiceId}, 0)";
                MySqlCommand cmdInsertEntity = new MySqlCommand(insertSql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
               
            }
            reader.Close();
        }

    }
}
