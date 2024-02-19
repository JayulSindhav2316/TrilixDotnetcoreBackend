using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class CustomfieldoptionModel
    {
        public int OptionId { get; set; }
        public int? CustomFieldId { get; set; }
        public string Option { get; set; }
        public string Code { get; set; }

        //public virtual Customfield CustomField { get; set; }
    }
}
