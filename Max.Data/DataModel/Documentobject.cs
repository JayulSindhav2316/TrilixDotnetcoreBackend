using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Documentobject
    {
        public Documentobject()
        {
            Documentaccesses = new HashSet<Documentaccess>();
            Documentobjectaccesshistories = new HashSet<Documentobjectaccesshistory>();
            Documenttags = new HashSet<Documenttag>();
        }

        public int DocumentObjectId { get; set; }
        public int? ContainerId { get; set; }
        public string FileName { get; set; }
        public int? FileType { get; set; }
        public string PathName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string BlobId { get; set; }
        public long? FileSize { get; set; }
        public int? Active { get; set; }
        public int? EntityId { get; set; }

        public virtual Documentcontainer Container { get; set; }
        public virtual Staffuser CreatedByNavigation { get; set; }
        public virtual ICollection<Documentaccess> Documentaccesses { get; set; }
        public virtual ICollection<Documentobjectaccesshistory> Documentobjectaccesshistories { get; set; }
        public virtual ICollection<Documenttag> Documenttags { get; set; }
    }
}