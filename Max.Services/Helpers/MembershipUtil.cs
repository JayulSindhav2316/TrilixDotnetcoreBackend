using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Helpers
{
    public class MembershipUtil
    {
        public static DateTime GetRenewalEndDate( Membership membership)
        {
            DateTime renewalEndDate = DateTime.Now;
            if (membership != null)
            {
                renewalEndDate = membership.EndDate;
                Membershiptype membershiptype = membership.MembershipType;
                if (membershiptype != null)
                {
                    var period = membershiptype.PeriodNavigation;
                    if (period.PeriodUnit == "Year")
                    {
                        renewalEndDate = membership.EndDate.AddYears(period.Duration);
                    }
                    else if (period.PeriodUnit == "Month")
                    {
                        renewalEndDate = membership.EndDate.AddMonths(period.Duration);
                    }
                    else
                    {
                        renewalEndDate = membership.EndDate.AddDays(period.Duration);
                    }
                }
            }
            return renewalEndDate;
        }
    }
}
