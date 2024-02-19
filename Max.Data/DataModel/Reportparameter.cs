using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Reportparameter
    {
        public Reportparameter()
        {
            //Reportfilters = new HashSet<Reportfilter>();
        }

        public int ReportParameterId { get; set; }
        public int? CategoryId { get; set; }
        public string Parameter { get; set; }
        public string Type { get; set; }

        public virtual Reportcategory Category { get; set; }
        //public virtual ICollection<Reportfilter> Reportfilters { get; set; }
    }
}
