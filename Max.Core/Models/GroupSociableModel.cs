using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GroupSociableModel
    {
        public GroupSociableModel()
        {
            Roles = new List<GroupRoleSociableModel>();
        }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? Status { get; set; }
        public List<GroupRoleSociableModel> Roles { get; set; }
    }
}
