using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Group
    {
        public Group()
        {
            Containeraccesses = new HashSet<Containeraccess>();
            Documentaccesses = new HashSet<Documentaccess>();
            Groupmembers = new HashSet<Groupmember>();
            Linkgrouproles = new HashSet<Linkgrouprole>();
        }

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int? PreferredNumbers { get; set; }
        public int? ApplyTerm { get; set; }
        public DateTime? TerrmStartDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public int? OrganizationId { get; set; }
        public int? Status { get; set; }
        public int? Sync { get; set; }
        public int? SocialGroupId { get; set; }
        public virtual ICollection<Containeraccess> Containeraccesses { get; set; }
        public virtual ICollection<Documentaccess> Documentaccesses { get; set; }
        public virtual ICollection<Groupmember> Groupmembers { get; set; }
        public virtual ICollection<Linkgrouprole> Linkgrouproles { get; set; }
    }
}
