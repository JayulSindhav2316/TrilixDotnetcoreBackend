using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SolrDocumentModel
    {
        public SolrDocumentModel() { }

        public SolrDocumentModel(SolrDocumentModel model)
        {
            this.Id = model.Id;
            this.TenantId = model.TenantId;
            this.DocumentId = model.DocumentId;
            this.Text = model.Text;
            this.CreatedBy = model.CreatedBy;
            this.CreatedDate = model.CreatedDate;
            this.HighlightText = model.HighlightText;
        }

        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("tenantId")]
        public string TenantId { get; set; }
        [SolrField("documentId")]
        public string DocumentId { get; set; }
        [SolrField("text")]
        public string Text { get; set; }
        [SolrField("createdBy")]
        public string CreatedBy { get; set; }
        [SolrField("createdDate")]
        public DateTime CreatedDate { get; set; }
        [SolrField("fileName")]
        public string FileName { get; set; }
        public string HighlightText { get; set; }
        [SolrField("score")]
        public double? Score { get; set; }
    }
}
