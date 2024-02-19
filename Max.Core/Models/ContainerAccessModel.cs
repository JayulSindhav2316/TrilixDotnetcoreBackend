using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ContainerAccessModel
    {
        public uint ContainerAccessId { get; set; }
        public int ContainerId { get; set; }
        public int MembershipTypeId { get; set; }
        public int MembershipCategoryId { get; set; }
        public  MembershipTypeModel MembershipType { get; set; }
        public GroupModel Group { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int StaffRoles { get; set; }
    }
}
