using Max.Core;
using MySqlConnector;
using System;
using System.Globalization;

namespace Max.Import
{
    class Program_NEFI
    {

        static void No_Main(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=membermax;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=max_nefi;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM membermax.import_wholesaler_members;", sourceConnection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string company_name = reader.GetValue(0).ToString();
                string first_name = reader.GetValue(1).ToString();
                string last_name = reader.GetValue(2).ToString();
                string email = reader.GetValue(3).ToString();
                string phone = reader.GetValue(4).ToString();
                string street_address = reader.GetValue(5).ToString();
                string city = reader.GetValue(6).ToString();
                string state = reader.GetValue(7).ToString();
                string zip = reader.GetValue(8).ToString();
                string duesBilled = reader.GetValue(12).ToString();
                string note = reader.GetValue(18).ToString();

                Console.WriteLine($"Creating Person {company_name}|{first_name}||{last_name}|{email}|{phone}|{street_address}|{city}|{state}|{zip}|{duesBilled}");

                //Insert Entity for Person

                string entityPersonSql = $"INSERT INTO entity (Name,OrganizationId) VALUES ('{first_name.Replace("'", "''") } {last_name.Replace("'", "''") }' ,1)";
                MySqlCommand cmdInsertEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long personEntityId = cmdInsertEntity.LastInsertedId;

                //Insert  Person

                string personSql = $"INSERT INTO person(Prefix,FirstName,LastName,MiddleName,CasualName,Suffix,Gender,Title,Salutation,DateOfBirth,OrganizationId,WebLoginName,WebPassword,Status,EntityId) VALUES ('','{first_name.Replace("'", "''")}','{last_name.Replace("'", "''")}','','','','Male','','','{Constants.MySQL_MinDate.ToShortDateString()}',1,'{email}','Password',1,{personEntityId})";
                MySqlCommand cmdInsertPerson = new MySqlCommand(personSql, targetConnection);
                cmdInsertPerson.ExecuteNonQuery();
                long personId = cmdInsertPerson.LastInsertedId;

                //Update  Person->Entity relation

                entityPersonSql = $"Update entity set PersonId = {personId} where entityId = {personEntityId}";
                MySqlCommand cmdUpdateEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdUpdateEntity.ExecuteNonQuery();

                //Insert Address
                string addressSql = $"INSERT INTO address(AddressType,Address1,City,State,Zip,Country,PersonId,IsPrimary) VALUES ('Work','{street_address.Replace("'", "''")}','{city.Replace("'", "''")}','{state}','{zip.Replace(" ", "")}','USA',{personId},1)";
                MySqlCommand cmdInsertAddress = new MySqlCommand(addressSql, targetConnection);
                cmdInsertAddress.ExecuteNonQuery();

                //Insert EMail
                string emailSql = $"INSERT INTO email (EmailAddressType,EmailAddress,PersonId,IsPrimary) VALUES ('Work','{email.Replace("'", "''")}',{personId},1)";
                MySqlCommand cmdInsertEmail = new MySqlCommand(emailSql, targetConnection);
                cmdInsertEmail.ExecuteNonQuery();

                //Insert Phone
                string phoneSql = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Work','{phone.GetCleanPhoneNumber()}',{personId},1)";
                MySqlCommand cmdInsertPhone = new MySqlCommand(phoneSql, targetConnection);
                cmdInsertPhone.ExecuteNonQuery();

                //Insert Note
                if(note.Length >0 )
                {
                    string noteSql = $"INSERT INTO note (EntityId,Notes,Severity,DisplayOnProfile,CreatedOn,CreatedBy,Status) VALUES ({personEntityId}, '{note.Replace("'", "''")}','General',1,'{Constants.MySQL_MinDate.ToShortDateString()}','admin',1)";
                    MySqlCommand cmdInsertNote = new MySqlCommand(noteSql, targetConnection);
                    cmdInsertNote.ExecuteNonQuery();
                }

                //Insert Entity for Company

