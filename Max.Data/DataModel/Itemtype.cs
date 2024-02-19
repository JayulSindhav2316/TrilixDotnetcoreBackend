using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Itemtype
    {
        public Itemtype()
        {
            Items = new HashSet<Item>();
        }

        public int ItemTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
