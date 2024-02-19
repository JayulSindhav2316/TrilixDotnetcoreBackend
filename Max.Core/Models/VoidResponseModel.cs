using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class VoidResponseModel
    {
        public int UserId { get; set; }
        public int ReceiptId { get; set; }
        public int VoidDetailId { get; set; }
        public int Status { get; set; }
        public bool IsAlreadyVoided { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        
    }
}
