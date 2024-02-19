using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class CustomfielddataModel
    {
        public int Id { get; set; }
        public int? CustomFieldId { get; set; }
        public string Value { get; set; }
        public int? EntityId { get; set; }
    }
}
