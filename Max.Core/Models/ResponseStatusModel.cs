using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Max.Core.Models
{
    public class ResponseStatusModel
    {
        public string Message { get; set; }
        public double OtpValidationSecondsLeft { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
