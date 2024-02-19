using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Contactrole
    {
        public Contactrole()
        {
            Contactactivities = new HashSet<Contactactivity>();
            Entityrolehistories = new HashSet<Entityrolehistory>();
            Entityroles = new HashSet<Entityrole>();
        }

        public int ContactRoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Active { get; set; }

        public virtual ICollection<Contactactivity> Contactactivities { get; set; }
        public virtual ICollection<Entityrolehistory> Entityrolehistories { get; set; }
        public virtual ICollection<Entityrole> Entityroles { get; set; }
    }
}
