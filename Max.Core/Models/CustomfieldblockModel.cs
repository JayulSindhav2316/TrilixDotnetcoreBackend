using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class CustomfieldblockModel
    {
        public CustomfieldblockModel()
        {
            Customfieldlookups = new HashSet<CustomfieldlookupModel>();
        }

        public int BlockId { get; set; }
        public string Name { get; set; }
        public int? IsExisting { get; set; }
        public int? ShowBlock { get; set; }
        public int? BlockFor { get; set; }
        public int? ModuleId { get; set; }
        public int? TabId { get; set; }

        public string ModuleName { get; set; }
        public string TabName { get; set; }
        //public virtual Moduleinfo Module { get; set; }
        //public virtual Tabinfo Tab { get; set; }
        public virtual ICollection<CustomfieldlookupModel> Customfieldlookups { get; set; }
    }
}
