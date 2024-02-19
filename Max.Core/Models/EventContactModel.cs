using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EventContactModel
    {
        public int EventContactId { get; set; }
        public int? EventId { get; set; }
        public int? StaffId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? HidePhoneNumber { get; set; }
    }
}
