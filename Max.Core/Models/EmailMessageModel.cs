using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EmailMessageModel
    {
        public string OrganizationName { get; set; }
        public int EntityId { get; set; }
        public int InvoiceId { get; set; }
        public int ReceiptId { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public string ReceipientAddress { get; set; }
        public byte[] PdfData { get; set; }
        [JsonIgnore]
        public InvoiceModel invoice { get;set; }
    }
}
