using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class AddSocialGroupMember
    {
        public List<GroupId> group_id { get; set; }
        public List<EntityId> entity_id { get; set; }
        public List<Label> label { get; set; }
        public List<GroupRole> group_roles { get; set; }

        public AddSocialGroupMember()
        {
            group_id = new List<GroupId>();
            entity_id = new List<EntityId>();
            label = new List<Label>();
            group_roles = new List<GroupRole>();
        }
    }

    public class EntityId
    {
        public string target_id { get; set; }
    }

    public class GroupRole
    {
        public string target_id { get; set; }
    }

    public class Label
    {
        public string value { get; set; }
    }

    public class GroupId
    {
        public string value { get; set; }
    }
}
