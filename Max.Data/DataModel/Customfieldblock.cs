using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Customfieldblock
    {
        public Customfieldblock()
        {
            Customfieldlookups = new HashSet<Customfieldlookup>();
        }

        public int BlockId { get; set; }
        public string Name { get; set; }
        public int? IsExisting { get; set; }
        public int? ShowBlock { get; set; }
        public int? BlockFor { get; set; }
        public int? ModuleId { get; set; }
        public int? TabId { get; set; }

        public virtual Moduleinfo Module { get; set; }
        public virtual Tabinfo Tab { get; set; }
        public virtual ICollection<Customfieldlookup> Customfieldlookups { get; set; }
    }
}
