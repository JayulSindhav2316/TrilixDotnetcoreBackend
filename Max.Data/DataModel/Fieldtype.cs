using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Fieldtype
    {
        public Fieldtype()
        {
            Customfields = new HashSet<Customfield>();
        }

        public int FieldTypeId { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Customfield> Customfields { get; set; }
    }
}
