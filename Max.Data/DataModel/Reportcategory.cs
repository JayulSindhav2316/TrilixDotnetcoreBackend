using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Reportcategory
    {
        public Reportcategory()
        {
            Reportfields = new HashSet<Reportfield>();
            Reportparameters = new HashSet<Reportparameter>();
        }

        public int ReportCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Reportfield> Reportfields { get; set; }
        public virtual ICollection<Reportparameter> Reportparameters { get; set; }
    }
}
