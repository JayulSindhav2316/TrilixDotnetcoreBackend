﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Core.Models
{
    public partial class Staffrole
    {
        public int StaffRoleId { get; set; }
        public int StaffId { get; set; }
        public int RoleId { get; set; }
    }
}