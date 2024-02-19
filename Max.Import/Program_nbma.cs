using Max.Core;
using MySqlConnector;
using System;
using System.Globalization;

namespace Max.Import
{
    class Program_nbma
    {

        static void NoMain(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=max_nbma;uid=root;password=Anit1066";
            string targetConnectionString = "server=localhost;port=3306;database=max_nbma;uid=root;password=Anit1066";

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM import_dr_yu;", sourceConnection);
            using var reader = command.ExecuteReader();

            int currentFamilyId = 0;
            int lastFamilyId = 0;
            long billableEntityId = 0;
            while (reader.Read())
            {
                int family_id = reader.GetInt32(0);
                int person_id = reader.GetInt32(1);
                string last_name = reader.GetValue(2).ToString();
                string suffix = reader.GetValue(3).ToString();
                string first_name = reader.GetValue(4).ToString();
                string mid_name = reader.GetValue(5).ToString();
                string prefix = reader.GetValue(6).ToString();
                string dob = reader.GetValue(7).ToString();
                string gender = reader.GetValue(8).ToString();
                string renewalDate = reader.GetValue(9).ToString();
                string membershipType = reader.GetValue(11).ToString();
                string primary_phone = reader.GetValue(12).ToString();
                string email = reader.GetValue(13).ToString();
                string home_street_address = reader.GetValue(15).ToString();
                string home_city = reader.GetValue(16).ToString();
                string home_state = reader.GetValue(17).ToString();
                string home_zip = reader.GetValue(18).ToString();
                string billing_street_address = reader.GetValue(20).ToString();
                string billing_city = reader.GetValue(21).ToString();
                string billing_state = reader.GetValue(22).ToString();
                string billing_zip = reader.GetValue(23).ToString();
                string fee_amount = reader.GetValue(25).ToString();
                string process_date_1 = reader.GetValue(26).ToString();
                string payment_1 = reader.GetValue(27).ToString();
                string process_date_2 = reader.GetValue(28).ToString();
                string payment_2 = reader.GetValue(29).ToString();
                string process_date_3 = reader.GetValue(30).ToString();
                string payment_3 = reader.GetValue(31).ToString();
                string process_date_4 = reader.GetValue(32).ToString();
                string payment_4 = reader.GetValue(33).ToString();
                string next_due_date = reader.GetValue(38).ToString();
                string payment_method = reader.GetValue(39).ToString();
                //string note_1 = reader.GetValue(40).ToString();
                //string note_2 = reader.GetValue(41).ToString();
                //string note_3 = reader.GetValue(42).ToString();
                string note_1 = string.Empty;
                string note_2 = string.Empty;
                string note_3 = string.Empty;

                string currentDate = "2022-03-01";
                Console.WriteLine($"Creating Person {person_id}|{last_name}||{suffix}|{first_name}|{mid_name}|{prefix}|{dob}|{gender}|{primary_phone}|{email}|{home_street_address}|{home_city}|{home_state}|{home_zip}|{billing_street_address}|{billing_city}|{billing_state}|{billing_zip}");
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
                try
                {
                    DateTime membershipStartDate = DateTime.Parse(renewalDate);
                    startDate = membershipStartDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    startDate = "2000-01-01";
                }

                try
                {
                    DateTime membershipNextDueDate = DateTime.Parse(next_due_date);
                    nextDueDate = membershipNextDueDate.ToString("yyyy-MM-dd");
                    DateTime membershipStartDate = DateTime.Parse(startDate);
                    endDate = membershipStartDate.AddYears(1).ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    nextDueDate = "2021-01-01";
                    endDate = "2021-01-01";
                }

                //Payment Dates

                string payment_1_date = "2021-01-01";
                try
                {
                    DateTime paymentDate = DateTime.Parse(process_date_1);
                    payment_1_date = paymentDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    payment_1_date = "2021-01-01";
                }

                string payment_2_date = "2021-01-01";
                try
                {
                    DateTime paymentDate = DateTime.Parse(process_date_2);
                    payment_2_date = paymentDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    payment_2_date = "2021-01-01";
                }

                string payment_3_date = "2021-01-01";
                try
                {
                    DateTime paymentDate = DateTime.Parse(process_date_3);
                    payment_3_date = paymentDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    payment_3_date = "2021-01-01";
                }
                string payment_4_date = "2021-01-01";
                try
                {
                    DateTime paymentDate = DateTime.Parse(process_date_4);
                    payment_4_date = paymentDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    payment_4_date = "2021-01-01";
                }

                if (payment_1.Length >0)
                {
                    payment_1 = payment_1.Replace("$", "").Replace(",","");
                    try
                    {
                        var amount = float.Parse(payment_1);
                        payment_1 = amount.ToString();
                    }
                    catch (Exception ex)
                    {
                        payment_1 = "";
                    }
                }
                if (payment_2.Length > 0)
                {
                    payment_2 = payment_2.Replace("$", "").Replace(",", "");
                    try
                    {
                        var amount = float.Parse(payment_2);
                        payment_2 = amount.ToString();
                    }
                    catch (Exception ex)
                    {
                        payment_2 = "";
                    }
                }
                if (payment_3.Length > 0)
                {
                    payment_3 = payment_3.Replace("$", "").Replace(",", "");

                    try
                    {
                        var amount = float.Parse(payment_3);
                        payment_3 = amount.ToString();
                    }
                    catch (Exception ex)
                    {
                        payment_3 = "";
                    }
                }
                if (payment_4.Length > 0)
                {
                    payment_4 = payment_4.Replace("$", "").Replace(",", "");

                    try
                    {
                        var amount = float.Parse(payment_4);
                        payment_4 = amount.ToString();
                    }
                    catch (Exception ex)
                    {
                        payment_4 = "";
                    }
                }

                string paymentMethod = string.Empty;

                if(payment_method.StartsWith("CC"))
                {
                    paymentMethod = "CreditCard";
                }
                else if (payment_method.StartsWith("Ck"))
                {
                    paymentMethod = "Check";
                }
                else 
                {
                    paymentMethod = "Cash";
                }
                //Insert Entity for Person

                string entityPersonSql = $"INSERT INTO entity (Name,OrganizationId) VALUES ('{prefix} {first_name.Replace("'", "''") } {last_name.Replace("'", "''") }' ,1)";
                MySqlCommand cmdInsertEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdInsertEntity.ExecuteNonQuery();
                long personEntityId = cmdInsertEntity.LastInsertedId;

                //Insert  Person

                string personSql = $"INSERT INTO person(Prefix,FirstName,LastName,MiddleName,CasualName,Suffix,Gender,Title,Salutation,DateOfBirth,OrganizationId,Status,EntityId) VALUES ('{prefix}','{first_name.Replace("'", "''")}','{last_name.Replace("'", "''")}','{mid_name.Replace("'", "''")}','','{suffix}','{gender}','','','{birthdate}',1,1,{personEntityId})";
                MySqlCommand cmdInsertPerson = new MySqlCommand(personSql, targetConnection);
                cmdInsertPerson.ExecuteNonQuery();
                long personId = cmdInsertPerson.LastInsertedId;

                //Update  Person->Entity relation

                entityPersonSql = $"Update entity set PersonId = {personId} where entityId = {personEntityId}";
                MySqlCommand cmdUpdateEntity = new MySqlCommand(entityPersonSql, targetConnection);
                cmdUpdateEntity.ExecuteNonQuery();

                //Insert Address
                string addressSql = $"INSERT INTO address(AddressType,Address1,City,State,Zip,Country,PersonId,IsPrimary) VALUES ('Home','{home_street_address.Replace("'", "''")}','{home_city.Replace("'", "''")}','{home_state}','{home_zip.Replace(" ", "")}','USA',{personId},1)";
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
                string phoneSql = $"INSERT INTO phone(PhoneType, PhoneNumber, PersonId, IsPrimary) VALUES ('Home','{primary_phone.GetCleanPhoneNumber()}',{personId},1)";
                MySqlCommand cmdInsertPhone = new MySqlCommand(phoneSql, targetConnection);
                cmdInsertPhone.ExecuteNonQuery();

                if (family_id != currentFamilyId)
                {
                    lastFamilyId = currentFamilyId;
                    currentFamilyId = family_id;
                    billableEntityId = personEntityId;
                }
                else
                {
                    string companyPersonSql = $"INSERT INTO relation (EntityId,RelatedEntityId,RelationshipId,StartDate,Status) VALUES ({personEntityId},{billableEntityId},29,'{currentDate}',1)";
                    MySqlCommand cmdInsertRelation = new MySqlCommand(companyPersonSql, targetConnection);
                    cmdInsertRelation.ExecuteNonQuery();
                }

               
                int membershipTypeId = 0;
                int feeId = 0;
                string glAccount = string.Empty;

                //Select membership type

                string getMembershipType = $"select f.MembershipTypeId, f.FeeId,g.Code from membershipType m inner join membershipfee f on f.MembershipTypeId = m.MembershipTypeId inner join glaccount g on g.GlAccountId = f.GlAccountId where m.name like '{membershipType.Trim()}%' and m.description like 'Dr. Yu%' and m.PaymentFrequency=1; ";
                MySqlCommand cmdFeeQuery = new MySqlCommand(getMembershipType, targetConnection);
                using MySqlDataReader rdr = cmdFeeQuery.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine("{0}-{1}", rdr.GetInt32(0), rdr.GetInt32(1));
                    membershipTypeId = rdr.GetInt32(0);
                    feeId = rdr.GetInt32(1);
                    glAccount = rdr.GetString(2);
                }
                rdr.Close();


