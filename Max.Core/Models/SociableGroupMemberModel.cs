using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class SociableGroupMemberModel
    {
        public List<SociableGroupMemberNumberValue> gid { get; set; }
        public List<SociableGroupMemberNumberValue> entity_id { get; set; }
        public List<SociableGroupMemberStringValue> field_positions { get; set; }
        public List<SociableGroupMemberStringValue> type { get; set; }

        public SociableGroupMemberModel()
        {
            gid = new List<SociableGroupMemberNumberValue>();
            entity_id = new List<SociableGroupMemberNumberValue>();
            type = new List<SociableGroupMemberStringValue>();
            field_positions = new List<SociableGroupMemberStringValue>();
        }
    }

    public class SociableGroupMemberNumberValue
    {
        public int? target_id { get; set; }
    }

    public class SociableGroupMemberStringValue
    {
        public string target_id { get; set; }
    }
}
