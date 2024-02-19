using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core
{
    /// <summary>
    /// Global Constants
    /// </summary>
    public static class Constants
    {
        #region Global Constants
        public static readonly DateTime MySQL_MinDate = new DateTime(1000, 1, 1);
        public static readonly DateTime MySQL_MaxDate = new DateTime(9999, 12, 31);

        /// <summary>
        /// .net Core Authroization scheme tries to match roles with a specfic cliam name
        /// so make sure that we supply the expected claim
        /// </summary>
        public static readonly string RoleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public static class Roles
        {
            public const string AdminRole = "Admin";
            public const string InternalRole = "Internal";
            public const string ExternalRole = "External";
          
        }

      

        #endregion Global Constants
    }
}
