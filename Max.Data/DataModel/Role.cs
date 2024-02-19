using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Role
    {
        public Role()
        {
            Containeraccesses = new HashSet<Containeraccess>();
            Documentaccesses = new HashSet<Documentaccess>();
            Rolemenus = new HashSet<Rolemenu>();
            Staffroles = new HashSet<Staffrole>();
        }

        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Containeraccess> Containeraccesses { get; set; }
        public virtual ICollection<Documentaccess> Documentaccesses { get; set; }
        public virtual ICollection<Rolemenu> Rolemenus { get; set; }
        public virtual ICollection<Staffrole> Staffroles { get; set; }
    }
}
