using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Staffuser
    {
        public Staffuser()
        {
            Communications = new HashSet<Communication>();
            Membershiphistories = new HashSet<Membershiphistory>();
            Staffroles = new HashSet<Staffrole>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public int? Status { get; set; }
        public DateTime? LastAccessed { get; set; }
        public string Salt { get; set; }

        public virtual ICollection<Communication> Communications { get; set; }
        public virtual ICollection<Membershiphistory> Membershiphistories { get; set; }
        public virtual ICollection<Staffrole> Staffroles { get; set; }
    }
}
