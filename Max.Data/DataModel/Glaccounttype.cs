using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Glaccounttype
    {
        public Glaccounttype()
        {
            Glaccounts = new HashSet<Glaccount>();
        }

        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Glaccount> Glaccounts { get; set; }
    }
}
