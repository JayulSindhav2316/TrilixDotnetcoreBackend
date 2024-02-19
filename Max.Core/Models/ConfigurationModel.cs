using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ConfigurationModel
    {
        public int ConfigurationId { get; set; }
        public int OrganizationId { get; set; }
        public string DocumentAccessControl { get; set; }
        public string ContactDisplayTabs { get; set; }
        public int ContactDisplayMembership { get; set; }
        public string DashboardType { get; set; }
        public int DisplayDashboard { get; set; }
    }
}
