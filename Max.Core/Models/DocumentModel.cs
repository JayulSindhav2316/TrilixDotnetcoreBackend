using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentModel
    {
        public int DocumentId { get; set; }
        public int EntityId { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string DocumentRoot { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string DisplayFileName { get; set; }
        public string URL { get; set; }
        public Byte[] Document { get; set; }
        public int? OrganizationId { get; set; }
        public string TenantId { get; set; }
        public int StaffId { get; set; }
        public int EventId { get; set; }
        public int EventBannerImageId { get; set; }
        public string ProfileImageData { get; set; }
    }
}
