using System;
using System.Collections.Generic;
using System.Text;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace Max.Core.Models
{
    public class EntityRoleModel
    {
        public int EntityRoleId { get; set; }
        public int EntityId { get; set; }
        public int ContactRoleId { get; set; }
        public int CompanyId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
        public int? StaffUserId { get; set; }
        public int? IsDeleted { get; set; }
        public bool? HaveHistoricRecords { get; set; }
        public ContactRoleModel ContactRole { get; set; }
    }
}
