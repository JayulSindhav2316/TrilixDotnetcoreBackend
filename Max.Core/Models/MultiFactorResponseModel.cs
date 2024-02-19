using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MultiFactorResponseModel
    {
        public bool IsValid { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public bool RememberDevice { get; set; }
        public int Attempts { get; set; }
        public ResponseStatusModel ResponseMessage { get; set; }
    }
}
