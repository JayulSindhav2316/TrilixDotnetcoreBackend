using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Documentcontainer
    {
        public Documentcontainer()
        {
            Containeraccesses = new HashSet<Containeraccess>();
            Documentobjects = new HashSet<Documentobject>();
        }

        public int ContainerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AccessControlEnabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string BlobContainerId { get; set; }
        public string EncryptionKey { get; set; }
        public int? EntityId { get; set; }

        public virtual Staffuser CreatedByNavigation { get; set; }
        public virtual ICollection<Containeraccess> Containeraccesses { get; set; }
        public virtual ICollection<Documentobject> Documentobjects { get; set; }
    }
}