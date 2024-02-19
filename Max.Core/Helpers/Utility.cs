using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Helpers
{
    public class Utility
    {
        public string GenerateRandomCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < Constants.PromoCodeLength; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }
    }
}
