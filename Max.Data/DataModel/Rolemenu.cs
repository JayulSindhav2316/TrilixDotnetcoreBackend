﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Rolemenu
    {
        public int RoleMenuId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }

        public virtual Menu Menu { get; set; }
        public virtual Role Role { get; set; }
    }
}
