using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentObjectAccessHistoryModel
    {
        public int AccessId { get; set; }
        public int DocumentObjectId { get; set; }
        public string DocumentName { get; set; }
        public int EntityId { get; set; }
        public string MemberName { get; set; }
        public int AccessType { get; set; }
        public DateTime AccessDate { get; set; }
        public int StaffUserId { get; set; }
        public string UserType { get; set; }
    }
}
