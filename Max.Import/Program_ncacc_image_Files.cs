using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Twilio.TwiML.Voice;

namespace Max.Import
{
    class Program_ncacc_image
    {

        static void Main_ncacc_image(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=ncacc-demo;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=ncacc-demo;uid=root;password=Anit1066";

            string rootPathCommissioners = Path.Combine(@"D:/Projects/MemberMax/NCACC/Import_Images/Commissioner");
            string rootPathCounty = Path.Combine(@"D:/Projects/MemberMax/NCACC/Import_Images/County");

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            var countyDirectories = new DirectoryInfo(rootPathCounty);

            string[] counties = Directory.GetDirectories(rootPathCounty, "*", SearchOption.TopDirectoryOnly);

            Console.WriteLine("Uploading county images");

            //Import data for Counties

            FileInfo[] files = countyDirectories.GetFiles("*", SearchOption.AllDirectories);

            var countyImageFileName = string.Empty;

            foreach (var file in files)
            {
                Console.WriteLine($"Uploading county image: {file.Name}");

                //Insert image file name in DB

                //Insert Image Name
                string insertImage = $"INSERT INTO import_county_image(ImageName) VALUES ('{file.Name}')";
                MySqlCommand cmdInsertImageName = new MySqlCommand(insertImage, targetConnection);
                cmdInsertImageName.ExecuteNonQuery();
            }

            //Now we have images so link the county names with images

            /*
             * For County Seals, images are only for Type (column H in the Accounts spreadsheet) = County. 
             * The name of the photos is the just the County name, not followed by “County” (i.e. image will be “Alamance.png” 
             * and Account name is “Alamance County”)
             */
            using var command = new MySqlCommand("SELECT * FROM import_accounts Where Type='County';", sourceConnection);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                string accountId = reader.GetValue(0).ToString();
                string accountName = reader.GetValue(1).ToString();

                var countyNameArray = accountName.Split(" ");
                if (countyNameArray.Length > 0)
                {
                    string expectedFileName = countyNameArray[0];
                    Console.WriteLine($"Found an image for county : {accountName} Image {expectedFileName}.png");

                    //check if image exists
                    var searchedAImageName = string.Empty;
                    string imageSearchSql = $"select * from import_county_image where ImageName = '{expectedFileName}.png'";
                    MySqlCommand imageSearchCommand = new MySqlCommand(imageSearchSql, targetConnection);
                    using var imageReader = imageSearchCommand.ExecuteReader();
                    while (imageReader.Read())
                    {
                        searchedAImageName = imageReader.GetValue(1).ToString();
                    }
                    imageReader.Close();

                    if(searchedAImageName != string.Empty)
                    {
                        Console.WriteLine($"Updating AccountId for county : {accountId} Image {searchedAImageName}");

                        var updateImageIdSql = $"Update import_county_image set AccountId = '{accountId}' where ImageName = '{searchedAImageName}'";
                        MySqlCommand cmdUpdateImageId = new MySqlCommand(updateImageIdSql, targetConnection);
                        cmdUpdateImageId.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine($"Image not found for county : {accountId} {accountName} Image {searchedAImageName}");
                    }
                }
                    

            }
        }
    }
}
