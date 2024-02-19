using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentObjectModel
    {
        public DocumentObjectModel()
        {
            DocuemntObjectAccessHistory = new List<DocumentObjectAccessHistoryModel>();
            DocumentTags = new List<DocumentTagModel>();
        }

        public int DocumentObjectId { get; set; }
        public int OrganizationId { get; set; }
        public string TenantId { get; set; }
        public int ContainerId { get; set; }
        public string FileName { get; set; }
        public int FileType { get; set; }
        public long FileSize { get; set; }
        public string PathName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public int CreatedBy { get; set; }
        public string BlobId { get; set; }
        public string HighlightText { get; set; }
        public double? Score { get; set; }
        public string TempRootFolder { get; set; }
        public string AccessList { get; set; }
        public string TagList { get; set; }
        public int AccessControlEnabled { get; set; }
        public List<DocumentTagModel> DocumentTags { get; set; }
        public List<SelectListModel> SelectedTags { get; set; }
        public StaffUserModel CreatedByNavigation { get; set; }
        public List<DocumentObjectAccessHistoryModel> DocuemntObjectAccessHistory { get; set; }
        public List<int> StaffRoles { get; set; }
        public int entityId { get; set; }
    }
}
