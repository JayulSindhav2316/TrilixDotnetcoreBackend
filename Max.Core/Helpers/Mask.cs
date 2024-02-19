using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Max.Core.Helpers
{
    public class Mask
    {
        public static string Email(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            string[] emailArr = email.Split('@');
            string domainExt = Path.GetExtension(email);

            string maskedEmail = string.Format("{0}****{1}@{2}****{3}{4}",
                emailArr[0][0],
                emailArr[0].Substring(emailArr[0].Length - 1),
                emailArr[1][0],
                emailArr[1].Substring(emailArr[1].Length - domainExt.Length - 1, 1),
                domainExt
                );

            return maskedEmail;
        }

        public static string Phone(string phone)
        {
            if (string.IsNullOrEmpty(phone) )
                return phone;

            if(phone.Length < 10)
            {
                return "Invalid Phone number";
            }
            int length = phone.Length;
            string maskedPhone = $"XXXXXXXX{phone.Substring(phone.Length-2, 2)}";

            return maskedPhone;
        }
    }
}
