using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Auditmetadata
    {
        public Auditmetadata()
        {
            Audits = new HashSet<Audit>();
        }

        public Guid HashPrimaryKey { get; set; }
        public string SchemaTable { get; set; }
        public string ReadablePrimaryKey { get; set; }
        public string Table { get; set; }
        public string DisplayName { get; set; }

        public virtual ICollection<Audit> Audits { get; set; }
    }
}
