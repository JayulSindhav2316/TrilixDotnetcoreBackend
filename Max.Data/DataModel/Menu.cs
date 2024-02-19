using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Menu
    {
        public Menu()
        {
            Rolemenus = new HashSet<Rolemenu>();
        }

        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public int? GroupId { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public string RouterUrl { get; set; }
        public int Expanded { get; set; }
        public int Disabled { get; set; }
        public string Tooltip { get; set; }
        public string Title { get; set; }
        public int DisplayOrder { get; set; }
        public bool? Display { get; set; }

        public virtual Menugroup Group { get; set; }
        public virtual ICollection<Rolemenu> Rolemenus { get; set; }
    }
}
