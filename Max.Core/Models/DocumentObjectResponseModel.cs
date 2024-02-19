using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentObjectResponseModel
    {
        public int DocumentObjectId { get; set; }
        public byte[] Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentUrl { get; set; }
        public string ResponseMessage { get; set; }
    }
}
