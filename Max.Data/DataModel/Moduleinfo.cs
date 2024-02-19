using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Moduleinfo
    {
        public Moduleinfo()
        {
            Customfieldblocks = new HashSet<Customfieldblock>();
            Customfieldlookups = new HashSet<Customfieldlookup>();
            Tabinfos = new HashSet<Tabinfo>();
        }

        public int ModuleId { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Customfieldblock> Customfieldblocks { get; set; }
        public virtual ICollection<Customfieldlookup> Customfieldlookups { get; set; }
        public virtual ICollection<Tabinfo> Tabinfos { get; set; }
    }
}
