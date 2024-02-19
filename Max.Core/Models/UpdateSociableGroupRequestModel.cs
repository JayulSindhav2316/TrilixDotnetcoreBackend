using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class UpdateSociableGroupRequestModel
    {
        public List<UpdateSociableGroupNumberValue> groupId { get; set; }
        public List<UpdateSociableGroupStringValue> field_name { get; set; }
        public List<UpdateSociableGroupStringValue> field_group_description { get; set; }
        public List<UpdateSociableGroupNumberValue> field_target_size { get; set; }
        public List<UpdateSociableGroupBoolValue> field_isactive { get; set; }
        public List<UpdateSociableGroupBoolValue> field_term { get; set; }
        public List<UpdateSociableGroupStringValue> field_start_date { get; set; }
        public List<UpdateSociableGroupStringValue> field_end_date { get; set; }
        public List<UpdateSociableGroupStringValue> field_group_positions { get; set; }
        public CreateSociableGroupRequestLinks _links { get; set; }

        public UpdateSociableGroupRequestModel()
        {
            field_name = new List<UpdateSociableGroupStringValue>();
            field_group_description = new List<UpdateSociableGroupStringValue>();
            field_target_size = new List<UpdateSociableGroupNumberValue>();
            field_isactive = new List<UpdateSociableGroupBoolValue>();
            field_term = new List<UpdateSociableGroupBoolValue>();
            field_start_date = new List<UpdateSociableGroupStringValue>();
            field_end_date = new List<UpdateSociableGroupStringValue>();
            field_group_positions = new List<UpdateSociableGroupStringValue>();
            _links = new CreateSociableGroupRequestLinks();
        }
    }

    public class UpdateSociableGroupRoleRequestModel
    {
        public List<UpdateSociableGroupStringValue> field_group_positions { get; set; }
        public CreateSociableGroupRequestLinks _links { get; set; }
        public UpdateSociableGroupRoleRequestModel()
        {
            field_group_positions = new List<UpdateSociableGroupStringValue>();
            _links = new CreateSociableGroupRequestLinks();
        }
    }

    public class UpdateSociableGroupStringValue
    {
        public string value { get; set; }
    }

    public class UpdateSociableGroupBoolValue
    {
        public bool value { get; set; }
    }

    public class UpdateSociableGroupNumberValue
    {
        public int? value { get; set; }
    }
}
