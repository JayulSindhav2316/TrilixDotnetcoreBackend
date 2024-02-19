using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Documenttag
    {
        public int DocumentTagId { get; set; }
        public int? DocumentObjectId { get; set; }
        public int? TagId { get; set; }
        public string TagValue { get; set; }

        public virtual Documentobject DocumentObject { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
