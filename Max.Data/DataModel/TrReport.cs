using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class TrReport
    {
        public TrReport()
        {
            TrDefinitions = new HashSet<TrDefinition>();
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public ulong? Bookmark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual TrCategory Category { get; set; }
        public virtual TrTemplate Template { get; set; }
        public virtual ICollection<TrDefinition> TrDefinitions { get; set; }
    }
}
