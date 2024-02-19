using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class CustomfieldlookupModel
    {
        public int Id { get; set; }
        public int? ModuleId { get; set; }
        public int? CustomFieldId { get; set; }
        public int? OrderOfDisplay { get; set; }
        public int? TabId { get; set; }
        public int? Status { get; set; }
        public int? BlockId { get; set; }
        public CustomFieldModel CustomField { get; set; }
    }
}
