using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Menugroup
    {
        public Menugroup()
        {
            Menus = new HashSet<Menu>();
        }

        public int MenuGroupId { get; set; }
        public string GroupName { get; set; }
        public int Status { get; set; }
        public int DisplayOrder { get; set; }
        public string MenuName { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
