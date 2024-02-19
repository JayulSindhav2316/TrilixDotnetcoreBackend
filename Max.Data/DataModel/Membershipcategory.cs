using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Membershipcategory
    {
        public Membershipcategory()
        {
            Membershiptypes = new HashSet<Membershiptype>();
        }

        public int MembershipCategoryId { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Membershiptype> Membershiptypes { get; set; }
    }
}
