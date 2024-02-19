using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ContainerRequestModel
    {

        public int ContainerId { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AccessControlEnabled { get; set; }
        public List<int> MembershipTypes { get; set; }
        public List<int> Groups { get; set; }
        public List<int> StaffRoles { get; set; }
        public int UserId { get; set; }
        public int? EntityId { get; set; }
    }
}