                string entitySql = $"INSERT INTO entity (Name,OrganizationId) VALUES ('{company_name.Replace("'", "''")}',1)";
                cmdInsertEntity = new MySqlCommand(entitySql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long companyEntityId = cmdInsertEntity.LastInsertedId;

                //Insert Company

                string companySql = $"INSERT INTO company(CompanyName,StreetAddress,City,State,Zip,Email,Phone,Website,BillableContactId,EntityId) VALUES ('{company_name.Replace("'", "''")}','{street_address.Replace("'", "''")}','{city}','{state}','{zip.Replace(" ", "")}','{email}','{phone.GetCleanPhoneNumber()}','',{personId},{companyEntityId})";
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

                ////Set Person to Company Relation

                //string personCompanySql = $"INSERT INTO relation (EntityId,RelatedEntityId,RelationshipId,StartDate,Status) VALUES ({personEntityId},{companyEntityId},28,'{Constants.MySQL_MinDate.ToShortDateString()}',1)";
                //MySqlCommand cmdInsertReverseRelation = new MySqlCommand(personCompanySql, targetConnection);
                //cmdInsertReverseRelation.ExecuteNonQuery();

                int membershipType = 0;
                int feeId = 0;
                string glAccount = string.Empty;
                if (duesBilled.Length > 0)
                {
                    decimal due = Decimal.Parse(duesBilled, NumberStyles.Currency);
                    Console.WriteLine($"Fee {due}");

                    string getMembershipType = $"select f.MembershipTypeId, f.FeeId,g.Code from membershipType m inner join membershipfee f on f.MembershipTypeId = m.MembershipTypeId inner join glaccount g on g.GlAccountId = f.GlAccountId where m.name like '%Wholesale%' and feeAmount = {due}; ";
                    MySqlCommand cmdFeeQuery = new MySqlCommand(getMembershipType, targetConnection);
                    using MySqlDataReader rdr = cmdFeeQuery.ExecuteReader();

                    while (rdr.Read())
                    {
                        Console.WriteLine("{0}-{1}", rdr.GetInt32(0), rdr.GetInt32(1));
                        membershipType = rdr.GetInt32(0);
                        feeId = rdr.GetInt32(1);
                        glAccount = rdr.GetString(2);
                    }
                    rdr.Close();

                    if(membershipType>0)
                    {
                        //Insert into Membership table

                        string entityMembershipSql = $"INSERT INTO membership(MembershipTypeId,StartDate,EndDate,ReviewDate,BillableEntityId,NextBillDate,BillingOnHold,CreateDate,RenewalDate,TerminationDate,Status,AutoPayEnabled) VALUES ({membershipType},'2021-01-01','2021-12-31','0001-01-01',{companyEntityId},'2022-01-01',0,'2021-01-01','0001-01-01','0001-01-01',1,0)";
                        MySqlCommand cmdInsertMembership = new MySqlCommand(entityMembershipSql, targetConnection);
                        cmdInsertMembership.ExecuteNonQuery();
                        long membershipId = cmdInsertMembership.LastInsertedId;

                        //Insert into Membership Fee

                        string entityMembershipFeeSql = $"INSERT INTO billingfee(MembershipId, MembershipFeeId, Fee, Discount, Status) VALUES ({membershipId},{feeId},{due},0,1)";
                        MySqlCommand cmdInsertMembershipFee = new MySqlCommand(entityMembershipFeeSql, targetConnection);
                        cmdInsertMembershipFee.ExecuteNonQuery();
                        long membershipFeeId = cmdInsertMembershipFee.LastInsertedId;

                        //Insert into Membership Connection

                        string entityMembershipConnectionSql = $"INSERT INTO membershipconnection (MembershipId,EntityId,Status) VALUES({membershipId},{companyEntityId},1)";
                        MySqlCommand cmdInsertMembershipConnection = new MySqlCommand(entityMembershipConnectionSql, targetConnection);
                        cmdInsertMembershipConnection.ExecuteNonQuery();
                        long membershipConnectionId = cmdInsertMembershipConnection.LastInsertedId;


                        ////Insert Invoice 

                        //string entityInvoiceConnectionSql = $"INSERT INTO invoice (Date,EntityId,DueDate,BillingType,InvoiceType,MembershipId,BillableEntityId,Status,UserId,InvoiceItemType,PromoCodeId) VALUES('2021-01-01',{personEntityId},'2021-01-01','Membership','Individual',{membershipId},{companyEntityId},1,1,5,0)";
                        //MySqlCommand cmdInsertInvoiceConnection = new MySqlCommand(entityInvoiceConnectionSql, targetConnection);
                        //cmdInsertInvoiceConnection.ExecuteNonQuery();
                        //long invoiceId = cmdInsertInvoiceConnection.LastInsertedId;

                        ////Insert InvoiceDetail

                        //string entityInvoiceDetailConnectionSql = $"INSERT INTO invoicedetail(InvoiceId, Description, GlAccount, Price, Quantity, Discount, Amount, Status, FeeId, ItemType) VALUES({invoiceId},'Membership Fee','{glAccount}',{due},1,0,{due},1,{feeId},4)";
                        //MySqlCommand cmdInsertInvoiceDetailConnection = new MySqlCommand(entityInvoiceDetailConnectionSql, targetConnection);
                        //cmdInsertInvoiceDetailConnection.ExecuteNonQuery();
                        //long invoiceDetailId = cmdInsertInvoiceDetailConnection.LastInsertedId;
                    }
                    

                }

            }
        }


    }
}
