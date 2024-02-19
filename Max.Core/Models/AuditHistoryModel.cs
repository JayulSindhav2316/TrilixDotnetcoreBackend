using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class AuditHistoryModel
    {
        public AuditHistoryModel() { }   

        public string AuditSummary { get; set; }
        public string DisplayName { get; set; }
        public string PrimaryKey { get; set; }
        public string EntityStatus { get; set; }
        public DateTimeOffset DateTimeOffSet { get; set; }
        public List<AuditChangesModel> AuditChanges { get; set; }
    }

    public class AuditChangesModel
    {
          public string AuditChangesText { get; set; }   
    }
}
