using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class CreateSociableGroupResponseModel
    {
        public List<CreateSociableGroupResponseIds> id { get; set; }
        public List<FieldEndDate> field_end_date { get; set; }
        public List<FieldGroupDescription> field_group_description { get; set; }
        public List<FieldGroupPosition> field_group_positions { get; set; }
        public List<FieldIsactive> field_isactive { get; set; }
        public List<FieldName> field_name { get; set; }
        public List<FieldStartDate> field_start_date { get; set; }
        public List<FieldTargetSize> field_target_size { get; set; }
        public List<FieldTerm> field_term { get; set; }

        public CreateSociableGroupResponseModel()
        {
            field_group_positions = new List<FieldGroupPosition>();
        }
    }


    public class FieldEndDate
    {
        public DateTime value { get; set; }
    }

    public class FieldGroupDescription
    {
        public string value { get; set; }
        public object format { get; set; }
        public string processed { get; set; }
    }

    public class FieldGroupPosition
    {
        public string value { get; set; }
    }

    public class FieldIsactive
    {
        public bool value { get; set; }
    }

    public class FieldName
    {
        public string value { get; set; }
        public object format { get; set; }
        public string processed { get; set; }
    }

    public class FieldStartDate
    {
        public DateTime value { get; set; }
    }

    public class FieldTargetSize
    {
        public int value { get; set; }
    }

    public class FieldTerm
    {
        public bool value { get; set; }
    }

    public class CreateSociableGroupResponseIds
    {
        public int value { get; set; }
    }
}
