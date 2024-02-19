using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Tag
    {
        public Tag()
        {
            Documenttags = new HashSet<Documenttag>();
        }

        public int TagId { get; set; }
        public string TagName { get; set; }

        public virtual ICollection<Documenttag> Documenttags { get; set; }
    }
}
