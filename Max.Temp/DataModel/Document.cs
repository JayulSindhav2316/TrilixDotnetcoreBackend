using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Document
    {
        public int DocumentId { get; set; }
        public int? PersonId { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string DisplayFileName { get; set; }

        public virtual Person Person { get; set; }
    }
}
