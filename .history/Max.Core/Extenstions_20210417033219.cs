using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core
{
    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class Extenstions
    {
        /// <summary>
        /// Tests to see if the string contains a valid integer number
        /// </summary>
        /// <param name="value"></param>
        public static bool IsInteger(this string value)
        {
            int testValue;
            return int.TryParse(value, out testValue);
        }
    }
}
