using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentObjectRequestModel
    {

        public int DocumentObjectId { get; set; }
        public int OrganizationId { get; set; }
        public string TenantId { get; set; }
        public int StaffId { get; set; }
        public int EntityId { get; set; }
        public string TempRootFolder { get; set; }
    }
}
