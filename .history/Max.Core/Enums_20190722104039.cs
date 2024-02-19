using System;
using System.Collections.Generic;
using System.Text;

namespace Hylaine.Hope.Core
{
    /// <summary>
    /// Global Enums
    /// </summary>
    /// 

    // Address Type
    public enum AddressType 
    {
        UNKNOWN = -1,
        Mailing = 0,
        Physical = 1
    }

    // Gender
    public enum GenderType
    {
        UNKNOWN = -1,
        Male = 0,
        Female = 1
    }

    //Ethnicity
    public enum EthnicityType
    {
        UNKNOWN = -1,
        American = 0,
        Asian = 1,
        Hispanic = 2,
        Hawaiian =3
    }

    //Approval Status of Family
    public enum SponsorFamilyStatus
    {
        Created     = 0,    
        Approved    = 1,    //the people in the family may be assigned to sponsors, preferably all of them
        Assigned    = 2,    //the family, in its entirety, is assigned to sponsors
        Completed   = 3     //the family has picked up its donations for the event
    }
}
