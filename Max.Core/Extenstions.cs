using Max.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Max.Core
{
    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class Extenstions
    {

        // Regular expression used to validate a phone number.
        public const string RegxPhone = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

        /// <summary>
        /// Tests to see if the string contains a valid integer number
        /// </summary>
        /// <param name="value"></param>
        /// 

        public static bool IsInteger(this string value)
        {
            int testValue;
            return int.TryParse(value, out testValue);
        }

        /// <summary>
        /// Remove mask charcters from phone number
        /// </summary>
        /// <param name="value"></param>
        public static string GetCleanPhoneNumber(this string value)
        {
            if (value == null) return string.Empty;
            return Regex.Replace(value, "[^0-9]", "");
        }
        /// <summary>
        /// Truncate string to max length
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                return value.Substring(0, maxLength);
            }

            return value;
        }
        /// <summary>
        /// Format Phone Numbers in US Format
        /// TODO: AKS -> We need to make international
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatPhoneNumber(this string value)
        {
            string format = "({0}) {1}-{2}";
            string phoneNumber = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            else
            {
                try
                {
                    if (value.Length < 10)
                    {
                        value = value.PadLeft(10, ' ');
                    }
                    string area = value.Substring(0, 3) ?? "";
                    string major = value.Substring(3, 3) ?? "";
                    string minor = value.Substring(6, 4) ?? "";

                    if (value.Length > 10)
                    {
                        if (value.Length < 14)
                        {
                            value = value.PadRight(14, ' ');
                        }
                        format = "({0}) {1}-{2} x{3}";

                        string extension = value.Substring(10, 4) ?? "";
                        if (value.Length == 15)
                        {
                            extension = value.Substring(10, 5) ?? "";
                        }
                        phoneNumber = string.Format(format, area, major, minor, extension);
                    }
                    else
                    {
                        phoneNumber = string.Format(format, area, major, minor);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return phoneNumber;
        }

        /// <summary>
        /// Get Primary Phone
        /// </summary>
        /// <param name="value"></param>
        public static string GetPrimaryPhoneNumber(this List<PhoneModel> value)
        {
            if (value == null) return string.Empty;

            if (value.Count == 0) return string.Empty;

            var primaryPhone = value.Where(x => x.IsPrimary == Constants.TRUE).FirstOrDefault();
            if (primaryPhone != null) return primaryPhone.PhoneNumber.GetCleanPhoneNumber();

            //Return first Item if  there is no primary item defined
            return value[0].PhoneNumber.GetCleanPhoneNumber();
        }

        /// <summary>
        /// Get Primary PhoneType
        /// </summary>
        /// <param name="value"></param>
        public static string GetPrimaryPhoneType(this List<PhoneModel> value)
        {
            if (value == null) return string.Empty;

            if (value.Count == 0) return string.Empty;

            var primaryPhone = value.Where(x => x.IsPrimary == Constants.TRUE).FirstOrDefault();
            if (primaryPhone != null) return primaryPhone.PhoneType;

            //Return first Item if  there is no primary item defined
            return value[0].PhoneType;
        }

        /// <summary>
        /// Get Primary Email
        /// </summary>
        /// <param name="value"></param>
        public static string GetPrimaryEmail(this List<EmailModel> value)
        {
            if (value == null) return string.Empty;

            if (value.Count == 0) return string.Empty;

            var primaryEmail = value.Where(x => x.IsPrimary == Constants.TRUE).FirstOrDefault();
            if (primaryEmail != null) return primaryEmail.EmailAddress;

            //Return first Item if  there is no primary item defined
            return value[0].EmailAddress;
        }

        /// <summary>
        /// Get Primary Email Type
        /// </summary>
        /// <param name="value"></param>
        public static string GetPrimaryEmailType(this List<EmailModel> value)
        {
            if (value == null) return string.Empty;

            if (value.Count == 0) return string.Empty;

            var primaryEmail = value.Where(x => x.IsPrimary == Constants.TRUE).FirstOrDefault();
            if (primaryEmail != null) return primaryEmail.EmailAddressType;

            //Return first Item if  there is no primary item defined
            return value[0].EmailAddressType;
        }

        /// <summary>
        /// Get Primary Address
        /// </summary>
        /// <param name="value"></param>
        public static AddressModel GetPrimaryAddress(this List<AddressModel> value)
        {
            if (value == null) return new AddressModel();

            if (value.Count == 0) return new AddressModel();

            var primaryAddress = value.Where(x => x.IsPrimary == Constants.TRUE).FirstOrDefault();
            if (primaryAddress != null) return primaryAddress;

            //Return first Item if  there is no primary item defined
            return value[0];
        }

        /// <summary>
        /// Check  if Enumerator is Null or Epty
        /// </summary>

        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source != null)
            {
                foreach (object obj in source)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (T obj in source)
                {
                    return false;
                }
            }
            return true;
        }
        public static string GetFileName(this IFormFile file)
        {
            return ContentDispositionHeaderValue.Parse(
                            file.ContentDisposition).FileName.ToString().Trim('"');
        }

        public static async Task<MemoryStream> GetFileStream(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream;
        }

        public static async Task<byte[]> GetFileArray(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream.ToArray();
        }

        /// Summary
        /// DateTime extentions
        /// 
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }
        public static string GetAbbreviatedMonthName(this int value)
        {
            DateTime date = new DateTime(2020, value, 1);
            return date.ToString("MMM");
        }
        public static string ToCurrency(this decimal decimalValue)
        {
            return $"{decimalValue:C}";
        }

        public static decimal RoundOff(this decimal decimalValue)
        {
            return Decimal.Round(decimalValue * 100) / 100;
        }
        public static bool isNumeric(this string value)
        {
            return int.TryParse(value, out int n);
        }
        public static bool IsPhoneNumber(this string number)
        {
            if (number != null)
            {
                return Regex.IsMatch(number, RegxPhone);
            }
            else
            {
                return false;
            }
        }

        /// <summary>  
        /// For calculating age  
        /// </summary>  
        /// <param name="Dob">Enter Date of Birth to Calculate the age</param>  
        /// <returns> years, months,days, hours...</returns>  
        public static string CalculateAge(this string DateOfBirth)
        {
            if (DateOfBirth != string.Empty && DateOfBirth != null)
            {
                DateTime Dob = Convert.ToDateTime(DateOfBirth);
                DateTime Now = DateTime.Now;
                int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
                DateTime PastYearDate = Dob.AddYears(Years);
                int Months = 0;
                for (int i = 1; i <= 12; i++)
                {
                    if (PastYearDate.AddMonths(i) == Now)
                    {
                        Months = i;
                        break;
                    }
                    else if (PastYearDate.AddMonths(i) >= Now)
                    {
                        Months = i - 1;
                        break;
                    }
                }
                //int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
                //int Hours = Now.Subtract(PastYearDate).Hours;
                //int Minutes = Now.Subtract(PastYearDate).Minutes;
                //int Seconds = Now.Subtract(PastYearDate).Seconds;
                return String.Format("{0} Year(s) {1} Month(s)", Years, Months);
            }
            else
            {
                return "";
            }

        }

        /// <summary>
        /// Format Zip in US Format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatZip(this string value)
        {
            string format = "{0}-{1}";
            string zip = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            else
            {
                try
                {
                    if (value.Length > 5 && value.Length == 9)
                    {
                        string preHyphen = value.Substring(0, 5);
                        string postHyphen = value.Substring(5, 4);
                        zip = string.Format(format, preHyphen, postHyphen);
                    }
                    else
                    {
                        zip = value;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return zip;
        }
        /// <summary>
        /// Replace first occurance of the searchText in a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchText"> search pattern</param>
        /// <param name="replaceText"> string to be replaced with</param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string searchText, string replaceText)
        {
            int position = text.IndexOf(searchText);
            if (position < 0)
            {
                return text;
            }
            return text.Substring(0, position) + replaceText + text.Substring(position + searchText.Length);
        }
        /// <summary>
        /// Get Connent message to string
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static string ContentToString(this HttpContent httpContent)
        {
            var readAsStringAsync = httpContent.ReadAsStringAsync();
            return readAsStringAsync.Result;
        }
        /// <summary>
        /// Put a string between double quotes.
        /// </summary>
        /// <param name="value">Value to be put between double quotes ex: foo</param>
        /// <returns>double quoted string ex: "foo"</returns>
        public static string AddDoubleQuotes(this string value)
        {
            return "\"" + value + "\"";
        }
    }
}
