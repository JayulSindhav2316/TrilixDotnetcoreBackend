using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Report
    {
        public Report()
        {
            //Reportarguments = new HashSet<Reportargument>();
            Reportfilters = new HashSet<Reportfilter>();
            Reportshares = new HashSet<Reportshare>();
            Reportsortorders = new HashSet<Reportsortorder>();

            Membershipreports = new HashSet<Membershipreport>();
            Eventreports = new HashSet<Eventreport>();
            Opportunityreports = new HashSet<Opportunityreport>();
        }

        public int ReportId { get; set; }
        public int? OrganizationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public string ReportType { get; set; }
        public int isCommunity { get; set; }
        public int isFavorite { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int PreviewMode { get; set; }
        public string Fields { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual Staffuser User { get; set; }
        //public virtual ICollection<Reportargument> Reportarguments { get; set; }

        public virtual ICollection<Membershipreport> Membershipreports { get; set; }
        public virtual ICollection<Eventreport> Eventreports { get; set; }
        public virtual ICollection<Opportunityreport> Opportunityreports { get; set; }

        public virtual ICollection<Reportfilter> Reportfilters { get; set; }
        public virtual ICollection<Reportshare> Reportshares { get; set; }
        public virtual ICollection<Reportsortorder> Reportsortorders { get; set; }
    }
}
