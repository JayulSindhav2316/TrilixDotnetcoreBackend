using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Document
    {
        public int DocumentId { get; set; }
        public int? EntityId { get; set; }
        public int? OrganizationId { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string DisplayFileName { get; set; }
        public int? StaffId { get; set; }
        public int? EventId { get; set; }
        public int? EventBannerImageId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
