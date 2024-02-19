using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Tabinfo
    {
        public Tabinfo()
        {
            Customfieldblocks = new HashSet<Customfieldblock>();
        }

        public int TabId { get; set; }
        public int? ModuleId { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public virtual Moduleinfo Module { get; set; }
        public virtual ICollection<Customfieldblock> Customfieldblocks { get; set; }
    }
}
