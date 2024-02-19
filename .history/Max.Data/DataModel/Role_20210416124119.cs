using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Core.Models
{
    public partial class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
    }
}