                if (membershipTypeId > 0)
                {
                    fee_amount = fee_amount.Replace("$", "").Replace(",", "");
                    //Insert into Membership table

                    string entityMembershipSql = $"INSERT INTO membership(MembershipTypeId,StartDate,EndDate,ReviewDate,BillableEntityId,NextBillDate,BillingOnHold,CreateDate,RenewalDate,TerminationDate,Status,AutoPayEnabled) VALUES ({membershipTypeId},'{startDate}','{endDate}','{endDate}',{billableEntityId},'{nextDueDate}',0,'2000-01-01','0001-01-01','0001-01-01',1,0)";
                    MySqlCommand cmdInsertMembership = new MySqlCommand(entityMembershipSql, targetConnection);
                    cmdInsertMembership.ExecuteNonQuery();
                    long membershipId = cmdInsertMembership.LastInsertedId;

                    //Insert into Membership Fee

                    string entityMembershipFeeSql = $"INSERT INTO billingfee(MembershipId, MembershipFeeId, Fee, Discount, Status) VALUES ({membershipId},{feeId},{fee_amount},0,1)";
                    MySqlCommand cmdInsertMembershipFee = new MySqlCommand(entityMembershipFeeSql, targetConnection);
                    cmdInsertMembershipFee.ExecuteNonQuery();
                    long membershipFeeId = cmdInsertMembershipFee.LastInsertedId;

                    //Insert into Membership Connection

                    string entityMembershipConnectionSql = $"INSERT INTO membershipconnection (MembershipId,EntityId,Status) VALUES({membershipId},{personEntityId},1)";
                    MySqlCommand cmdInsertMembershipConnection = new MySqlCommand(entityMembershipConnectionSql, targetConnection);
                    cmdInsertMembershipConnection.ExecuteNonQuery();
                    long membershipConnectionId = cmdInsertMembershipConnection.LastInsertedId;


                    ////Insert Invoice 

                    string entityInvoiceConnectionSql = $"INSERT INTO invoice (Date,EntityId,DueDate,BillingType,InvoiceType,MembershipId,BillableEntityId,Status,UserId,InvoiceItemType,PromoCodeId) VALUES('{startDate}',{personEntityId},'{startDate}','Membership','Individual',{membershipId},{billableEntityId},1,1,4,0)";
                    MySqlCommand cmdInsertInvoiceConnection = new MySqlCommand(entityInvoiceConnectionSql, targetConnection);
                    cmdInsertInvoiceConnection.ExecuteNonQuery();
                    long invoiceId = cmdInsertInvoiceConnection.LastInsertedId;

                    ////Insert InvoiceDetail

                    string entityInvoiceDetailConnectionSql = $"INSERT INTO invoicedetail(InvoiceId, Description, GlAccount, Price, Quantity, Discount, Amount, Status, FeeId, ItemType) VALUES({invoiceId},'Membership Fee','{glAccount}',{fee_amount},1,0,{fee_amount},1,{feeId},4)";
                    MySqlCommand cmdInsertInvoiceDetailConnection = new MySqlCommand(entityInvoiceDetailConnectionSql, targetConnection);
                    cmdInsertInvoiceDetailConnection.ExecuteNonQuery();
                    long invoiceDetailId = cmdInsertInvoiceDetailConnection.LastInsertedId;

                    if(payment_1.Length > 0)
                    {
                        //INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId)

                        string entityReceiptConnectionSql = $"INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId,PromoCodeId) VALUES('{payment_1_date}',1,'{paymentMethod}','{payment_method}',1,1,0)";
                        MySqlCommand cmdInsertReceiptConnection = new MySqlCommand(entityReceiptConnectionSql, targetConnection);
                        cmdInsertReceiptConnection.ExecuteNonQuery();
                        long receiptId = cmdInsertReceiptConnection.LastInsertedId;

                        //INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType)

                        string entityReceiptDetailConnectionSql = $"INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType) VALUES({receiptId},{personEntityId},1,{payment_1},{payment_1},1,'Membership Fee',{invoiceDetailId},0,0,4)";
                        MySqlCommand cmdInsertReceiptDetailConnection = new MySqlCommand(entityReceiptDetailConnectionSql, targetConnection);
                        cmdInsertReceiptDetailConnection.ExecuteNonQuery();

                        //INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, TransactionType, PaymentType, CardType, AccountNumber, RoutingNumber, BankName, AuthCode, ResponseCode, MessageDetails, Status, Result)

                        string entityPaymentConnectionSql = $"INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, PaymentType, AccountNumber, AuthCode, ResponseCode,Status, Result) VALUES('{payment_1_date}',{personEntityId},{receiptId},{payment_1},'{paymentMethod}','{payment_method}','','1',2,1)";
                        MySqlCommand cmdInsertPaymentConnection = new MySqlCommand(entityPaymentConnectionSql, targetConnection);
                        cmdInsertPaymentConnection.ExecuteNonQuery();
                    }
                    if (payment_2.Length > 0)
                    {
                        //INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId)

                        string entityReceiptConnectionSql = $"INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId,PromoCodeId) VALUES('{payment_2_date}',1,'{paymentMethod}','{payment_method}',1,1,0)";
                        MySqlCommand cmdInsertReceiptConnection = new MySqlCommand(entityReceiptConnectionSql, targetConnection);
                        cmdInsertReceiptConnection.ExecuteNonQuery();
                        long receiptId = cmdInsertReceiptConnection.LastInsertedId;

                        //INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType)

                        string entityReceiptDetailConnectionSql = $"INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType) VALUES({receiptId},{personEntityId},1,{payment_2},{payment_2},1,'Membership Fee',{invoiceDetailId},0,0,4)";
                        MySqlCommand cmdInsertReceiptDetailConnection = new MySqlCommand(entityReceiptDetailConnectionSql, targetConnection);
                        cmdInsertReceiptDetailConnection.ExecuteNonQuery();

                        //INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, TransactionType, PaymentType, CardType, AccountNumber, RoutingNumber, BankName, AuthCode, ResponseCode, MessageDetails, Status, Result)

                        string entityPaymentConnectionSql = $"INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, PaymentType, AccountNumber, AuthCode, ResponseCode,Status, Result) VALUES('{payment_2_date}',{personEntityId},{receiptId},{payment_2},'{paymentMethod}','{payment_method}','','1',2,1)";
                        MySqlCommand cmdInsertPaymentConnection = new MySqlCommand(entityPaymentConnectionSql, targetConnection);
                        cmdInsertPaymentConnection.ExecuteNonQuery();
                    }
                    if (payment_3.Length > 0)
                    {
                        //INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId)

                        string entityReceiptConnectionSql = $"INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId,PromoCodeId) VALUES('{payment_3_date}',1,'{paymentMethod}','{payment_method}',1,1,0)";
                        MySqlCommand cmdInsertReceiptConnection = new MySqlCommand(entityReceiptConnectionSql, targetConnection);
                        cmdInsertReceiptConnection.ExecuteNonQuery();
                        long receiptId = cmdInsertReceiptConnection.LastInsertedId;

                        //INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType)

                        string entityReceiptDetailConnectionSql = $"INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType) VALUES({receiptId},{personEntityId},1,{payment_3},{payment_3},1,'Membership Fee',{invoiceDetailId},0,0,4)";
                        MySqlCommand cmdInsertReceiptDetailConnection = new MySqlCommand(entityReceiptDetailConnectionSql, targetConnection);
                        cmdInsertReceiptDetailConnection.ExecuteNonQuery();

                        //INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, TransactionType, PaymentType, CardType, AccountNumber, RoutingNumber, BankName, AuthCode, ResponseCode, MessageDetails, Status, Result)

                        string entityPaymentConnectionSql = $"INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, PaymentType, AccountNumber, AuthCode, ResponseCode,Status, Result) VALUES('{payment_3_date}',{personEntityId},{receiptId},{payment_3},'{paymentMethod}','{payment_method}','','1',2,1)";
                        MySqlCommand cmdInsertPaymentConnection = new MySqlCommand(entityPaymentConnectionSql, targetConnection);
                        cmdInsertPaymentConnection.ExecuteNonQuery();
                    }
                    if (payment_4.Length > 0)
                    {
                        //INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId)

                        string entityReceiptConnectionSql = $"INSERT INTO receiptheader(Date, StaffId, PaymentMode, CheckNo, Status, OrganizationId,PromoCodeId) VALUES('{payment_4_date}',1,'{paymentMethod}','{payment_method}',1,1,0)";
                        MySqlCommand cmdInsertReceiptConnection = new MySqlCommand(entityReceiptConnectionSql, targetConnection);
                        cmdInsertReceiptConnection.ExecuteNonQuery();
                        long receiptId = cmdInsertReceiptConnection.LastInsertedId;

                        //INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType)

                        string entityReceiptDetailConnectionSql = $"INSERT INTO receiptdetail(ReceiptHeaderId, EntityId, Quantity, Rate, Amount, Status, Description, InvoiceDetailId, Tax, Discount, ItemType) VALUES({receiptId},{personEntityId},1,{payment_4},{payment_4},1,'Membership Fee',{invoiceDetailId},0,0,4)";
                        MySqlCommand cmdInsertReceiptDetailConnection = new MySqlCommand(entityReceiptDetailConnectionSql, targetConnection);
                        cmdInsertReceiptDetailConnection.ExecuteNonQuery();

                        //INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, TransactionType, PaymentType, CardType, AccountNumber, RoutingNumber, BankName, AuthCode, ResponseCode, MessageDetails, Status, Result)

                        string entityPaymentConnectionSql = $"INSERT INTO paymenttransaction(TransactionDate, EntityId, ReceiptId, Amount, PaymentType, AccountNumber, AuthCode, ResponseCode,Status, Result) VALUES('{payment_4_date}',{personEntityId},{receiptId},{payment_4},'{paymentMethod}','{payment_method}','','1',2,1)";
                        MySqlCommand cmdInsertPaymentConnection = new MySqlCommand(entityPaymentConnectionSql, targetConnection);
                        cmdInsertPaymentConnection.ExecuteNonQuery();
                    }
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
                }
                
            }
        }

    }
}
