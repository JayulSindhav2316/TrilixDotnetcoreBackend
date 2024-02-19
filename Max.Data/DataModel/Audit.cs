using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Audit
    {
        public Guid Id { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime DateTimeOffset { get; set; }
        public int EntityState { get; set; }
        public string ByUser { get; set; }
        public Guid? AuditMetaDataHashPrimaryKey { get; set; }
        public string AuditMetaDataSchemaTable { get; set; }

        public virtual Auditmetadata AuditMetaData { get; set; }
    }
}
