using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentAccessModel
    {
        public int DocumentAccessId { get; set; }
        public int? DocumentObjectId { get; set; }
        public int? MembershipTypeId { get; set; }
        public int? GroupId { get; set; }
        public int? StaffRoleId { get; set; }
        public DocumentObjectModel DocumentObject { get; set; }
        public GroupModel Group { get; set; }
        public MembershipTypeModel MembershipType { get; set; }

        public RoleModel Roles { get; set; }
    }
}
