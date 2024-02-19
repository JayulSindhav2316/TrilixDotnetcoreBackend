using System;
using System.Collections.Generic;
using System.Text;

namespace Hylaine.Hope.Core
{
    /// <summary>
    /// Global Constants
    /// </summary>
    public static class Constants
    {
        #region Global Constants
        public static readonly DateTime SQLServer_MinDate = new DateTime(1900, 1, 1);
        public static readonly DateTime SQLServer_MaxDate = new DateTime(9999, 12, 31);

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

        public static readonly string HeadOfHoueHold = "HOH";

        #endregion Global Constants
    }
}
