using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Menu
    {
        public Menu()
        {
            Rolemenus = new HashSet<Rolemenu>();
        }

        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Group { get; set; }

        public virtual ICollection<Rolemenu> Rolemenus { get; set; }
    }
}
